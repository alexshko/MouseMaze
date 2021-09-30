using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FloatingRock : MonoBehaviour
{
    public float speedOfMove;
    public bool isMooving { get; set; }

    private Rigidbody rb;


    protected abstract void ApplyForce();

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void Update()
    {
        if (isMooving)
        {
            ApplyForce();
        }
    }

}
