using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EDBR.Data;

public class MapManager : MonoBehaviour
{
    public GameObject node_prefab;
    public Transform node_parent;

    private void Awake()
    {
        GameManager.Events.trackedFactionsUpdated += trackedFactionsUpdated;
    }

    private void trackedFactionsUpdated()
    {
        //Clear all current nodes
        foreach(Transform child in node_parent)
        {
            Destroy(child);
        }

        //Request the system data
        List<string> systems = new List<string>();
        foreach(TrackedFaction tf in GameManager.Session.trackedFactions)
        {
            foreach(_faction.FactionPresence fp in tf.faction.faction_presence)
            {
                systems.Add(fp.system_name);
            }
        }
    }
}
