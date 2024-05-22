using bvData;
using bvUtils;
using bvAPI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Test : MonoBehaviour
{
    public bvSystem[] systems;

    private void Awake()
    {
        bvCore.Events.MapUpdated.AddListener(MapUpdated);
    }

    private void MapUpdated()
    {
        systems = bvCore.Session.systems;
    }
}
