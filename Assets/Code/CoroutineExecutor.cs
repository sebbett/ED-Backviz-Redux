using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineExecutor : MonoBehaviour
{
    //this class literally only exists to
    //allow the execution of StartCoroutine
    //from the GameManager, because it requires
    //a MonoBehaviour to execute the thing.

    private void Awake()
    {
        //GameManager.Session.executor = this;
        bvCore.exe = this;
    }

    //fucking trash.
}
