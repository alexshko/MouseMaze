using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateWay : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Open Gate");
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Open Gate by collision");
    }
}
