using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EDBR.Data;
using EDBR;
using UnityEngine.Events;
using System.Linq;
using bvData;

public class MapManager : MonoBehaviour
{
    public GameObject node_prefab;
    public Transform node_parent;

    private void Awake()
    {
        bvCore.Events.MapUpdated.AddListener(updateMap);
    }

    private void updateMap()
    {
        //Clear all current nodes
        foreach (Transform child in node_parent)
        {
            Destroy(child.gameObject);
        }

        //Populate new nodes
        foreach (bvSystem s in bvCore.Session.systems)
        {
            GameObject newNode = Instantiate(node_prefab);
            newNode.transform.position = s.position;
            newNode.transform.parent = node_parent;
        }
    }
}
