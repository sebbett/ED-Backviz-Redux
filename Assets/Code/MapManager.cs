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
        GameManager.Events.trackedSystemsUpdated.AddListener(updateSystems);
    }

    private void updateSystems()
    {
        //Clear all current nodes
        foreach (Transform child in node_parent)
        {
            Destroy(child.gameObject);
        }

        //Populate new nodes
        foreach (_system s in GameManager.Session.trackedSystems)
        {
            Vector3 pos = new Vector3(s.x, s.y, s.z);
            GameObject newNode = Instantiate(node_prefab, pos, Quaternion.identity);
            newNode.GetComponentInChildren<MeshRenderer>().material.color = GameManager.Session.colorOfSystem(s);
            newNode.GetComponent<Node>().system_name = s.name;
            newNode.transform.parent = node_parent;
        }
    }
}
