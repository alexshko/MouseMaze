using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateWay : MonoBehaviour
{
    public Transform AffectedGameObject;

    //need to change this so it will be more generic:
    public Action<Transform> actionToPerformWhenStepped;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Open Gate");
        if (actionToPerformWhenStepped != null && AffectedGameObject != null)
        {
            actionToPerformWhenStepped(AffectedGameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Open Gate by collision");
        if (actionToPerformWhenStepped != null && AffectedGameObject != null)
        {
            actionToPerformWhenStepped(AffectedGameObject);
        }
    }
}
