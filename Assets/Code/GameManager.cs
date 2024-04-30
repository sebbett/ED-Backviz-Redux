using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EDBR;
using EDBR.Data;

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

            if (Events.trackedFactionsUpdated != null)
                Events.trackedFactionsUpdated();
        }
        public static void addTrackedFactions(_faction[] newFactions)
        {
            foreach(_faction newFaction in newFactions)
            {
                if (!isTrackingFaction(newFaction))
                    _trackedFactions.Add(new TrackedFaction(newFaction));
            }

            if (Events.trackedFactionsUpdated != null)
                Events.trackedFactionsUpdated();
        }

        public static void setTrackedFactions(TrackedFaction[] factions)
        {
            _trackedFactions = new List<TrackedFaction>();
            foreach (TrackedFaction tf in factions)
                _trackedFactions.Add(tf);
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
        public delegate void GenericEvent();
        public delegate void RequestError(string error);
        public delegate void FactionDataReceived(_faction[] data);
        public delegate void SystemDataReceived(_system[] data);

        public static GenericEvent trackedFactionsUpdated;

        public static RequestError requestError;
        public static FactionDataReceived factionDataReceived;
        public static SystemDataReceived systemDataReceived;
    }
}
