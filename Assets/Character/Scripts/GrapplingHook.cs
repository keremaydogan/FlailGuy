using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    Transform grapHook;
    Transform anchor;
    Rigidbody2D anchorRb;

    Vector3 throwingDir;

    float throwingForce;

    float horInput;

    bool throwHook;
    private void Awake()
    {
        grapHook = transform.Find("GrapplingHook");
        anchor = grapHook.Find("Anchor");
        anchorRb = anchor.GetComponent<Rigidbody2D>();

        throwingDir = new Vector2(0, 1);
    }

    private void Update()
    {
        Inputs();
    }

    private void FixedUpdate()
    {
    }

    void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            throwHook = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            throwHook = false;
        }

    }
}
