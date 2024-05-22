using bvData;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static bvAPI.bvAPI;

public static class bvCore
{
    public static class Events
    {
        public static UnityEvent<string> APIError = new UnityEvent<string>();

        public static UnityEvent MapUpdated = new UnityEvent();
        public static UnityEvent TrackedFactionsUpdated = new UnityEvent();
        public static UnityEvent<bvFaction> FactionSelected = new UnityEvent<bvFaction>();
        public static UnityEvent<bvSystem> SystemSelected = new UnityEvent<bvSystem>();
    }

    public static class Session
    {
        public static bvFaction[] factions = new bvFaction[0];
        public static bvSystem[] systems = new bvSystem[0];
        public static string _selectedFactionID = "";
        public static string selectedFactionID { get { return _selectedFactionID; } private set { _selectedFactionID = value; } }
        public static string _selectedSystemID = "";
        public static string selectedSystemID {get { return _selectedSystemID; } private set { _selectedSystemID = value; } }

        public static void RequestFactions(string[] faction_names) => exe.StartCoroutine(GetFactionData(faction_names, AddFactions));

        public static void AddFactions(bvFaction[] new_factions)
        {
            Debug.Log($"AddFactions({new_factions.Length})");
            List<bvFaction> known_factions = factions.ToList();

            foreach (bvFaction new_faction in new_factions)
            {
                //If we are not already tracking the faction, track it
                bool found = known_factions.Any(item => item.id == new_faction.id);
                if (!found)
                    known_factions.Add(new_faction);
            }

            factions = known_factions.ToArray();

            List<bvSystem> known_systems = systems.ToList();

            foreach(bvFaction faction in factions)
            {
                foreach(bvFaction.FactionPresence presence in faction.faction_presence)
                {
                    bool found = known_systems.Any(item => item.id == presence.id);
                    if (!found)
                        known_systems.Add(new bvSystem(presence));
                }
            }

            systems = known_systems.ToArray();
            Events.MapUpdated.Invoke();

            Debug.Log($"AddFactions({factions.Length})");
            Events.TrackedFactionsUpdated.Invoke();
        }

        public static void SetSelectedFaction(string id)
        {
            foreach (bvFaction trackedFaction in factions)
            {
                if (trackedFaction.id == id)
                {
                    selectedFactionID = trackedFaction.id;
                    Events.FactionSelected.Invoke(trackedFaction);
                }
            }
        }
        public static void SetSelectedSystem(string id)
        {
            foreach(bvSystem trackedSystem in systems)
            {
                if (trackedSystem.id == id)
                {
                    selectedSystemID = id;
                    Events.SystemSelected.Invoke(trackedSystem);
                }
            }
        }

        public static bool factionIsTracked(bvFaction faction)
        {
            if(factions.Length > 0)
                return factions.Any(item => item.id == faction.id);
            return false;
        }
    }

    public static CoroutineExecutor exe;
}
