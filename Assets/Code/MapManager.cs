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
        GameManager.Events.systemsUpdated.AddListener(updateSystems);
    }

    private void updateSystems()
    {
        //Clear all current nodes
        foreach (Transform child in node_parent)
        {
            Destroy(child.gameObject);
        }

        //Populate new nodes
        foreach (system_details s in GameManager.Session.trackedSystems)
        {
            Vector3 pos = new Vector3((float)s.x, (float)s.y, (float)s.z);
            GameObject newNode = Instantiate(node_prefab, pos, Quaternion.identity);
            newNode.GetComponentInChildren<MeshRenderer>().material.color = GameManager.Session.colorOfSystem(s);
            newNode.GetComponent<Node>().onClick.AddListener(() => GameManager.Session.setSelectedSystem(s._id));

            if(s.conflicts.Count > 0) newNode.transform.Find("conflict_particles").gameObject.SetActive(true);

            newNode.transform.parent = node_parent;
        }
    }
}
