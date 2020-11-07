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

    bool jumpAllowed;
    public float jumpHeight;
    [SerializeField] bool jumpInput = false;
    [SerializeField] float jumpVelo;

    public float jumpForceMax;
    [SerializeField] float jumpForceLeveled;

    bool jumpDown;
    bool jumpDownTrigger;
    public bool jumpD => jumpDown;

    Transform skin;
    Transform weapon;

    private void Awake()
    {
        mp = GetComponent<MovementPhysics>();
        rb = GetComponent<Rigidbody2D>();

        skin = transform.Find("Skin");
        weapon = transform.Find("Weapon");

        AwakeCalc();
    }

    void AwakeCalc()
    {
        jumpVelo = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale * jumpHeight);
        jumpForceLeveled = jumpForceMax;
    }

    void Update()
    {
        Inputs();

        Turn();

        SlopeCoeff();

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.1F;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = mp.grounded;
        moveDir = mp.movingDir;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        HorizontalMove();

        Jump();

        JumpDown();
    }

    void Inputs()
    {
        horInput = Input.GetAxisRaw("Horizontal");

        if (jumpAllowed && Input.GetButtonDown("Jump") && !Input.GetButton("Down") && isGrounded)
        {
            //rb.velocity = new Vector2(rb.velocity.x, jumpVelo);

            //jumpForceDynm = jumpForce;

            rb.velocity = new Vector2(rb.velocity.x, 0);

            jumpForceLeveled = jumpForceMax;

            jumpInput = true;
            jumpAllowed = false;
            mp.fellCheck = false;
        }

        if(isGrounded && Input.GetButton("Down") && Input.GetButtonDown("Jump"))
        {
            jumpDownTrigger = true;
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

        if (isGrounded && Input.GetButton("Down")) {
            runCoeff = crouchConst;
        }
        else if (isGrounded && Input.GetButton("Run")) {
            runCoeff = runConst;
        }
        else {
            runCoeff = 1;
        }

        if (!isGrounded) {
            walkAirCoeff = walkAirConst;
        }
        else {
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

                //rb.gravityScale = 1.5F;
            }

            jumpForceLeveled = jumpForceLeveled / 3 * 2;
            rb.AddForce(Vector2.up * jumpForceLeveled * -Physics2D.gravity);

            if(jumpForceLeveled < 20)
            {
                jumpForceLeveled = 0;
            }
        }
    }

    void JumpDown()
    {
        if (jumpDown)
        {
            jumpDown = false;
        }
        if (jumpDownTrigger)
        {
            jumpDownTrigger = false;
            jumpDown = true;
        }
    }

    void Turn()
    {
        if (mp.faceDir == 1)
        {
            skin.localScale = Vector3.one;
            weapon.localScale = Vector3.one;
        }
        else if (mp.faceDir == -1)
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
