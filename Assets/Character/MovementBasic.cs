using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class MovementBasic : MonoBehaviour
{
    Rigidbody2D rb;

    MovementPhysics mp;

    [SerializeField] private bool isGrounded;

    [SerializeField] float velocityX;

    private float horInput;

    public float maxVelocity;
    public float walkForce;
    public float walkAirConst;
    private float walkAirCoeff;
    public float crouchConst; 
    public float runConst; //Constant 
    private float runCoeff;

    public float jumpForce;
    //[SerializeField] private float jumpForceMax;
    [SerializeField] bool jumpInput;

    Transform skin;
    Transform weapon;


    private void Awake()
    {
        mp = GetComponent<MovementPhysics>();
        rb = GetComponent<Rigidbody2D>();

        skin = transform.Find("Skin");
        weapon = transform.Find("Weapon");

        //jumpForce = jumpForceMax;
    }


    void Update()
    {
        Inputs();

        Turn();
    }

    private void FixedUpdate()
    {
        isGrounded = mp.grounded;

        velocityX = rb.velocity.x;
        
        HorizontalMove();

        Jump();
    }

    void Inputs()
    {
        horInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButton("Jump") && !Input.GetButton("Down") && isGrounded)
        {
            jumpInput = true;
        }
        //else if (Input.GetButtonUp("Jump") || rb.velocity.y < 0)
        //{
        //    jumpInput = false;
        //    jumpForce = jumpForceMax;
        //}
    }

    void HorizontalMove()
    {
        if(rb.velocity.x * horInput <= maxVelocity * runCoeff)
        {
            rb.AddForce(Vector2.right  * walkForce * walkAirCoeff  * horInput * Time.deltaTime * rb.mass);
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
        if (jumpInput)
        {
            rb.AddForce(Vector2.up * jumpForce * rb.mass);
            jumpInput = false;
        }

        //if (jumpInput)
        //{
        //    jumpForce = jumpForce / 1.2F;
        //}

        //if(jumpForce < 50)
        //{
        //    jumpForce = 0;
        //}
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
