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
    }

    public static class Session
    {
        public static bvFaction[] factions;
        public static bvSystem[] systems;

        public static void RequestFaction(string[] faction_names) => exe.StartCoroutine(GetFactionData(faction_names, AddFactions));

        public static void AddFactions(bvFaction[] new_factions)
        {
            List<bvFaction.FactionPresence> new_systems = new List<bvFaction.FactionPresence>();

            foreach (bvFaction new_faction in new_factions)
            {
                //If we are not already tracking the faction, track it
                bool found = factions.Any(item => item.id == new_faction.id);
                if (!found)
                    factions.Append(new_faction);

                foreach(bvFaction.FactionPresence presence in new_faction.faction_presence)
                {
                    //If the system is not being tracked, add it to the queue to be queried
                    found = systems.Any(item => item.id == presence.id);
                    if(!found)
                        new_systems.Add(presence);
                }
            }

            //RequestSystemData
        }
    }

    public static CoroutineExecutor exe;
}
