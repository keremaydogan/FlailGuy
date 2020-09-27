using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

public class MovementPhysics : MonoBehaviour
{
    MovementBasic mb;

    Rigidbody2D rb;

    CapsuleCollider2D bodyCollider;

    [SerializeField] private bool groundCheck;
    public bool fellCheck;
    [SerializeField] private bool isGrounded;

    [SerializeField] private float bCOverCirCenter;
    [SerializeField] private float bCOverCirRad;

    public bool grounded => isGrounded;

    float horInput;
    public float hInput => horInput;

    public LayerMask groundLayers;

    private Vector2 moveDir;
    public Vector2 movingDir => moveDir;

    RaycastHit2D slopeRay;
    float slopeRayLen;
    float slopeAngleRad;
    float slopeAngleDeg;
    public float maxSlopeAngle;

    float brakeForce = 40;
    float brakeCoeff = 1;

    private void Awake()
    {
        moveDir = Vector2.right;

        rb = GetComponent<Rigidbody2D>();
        mb = GetComponent<MovementBasic>();
        bodyCollider = transform.Find("Body").GetComponent<CapsuleCollider2D>();

        VariableCalc();
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

        Debug.DrawLine(transform.position, new Vector3(moveDir.x, moveDir.y) + transform.position, UnityEngine.Color.green);
        Debug.DrawLine(transform.position, new Vector3(0, moveDir.y) + transform.position, UnityEngine.Color.red);
        Debug.DrawLine(transform.position, new Vector3(moveDir.x, 0) + transform.position, UnityEngine.Color.blue);
    }
    
    void VariableCalc()
    {
        bCOverCirRad = bodyCollider.size.x / 2 - 0.01F;
        bCOverCirCenter = (bodyCollider.size.y - bodyCollider.size.x) / 2 + 0.04F;

        slopeRayLen = bodyCollider.size.y / 2 + 0.5F;
    }

    void Slope()
    {
        slopeRay = Physics2D.Raycast(transform.position + (new Vector3(0.2F, 0) * horInput), Vector2.down, slopeRayLen, groundLayers);
        slopeAngleDeg = Vector2.SignedAngle(Vector2.up, slopeRay.normal);
        slopeAngleRad = Mathf.Deg2Rad * slopeAngleDeg;

        if (isGrounded) {
            moveDir.x = horInput * Mathf.Cos(slopeAngleRad);
            moveDir.y = horInput * Mathf.Sin(slopeAngleRad);
        }
        else {
            moveDir = horInput * Vector2.right;
        }
    }

    void AutoBrake()
    {
        if (!mb.enabled) {
            brakeCoeff = 5;
        }
        else {
            brakeCoeff = 1;
        }

        if (groundCheck && horInput == 0 && rb.velocity.x != 0) {
            rb.AddForce(Vector2.left * rb.velocity * rb.mass * brakeForce * brakeCoeff);
        }
    }

    void Grounded()
    {
        groundCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - bCOverCirCenter), bCOverCirRad, groundLayers);

        if(fellCheck == false && rb.velocity.y < 0)
        {
            fellCheck = true;
        }

        //!!!!!!!!!
        //fellcheck is public and used by MovementBasic. You may wanna make it private. 
        //!!!!!!!!!

        if (groundCheck && fellCheck)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 100, 100), slopeAngleDeg + "\nCos (x): " + moveDir.x +"\nSin (y): " + moveDir.y);
    }
}
