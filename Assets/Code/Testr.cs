using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EDBR;
using System;

public class Testr : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Events.factionDataReceived += factionDataReceived;
    }

    private void factionDataReceived(string data)
    {
        Debug.Log(data);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Debug.Log("BACKSPACE");
            StartCoroutine(API.GetFactionData("The Archon Horde"));
        }
    }
}
