using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build;
using UnityEngine;

public class MovementPhysics : MonoBehaviour
{
    MovementBasic mb;

    Rigidbody2D rb;

    [SerializeField] private bool groundCheck;
    public bool fellCheck;
    [SerializeField] private bool isGrounded;

    public bool grounded => isGrounded;

    float horInput;
    public float hInput => horInput;

    public LayerMask groundLayers;

    RaycastHit2D slopeRay;
    float slopeAngle;

    float brakeForce = 150;
    float brakeCoeff = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mb = GetComponent<MovementBasic>();

        EnemyDetection.TargetPosition += GetPosition;
    }

    private void Update()
    {
        horInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        AutoBrake();

        IsGrounded();

        Slope();
    }
    
    void Slope()
    {
        slopeRay = Physics2D.Raycast(transform.position, Vector3.down, 3, groundLayers);
        Debug.Log(slopeRay.normal + " || degree: " + Vector3.Angle(Vector3.zero, new Vector3(slopeRay.normal.x, slopeRay.normal.y, 0)).ToString());
    }

    Vector3 GetPosition()
    {
        return transform.position;
    }
    void AutoBrake()
    {
        if (!mb.enabled)
        {
            brakeCoeff = 5;
        }
        else
        {
            brakeCoeff = 1;
        }
        if (groundCheck && horInput == 0 && rb.velocity.x != 0)
        {
            rb.AddForce(Vector2.right * rb.velocity * brakeForce * brakeCoeff * -1);
        }
    }
    void IsGrounded()
    {
        groundCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 1.7F), 0.69F, groundLayers);

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
}
