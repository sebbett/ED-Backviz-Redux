using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    public UnityEvent onClick = new UnityEvent();
    private void OnMouseUpAsButton()
    {
        onClick.Invoke();
    }
}
