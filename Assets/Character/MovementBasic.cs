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

    Transform skin;
    Transform weapon;


    void Start()
    {
        mp = GetComponent<MovementPhysics>();
        rb = GetComponent<Rigidbody2D>();

        skin = transform.Find("Skin");
        weapon = transform.Find("Weapon");
        weapon.gameObject.SetActive(false);
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
    }

    void Inputs()
    {
        horInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
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
        if (isGrounded && !Input.GetButton("Down"))
        {
            rb.AddForce(Vector2.up * jumpForce * rb.mass);
        }
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
