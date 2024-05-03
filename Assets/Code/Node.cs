using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public string system_name;

    private void OnMouseUpAsButton()
    {
        GameManager.Session.setSelectedSystem(system_name);
    }
}
