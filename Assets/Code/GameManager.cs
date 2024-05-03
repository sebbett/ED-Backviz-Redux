using EDBR;
using EDBR.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameManager
{
    
    public static class Session
    {
        private static List<TrackedFaction> _trackedFactions = new List<TrackedFaction>();
        public static List<TrackedFaction> trackedFactions { get { return _trackedFactions; } }

        public static void addTrackedFaction(_faction newFaction)
        {
            if(!isTrackingFaction(newFaction))
                _trackedFactions.Add(new TrackedFaction(newFaction));

            updateSystemsFromTrackedFactions();
            Events.factionsUpdated.Invoke();
        }
        public static void addTrackedFactions(_faction[] newFactions)
        {
            foreach(_faction newFaction in newFactions)
            {
                if (!isTrackingFaction(newFaction))
                    _trackedFactions.Add(new TrackedFaction(newFaction));
            }

            updateSystemsFromTrackedFactions();
            Events.factionsUpdated.Invoke();
        }
        public static void setTrackedFactions(TrackedFaction[] factions)
        {
            _trackedFactions = new List<TrackedFaction>();
            foreach (TrackedFaction tf in factions)
                _trackedFactions.Add(tf);

            updateSystemsFromTrackedFactions();
            Events.factionsUpdated.Invoke();
        }

        public static void updateSystemsFromTrackedFactions()
        {
            //Request the system data
            List<string> systems = new List<string>();

            //Grab the name of every system the faction has a presence in. If we're
            //not already preparing to query for it, add it to the List<> above
            foreach (TrackedFaction tf in trackedFactions)
            {
                foreach (_faction.FactionPresence fp in tf.faction.faction_presence)
                {
                    string sn = fp.system_name;
                    bool found = false;
                    foreach (_system s in trackedSystems)
                    {
                        if (s.name == sn)
                            found = true;
                    }
                    if (!found)
                        systems.Add(sn);
                }
            }

            executor.StartCoroutine(API.GetSystemData(systems.ToArray()));
        }
        public static CoroutineExecutor executor;

        public static bool isTrackingFaction(_faction faction)
        {
            foreach (TrackedFaction tracked in _trackedFactions)
            {
                if (faction._id == tracked._id)
                    return true;
            }
            return false;
        }

        private static List<_system> _trackedSystems = new List<_system>();
        public static List<_system> trackedSystems {  get { return _trackedSystems; } }

        public static void addTrackedSystems(_system[] newSystems)
        {
            foreach(var ns in newSystems)
            {
                bool found = false;
                foreach(_system sys in _trackedSystems)
                {
                    if (sys.name == ns.name)
                        found = true;
                }

                if (!found)
                    _trackedSystems.Add(ns);
            }
            
            Events.trackedSystemsUpdated.Invoke();
        }

        public static Color colorOfSystem(_system s)
        {
            string cfid = s.controlling_minor_faction_id;

            Color col = Color.white;
            foreach(TrackedFaction tf in trackedFactions)
            {
                if (tf.faction._id == cfid)
                    col = tf.color;
            }

            return col;
        }

        public static void setSelectedSystem(string name)
        {
            foreach(_system s in trackedSystems)
            {
                if (s.name == name)
                    Events.systemSelected.Invoke(s);
            }
        }
        public static void setSelectedFaction(string name)
        {
            foreach(TrackedFaction tf in trackedFactions)
            {
                if(tf.faction.name == name)
                {
                    setSelectedSystem(tf.faction.faction_presence[0].system_name);
                }
            }
        }
    }

    public static class Events
    {
        public static UnityEvent factionsUpdated = new UnityEvent();
        public static UnityEvent<_system> systemSelected = new UnityEvent<_system>();
        public static UnityEvent trackedSystemsUpdated = new UnityEvent();
        public static UnityEvent<string> requestError = new UnityEvent<string>();
        public static UnityEvent<_faction[]> factionDataReceived = new UnityEvent<_faction[]>();
    }
}
