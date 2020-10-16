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
    private float slopeCoeff;
    private Vector2 moveDir;

    //public float jumpHeight;
    public float jumpForce;
    [SerializeField] bool jumpInput = false;
    bool jumpAllowed;

    Transform skin;
    Transform weapon;
    private void Awake()
    {
        mp = GetComponent<MovementPhysics>();
        rb = GetComponent<Rigidbody2D>();

        skin = transform.Find("Skin");
        weapon = transform.Find("Weapon");

        //VariableCalc();
    }

    void Update()
    {
        Inputs();

        Turn();

        SlopeCoeff();
    }

    private void FixedUpdate()
    {
        isGrounded = mp.grounded;
        moveDir = mp.movingDir;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        HorizontalMove();

        Jump();
    }

    //void VariableCalc()
    //{
    //    jumpForce = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
    //    jumpForce = rb.mass * Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y)) / Time.fixedDeltaTime;
    //    Debug.Log("jf " + jumpForce + " " + Time.deltaTime + " " + Time.fixedDeltaTime);
    //}

    void Inputs()
    {
        horInput = Input.GetAxisRaw("Horizontal");

        if (jumpAllowed && Input.GetButtonDown("Jump") && !Input.GetButton("Down") && isGrounded)
        {
            jumpInput = true;
            jumpAllowed = false;
            mp.fellCheck = false;
        }
    }

    void SlopeCoeff()
    {
        slopeCoeff = 1 + -Mathf.Sign(moveDir.y) * (1 - Mathf.Abs(moveDir.x));
    }

    void HorizontalMove()
    {
        if(rb.velocity.x * horInput <= maxVelocity * runCoeff * slopeCoeff)
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
        if(isGrounded && !jumpAllowed && !Input.GetButtonUp("Jump"))
        {
            jumpAllowed = true;
        }

        if (jumpInput)
        {
            if (mp.fellCheck || !Input.GetButton("Jump"))
            {
                jumpInput = false;
            }

            //READ THERE READ THERE READ THERE READ THERE 
            //button system is okay now but you need to find about leveling system

            rb.AddForce(Vector2.up * jumpForce * rb.mass);

            //mp.fellCheck = false;
            //rb.velocity = new Vector2(rb.velocity.x, 0);
            //rb.AddForce(Vector2.up * jumpForce * rb.mass);
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

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 100, 100, 125), "Sign(y): " + Mathf.Sign(moveDir.y) + "\nMaxVelocity: " + maxVelocity * runCoeff * slopeCoeff + "\nVelocity: " + velocityX);
    }
}
