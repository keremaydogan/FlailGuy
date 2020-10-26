using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MovementPhysics : MonoBehaviour
{
    MovementBasic mb;

    Rigidbody2D rb;

    CapsuleCollider2D bodyCol;

    [SerializeField] private bool groundCheck;
    public bool fellCheck;
    [SerializeField] bool slopeCheck;
    [SerializeField] private bool isGrounded;

    [SerializeField] private float bCOverCirCenter;
    [SerializeField] private float bCOverCirRad;

    public bool grounded => isGrounded;

    float horInput;
    public float hInput => horInput;

    public LayerMask groundLayers;

    private Vector2 moveDir;
    public Vector2 movingDir => moveDir;

    [SerializeField] float facingDir = 1;
    public float faceDir => facingDir;

    RaycastHit2D slopeRay;
    float slopeRayLen;
    float slopeRayCenter;
    float slopeAngleRad;
    float slopeAngleDeg;
    public float maxSlopeAngle;

    float brakeForce = 60;
    float brakeCoeff = 1;

    [SerializeField] bool landingDetect = false;
    [SerializeField] bool landingTrigger;

    private void Awake()
    {
        moveDir = Vector2.right;

        rb = GetComponent<Rigidbody2D>();
        mb = GetComponent<MovementBasic>();
        bodyCol = transform.Find("Body").GetComponent<CapsuleCollider2D>();

        AwakeCalc();
    }

    private void Update()
    {
        horInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        AutoBrake();

        Grounded();

        Slope();

        LandingStabilization();

        FacingDir();

        Debug.DrawLine(transform.position, new Vector3(moveDir.x, moveDir.y) + transform.position, UnityEngine.Color.green);
        Debug.DrawLine(transform.position, new Vector3(0, moveDir.y) + transform.position, UnityEngine.Color.red);
        Debug.DrawLine(transform.position, new Vector3(moveDir.x, 0) + transform.position, UnityEngine.Color.blue);
        Debug.DrawLine(transform.position + new Vector3(slopeRayCenter * horInput, 0), new Vector3(slopeRayCenter * horInput, -slopeRayLen) + transform.position, UnityEngine.Color.magenta);
    }

    void AwakeCalc()
    {
        bCOverCirRad = bodyCol.size.x / 2 - 0.01F;
        bCOverCirCenter = (bodyCol.size.y - bodyCol.size.x) / 2 + 0.04F;

        slopeRayLen = bodyCol.size.y / 2 + 0.6F;
        slopeRayCenter = 0.2F;

        groundCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - bCOverCirCenter), bCOverCirRad, groundLayers);
    }

    void Slope()
    {
        slopeRay = Physics2D.Raycast(transform.position + new Vector3(slopeRayCenter * horInput, 0), Vector2.down, slopeRayLen, groundLayers);
        slopeAngleDeg = Vector2.SignedAngle(Vector2.up, slopeRay.normal);
        slopeAngleRad = Mathf.Deg2Rad * slopeAngleDeg;

        if (isGrounded) {
            moveDir.x = horInput * Mathf.Cos(slopeAngleRad);
            moveDir.y = horInput * Mathf.Sin(slopeAngleRad);
        }
        else {
            moveDir = horInput * Vector2.right;
        }

        if (groundCheck)
        {
            if (slopeAngleDeg * facingDir > maxSlopeAngle)
            {
                slopeCheck = false;
            }
            else
            {
                slopeCheck = true;
            }
        }
    }

    void AutoBrake()
    {
        if (!mb.enabled) {
            brakeCoeff = 1.5F;
        }
        else {
            brakeCoeff = 1;
        }

        if (isGrounded && horInput == 0 && rb.velocity.x != 0) {
            rb.AddForce(-rb.velocity * brakeForce * rb.mass * brakeCoeff);
        }

        //if(horInput == 0 && rb.velocity.magnitude < 0.5)
        //{
        //    rb.velocity = Vector2.zero;
        //}
    }

    //this looks weird with no animation but maybe it'll be okay with animation
    //if you fall without jump it doesn't work
    void LandingStabilization()
    {
        if (landingDetect)
        {
            landingTrigger = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - bCOverCirCenter - 1), 0.2F, groundLayers);
        }
        if (landingTrigger)
        {
            landingDetect = false;
            landingTrigger = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    void Grounded()
    {
        groundCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - bCOverCirCenter), bCOverCirRad, groundLayers);

        if(fellCheck == false && rb.velocity.y < 0)
        {
            fellCheck = true;
            landingDetect = true;
        }

        //!!!!!!!!!
        //fellcheck is public and used by MovementBasic. You may wanna make it private. 
        //!!!!!!!!!

        if (groundCheck && fellCheck && slopeCheck)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void FacingDir()
    {
        if(horInput > 0)
        {
            facingDir = 1;
        }
        else if(horInput < 0)
        {
            facingDir = -1;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 100, 100), slopeAngleDeg + "\nCos (x): " + moveDir.x +"\nSin (y): " + moveDir.y);
    }
}
