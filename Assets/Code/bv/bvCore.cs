using bvData;
using static bvAPI.bvAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;

public static class bvCore
{
    public static class Events
    {
        public static UnityEvent<string> APIError = new UnityEvent<string>();

        public static UnityEvent UpdateMap = new UnityEvent();
        public static UnityEvent TrackedFactionsUpdated = new UnityEvent();
    }

    public static class Session
    {
        public static bvFaction[] factions = new bvFaction[0];
        public static bvSystem[] systems = new bvSystem[0];

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
                    factions.Append(new_faction);
                    known_factions.Add(new_faction);
            }

            factions = known_factions.ToArray();

            //RequestSystemData
            //For the initial data, just for the mapping itself, we only need the coordinates of the
            //system, which can be obtained from factions?systemDetails=true. Full system details can
            //be acquired from systems?factionDetails=true, and we can do this as a system is clicked
            //I dont think we need to cache this.
            Debug.Log($"AddFactions({factions.Length})");
            Events.TrackedFactionsUpdated.Invoke();
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
