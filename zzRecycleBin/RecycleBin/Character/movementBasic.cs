using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class movementBasic : MonoBehaviour
{
    Rigidbody2D rb;

    private Vector2 walkDir;

    public float walkFrc;
    public float walkSpdSlw;
    public float walkSpdRun;
    public float maxVelocity;

    public float jumpPow;

    public Vector2 walkDirection;

    public bool autobrake;

    public Vector3 facingDirection;

    [SerializeField]
    private float horMomentum;
    [SerializeField]
    private bool onGround;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            onGround = true;
        }
        walkSpdSlw = 1;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            onGround = false;
        }
        walkSpdSlw = 0.8F;
    }
    void Start()
    {
        facingDirection = transform.localScale;

        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        maxVelocity = 15;

        walkFrc = 1200;

        jumpPow = 500;

        autobrake = true;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        horMomentum = rb.velocity.x * rb.mass;

        if (onGround)
        {
            Jump();
        }

        VerMovement();

        if (Input.GetKey(KeyCode.LeftShift) && onGround)
        {
            walkSpdRun = 1.5F;
        }
        else
        {
            walkSpdRun = 1;
        }

        if (autobrake)
        {
            Autobrake();
        }

    }

    void Flip(bool facingRight)
    {
        if (facingRight)
        {
            facingDirection.x = 2;
        }
        else
        {
            facingDirection.x = -2;
        }
        transform.localScale = facingDirection;
        Debug.Log(facingDirection);
    }

    void VerMovement()
    {
        if (Input.GetKey(KeyCode.A) && horMomentum > -(maxVelocity * rb.mass) * walkSpdRun)
        {
            rb.AddForce(Vector2.left * walkFrc * walkSpdSlw * walkSpdRun * Time.deltaTime);
            Flip(false);
        }
        if (Input.GetKey(KeyCode.D) && horMomentum < maxVelocity * rb.mass * walkSpdRun)
        {
            rb.AddForce(Vector2.right * walkFrc * walkSpdSlw * walkSpdRun * Time.deltaTime);
            Flip(true);
        }

    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddForce(Vector2.up * jumpPow);
        }
    }

    void Autobrake()
    {

        //if (horMomentum > 0 && !Input.GetKey(KeyCode.D))
        //{
        //    rb.AddForce(Vector2.left*10000 * Time.deltaTime);
        //}
        //if (horMomentum < 0 && !Input.GetKey(KeyCode.A))
        //{
        //    rb.AddForce(Vector2.right*10000*Time.deltaTime);
        //}

        //if (horMomentum > 0 && !Input.GetKey(KeyCode.D))
        //{
        //    rb.AddForce(Vector2.left * maxVelocity * 100 * Time.deltaTime);
        //}
        //if (horMomentum < 0 && !Input.GetKey(KeyCode.A))
        //{
        //    rb.AddForce(Vector2.right * maxVelocity * 100 * Time.deltaTime);
        //}


        if (horMomentum > 0 && !Input.GetKey(KeyCode.D) && onGround)
        {
            transform.Translate(new Vector3(0, 0.0001F, 0));
            rb.velocity = Vector3.zero;
        }
        if (horMomentum < 0 && !Input.GetKey(KeyCode.A) && onGround)
        {
            transform.Translate(new Vector3(0, 0.0001F, 0));
            rb.velocity = Vector3.zero;
        }
    }

}
