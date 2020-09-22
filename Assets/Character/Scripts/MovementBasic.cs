using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using TMPro;
using UnityEngine;

public class MovementBasic : MonoBehaviour
{
    Rigidbody2D rb;

    MovementPhysics mp;

    [SerializeField] private bool isGrounded;

    [SerializeField] float velocityX;
    [SerializeField] float velocityY;

    private float horInput;

    public float maxVelocity;
    public float walkForce;
    public float walkAirConst;
    private float walkAirCoeff;
    public float crouchConst; 
    public float runConst; //Constant 
    private float runCoeff;
    private Vector2 moveDir;


    public float jumpForce;
    [SerializeField] bool jumpInput;

    Transform skin;
    Transform weapon;

    private void Awake()
    {
        mp = GetComponent<MovementPhysics>();
        rb = GetComponent<Rigidbody2D>();

        skin = transform.Find("Skin");
        weapon = transform.Find("Weapon");
    }

    void Update()
    {
        Inputs();

        Turn();
    }

    private void FixedUpdate()
    {
        isGrounded = mp.grounded;
        moveDir = mp.movingDir;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        HorizontalMove();

        Debug.DrawLine(transform.position, new Vector3(moveDir.x, moveDir.y) + transform.position, UnityEngine.Color.red);
    }
    void Inputs()
    {
        horInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && !Input.GetButton("Down") && isGrounded)
        {
            Jump();
        }
    }

    void HorizontalMove()
    {
        if(rb.velocity.x * horInput <= maxVelocity * runCoeff)
        {
            rb.AddForce(moveDir  * walkForce * walkAirCoeff * Time.deltaTime * rb.mass);
        }

        if (isGrounded && Input.GetButton("Down"))
        {
            runCoeff = crouchConst;
        }
        else if (isGrounded && Input.GetButton("Run"))
        {
            runCoeff = runConst;
        }
        else
        {
            runCoeff = 1;
        }

        if (!isGrounded)
        {
            walkAirCoeff = walkAirConst;
        }
        else
        {
            walkAirCoeff = 1;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce * rb.mass);
        mp.fellCheck = false;
    }
    void Turn()
    {
        if (horInput == 1)
        {
            skin.localScale = Vector3.one;
            weapon.localScale = Vector3.one;
        }
        else if (horInput == -1)
        {
            skin.localScale = new Vector3(-1, 1, 1);
            weapon.localScale = new Vector3(-1, 1, 1);
        }
    }
}
