using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EDBR.Data;
using EDBR;
using UnityEngine.Events;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public GameObject node_prefab;
    public Transform node_parent;

    private void Awake()
    {
        GameManager.Events.trackedFactionsUpdated.AddListener(trackedFactionsUpdated);
        GameManager.Events.trackedSystemsUpdated.AddListener(updateSystems);
    }

    private void updateSystems()
    {
        Debug.Log("MapManager.trackedFactionsUpdated");
        //Clear all current nodes
        foreach (Transform child in node_parent)
        {
            Destroy(child.gameObject);
        }

        foreach (_system s in GameManager.Session.trackedSystems)
        {
            Vector3 pos = new Vector3(s.x, s.y, s.z);
            GameObject newNode = Instantiate(node_prefab, pos, Quaternion.identity);
            newNode.GetComponentInChildren<MeshRenderer>().material.color = GameManager.Session.colorOfSystem(s);
            newNode.GetComponent<Node>().system_name = s.name;
            newNode.transform.parent = node_parent;
        }
    }

    private void trackedFactionsUpdated()
    {
        Debug.Log("MapManager.trackedFactionsUpdated");
        //Request the system data
        List<string> systems = new List<string>();
        foreach(TrackedFaction tf in GameManager.Session.trackedFactions)
        {
            foreach(_faction.FactionPresence fp in tf.faction.faction_presence)
            {
                string sn = fp.system_name;
                bool found = false;
                foreach (_system s in GameManager.Session.trackedSystems)
                {
                    if (s.name == sn)
                        found = true;
                }
                if (!found)
                    systems.Add(sn);
            }
        }
        string[] request = systems.ToArray();
        StartCoroutine(API.GetSystemData(request));
    }
}
