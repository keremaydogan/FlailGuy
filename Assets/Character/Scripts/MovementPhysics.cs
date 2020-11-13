using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class MovementPhysics : MonoBehaviour
{
    MovementBasic mb;

    Rigidbody2D rb;

    GameObject body;
    CapsuleCollider2D bodyCol;

    private bool groundCheck;
    public bool fellCheck;
    bool slopeCheck;
    [SerializeField] private bool isGrounded;

    [SerializeField] private float bCOverCirCenter;
    [SerializeField] private float bCOverCirRad;

    public bool grounded => isGrounded;

    float horInput;
    public float hInput => horInput;

    public LayerMask groundLayers;

    private Vector2 moveDir;
    public Vector2 movingDir => moveDir;

    float facingDir = 1;
    public float faceDir => facingDir;

    RaycastHit2D slopeRay;
    float slopeRayLen;
    float slopeRayX;
    float slopeRayY;
    float slopeAngleRad;
    float slopeAngleDeg;
    public float maxSlopeAngle;

    float brakeForce = 40;
    float brakeCoeff = 1;
    bool brakeAngle;

    [SerializeField] bool landingDetect = false;
    [SerializeField] bool landingTrigger;

    [SerializeField] bool onPlatform;
    public LayerMask platformLayer;
    float jumpDPlatformY;
    float jumpDOnPlatY;

    private void Awake()
    {
        moveDir = Vector2.right;

        rb = GetComponent<Rigidbody2D>();
        mb = GetComponent<MovementBasic>();
        body = transform.GetChild(0).gameObject;
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

        JumpDown();

        Debug.DrawLine(transform.position, new Vector3(moveDir.x, moveDir.y) + transform.position, UnityEngine.Color.green);
        Debug.DrawLine(transform.position, new Vector3(0, moveDir.y) + transform.position, UnityEngine.Color.red);
        Debug.DrawLine(transform.position, new Vector3(moveDir.x, 0) + transform.position, UnityEngine.Color.blue);
        //Debug.DrawLine(transform.position + new Vector3(slopeRayX * horInput, 0), new Vector3(slopeRayX * horInput, -slopeRayLen) + transform.position, UnityEngine.Color.magenta);
    }

    void AwakeCalc()
    {
        bCOverCirRad = bodyCol.size.x / 2 - 0.01F;
        bCOverCirCenter = (bodyCol.size.y - bodyCol.size.x) / 2 + 0.04F;

        slopeRayLen = 0.6F;
        slopeRayY = -bodyCol.size.y / 2 + 0.1F;

        //slopeRayLen = 0.6F;
        //slopeRayX = 0.2F;

        jumpDOnPlatY = bodyCol.size.y / 2 + 0.01F;
    }

    void Slope()
    {
        slopeRayX = 0.2F * horInput;

        slopeRay = Physics2D.Raycast(transform.position + new Vector3(slopeRayX, slopeRayY), Vector2.down, slopeRayLen, groundLayers);

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
        else
        {
            slopeCheck = false;
        }
    }

    void AutoBrake()
    {
        if (!mb.enabled)
        {
            brakeCoeff = 1.5F;
        }
        else
        {
            brakeCoeff = 1;
        }

        if (isGrounded && horInput == 0 && rb.velocity.x != 0 && Vector2.Angle(Vector2.down, rb.velocity) > 10)
        {
            rb.AddForce(-rb.velocity * brakeForce * rb.mass * brakeCoeff);
        }
    }

    void JumpDown()
    {
        onPlatform = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - jumpDOnPlatY), 0.5F, platformLayer);

        if (mb.jumpD && onPlatform)
        {
            body.layer = 13;
            jumpDPlatformY = transform.position.y - jumpDOnPlatY;
        }
        if (transform.position.y > jumpDPlatformY + jumpDOnPlatY + 0.1F || transform.position.y < jumpDPlatformY)
        {
            body.layer = 10;
        }
    }

    //this looks weird with no animation but maybe it'll be okay with animation
    //if you fall without jump it doesn't workc
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
        groundCheck = slopeRay;

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
        GUI.Label(new Rect(25, 25, 100, 100), slopeAngleDeg + "\nCos (x): " + moveDir.x + "\nSin (y): " + moveDir.y);
        GUI.Label(new Rect(130, 25, 100, 100), (jumpDPlatformY + jumpDOnPlatY + 0.1F) + "\n" + jumpDPlatformY + "\n" + transform.position.y);
    }
}
