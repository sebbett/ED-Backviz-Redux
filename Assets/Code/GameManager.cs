using EDBR;
using EDBR.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameManager
{
    
    public static class Session
    {
        public static CoroutineExecutor executor;
        private static List<TrackedFaction> _trackedFactions = new List<TrackedFaction>();
        public static List<TrackedFaction> trackedFactions { get { return _trackedFactions; } }

        private static List<system_details> _trackedSystems = new List<system_details>();
        public static List<system_details> trackedSystems { get { return _trackedSystems; } }

        public static void addTrackedFactions(_faction[] newFactions)
        {
            foreach(_faction newFaction in newFactions)
            {
                if (!isTrackingFaction(newFaction))
                    _trackedFactions.Add(new TrackedFaction(newFaction));
            }

            Events.factionsUpdated.Invoke();
            updateSystemsFromTrackedFactions();
        }

        public static void updateSystemsFromTrackedFactions()
        {
            foreach (TrackedFaction tf in trackedFactions)
            {
                foreach (_faction.FactionPresence fp in tf.faction.faction_presence)
                {
                    system_details sd = fp.details;

                    bool found = false;
                    foreach (system_details ts in _trackedSystems)
                    {
                        if (sd._id == ts._id)
                            found = true;
                    }
                    if (!found)
                        _trackedSystems.Add(sd);
                }
            }

            Events.systemsUpdated.Invoke();

            List<string> systemsNeedingInfluence = new List<string>();
            foreach (system_details sd in _trackedSystems)
            {
                bool needinf = false;
                foreach(system_details.Faction sf in sd.factions)
                {
                    if (sf.influence <= 0.0f)
                        needinf = true;
                }

                if (needinf)
                    systemsNeedingInfluence.Add(sd.name);
            }

            executor.StartCoroutine(API.GetSystemData(systemsNeedingInfluence.ToArray()));
        }
        public static void addTrackedSystems(system_details[] newSystems)
        {
            foreach(var ns in newSystems)
            {
                bool found = false;
                foreach(system_details sys in _trackedSystems)
                {
                    if (sys.name == ns.name)
                        found = true;
                }

                if (!found)
                    _trackedSystems.Add(ns);
            }
            
            Events.systemsUpdated.Invoke();
        }

        public static void updateSystemFactionInfluence(_system[] systems)
        {
            foreach(_system i in systems)
            {
                foreach(system_details j in trackedSystems)
                {
                    if (i.id == j._id)
                    {
                        foreach(system_details.Faction sf in j.factions)
                        {
                            foreach(_system.Faction f in i.factions)
                            {
                                if (f.faction_id == sf.faction_id)
                                {
                                    sf.influence = f.faction_details.faction_presence.influence;
                                    sf.state = f.faction_details.faction_presence.state;
                                }
                            }
                        }
                    }
                }
            }

            Events.systemsUpdated.Invoke();
        }

        public static void setSelectedSystem(string id)
        {
            foreach(system_details s in trackedSystems)
            {
                if (s._id == id)
                    Events.systemSelected.Invoke(s);
            }
        }
        public static void setSelectedFaction(string name)
        {
            foreach(TrackedFaction tf in trackedFactions)
            {
                if(tf.faction.name == name)
                {
                    setSelectedSystem(tf.faction.faction_presence[0].system_id);
                }
            }
        }

        public static Color colorOfSystem(system_details s)
        {
            string cfid = s.controlling_minor_faction_id;

            Color col = Color.white;
            foreach (TrackedFaction tf in trackedFactions)
            {
                if (tf.faction._id == cfid)
                    col = tf.color;
            }

            return col;
        }
        public static bool isTrackingFaction(_faction faction)
        {
            foreach (TrackedFaction tracked in _trackedFactions)
            {
                if (faction._id == tracked._id)
                    return true;
            }
            return false;
        }
    }

    public static class Events
    {
        public static UnityEvent<string> statusUpdated = new UnityEvent<string>();
        public static UnityEvent factionsUpdated = new UnityEvent();
        public static UnityEvent<system_details> systemSelected = new UnityEvent<system_details>();
        public static UnityEvent systemsUpdated = new UnityEvent();
        public static UnityEvent<string> requestError = new UnityEvent<string>();
        public static UnityEvent<_faction[]> factionDataReceived = new UnityEvent<_faction[]>();
    }
}
