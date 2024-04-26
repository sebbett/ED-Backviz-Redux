using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static class Events
    {
        public delegate void FactionDataError(string error);
        public delegate void FactionDataReceived(string data);

        public static FactionDataError factionDataError;
        public static FactionDataReceived factionDataReceived;
    }
}
