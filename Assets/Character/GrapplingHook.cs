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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(anchor.localPosition, anchor.localPosition + throwingDir);
    }

    private void Update()
    {
        Inputs();

        SlowDown();

        horInput = Input.GetAxisRaw("Horizontal");


    }

    private void FixedUpdate()
    {
        Throw();
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
            anchorRb.velocity = Vector2.zero;
            anchorRb.AddForce(1000 * throwingDir);
        }

    }

    void SlowDown()
    {
        if (throwHook)
        {
            Time.timeScale = 0.05F;
            Time.fixedDeltaTime = 0.001F;
        }
        else if (!throwHook)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02F;
        }
    }

    void Throw()
    {
        if (throwHook)
        {
            if(horInput == 1)
            {
                throwingDir = Quaternion.AngleAxis(2880 * Time.deltaTime, Vector3.back) * throwingDir;
            }
            else if(horInput == -1)
            {
                throwingDir = Quaternion.AngleAxis(-2880 * Time.deltaTime, Vector3.back) * throwingDir;
            }
        }
    }
}
