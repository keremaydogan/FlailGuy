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
    [SerializeField] private bool fellCheck;
    [SerializeField] private bool isGrounded;

    public bool grounded => isGrounded;

    float horInput;
    public float hInput => horInput;

    public LayerMask layers;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mb = GetComponent<MovementBasic>();

        fellCheck = true;

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
    }

    Vector3 GetPosition()
    {
        return transform.localPosition;
    }

    void AutoBrake()
    {
        if (groundCheck && horInput == 0 && rb.velocity.x != 0)
        {
            rb.AddForce(Vector2.right * rb.velocity * 150 * -1);
        }
    }
    void IsGrounded()
    {
        groundCheck = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 1.7F), 0.69F, layers);

        if(groundCheck && fellCheck)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
