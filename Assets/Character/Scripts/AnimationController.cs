using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    MovementPhysics mp;
    [SerializeField] private bool isGrounded;
    Animator anim;

    float horInput;
    public float hInput => horInput;

    [SerializeField] float currentDir;
    [SerializeField] float attackDir;
    [SerializeField] float facingDir = 1;

    public bool detectAtkDir;
    public bool detectCurDir;

    Transform skin;
    Transform weapon;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        mp = GetComponent<MovementPhysics>();

        skin = transform.Find("Skin");
        weapon = transform.Find("Weapon");
    }

    private void Update()
    {
        horInput = mp.hInput;

        FacingDir();

        AttackTrigger();

        Crouch();
    }

    private void FixedUpdate()
    {
        isGrounded = mp.grounded;
    }

    void Crouch()
    {
        if (Input.GetButton("Down"))
        {
            anim.SetBool("isCrouching", true);
        }
        else
        {
            anim.SetBool("isCrouching", false);
        }
    }
    
    void AttackTrigger()
    {
        if (Input.GetButtonDown("Attack"))
        {
            anim.SetTrigger("Attack");
            anim.ResetTrigger("OppositeAtk");
        }
        if (detectCurDir)
        {
            currentDir = facingDir;
            attackDir = facingDir;
        }
        if (detectAtkDir)
        {
            attackDir = facingDir;
        }
        if(currentDir != attackDir)
        {
            anim.SetTrigger("OppositeAtk");
            Turn();
        }
    }

    void FacingDir()
    {
        if(horInput == 1)
        {
            facingDir = 1;
        }
        else if (horInput == -1)
        {
            facingDir = -1;
        }
    }

    void Turn()
    {
        if (attackDir == 1)
        {
            skin.localScale = Vector3.one;
            weapon.localScale = Vector3.one;
        }
        else if (attackDir == -1)
        {
            skin.localScale = new Vector3(-1, 1, 1);
            weapon.localScale = new Vector3(-1, 1, 1);
        }
        attackDir = currentDir;
    }
}
