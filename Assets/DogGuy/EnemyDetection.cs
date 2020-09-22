using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    RaycastHit2D visibleZone;
    RaycastHit2D memoryZone;
    public float vzRange;
    public float mzRange;
    public LayerMask vzLayers;
    public LayerMask mzLayers;

    Vector3 targetPos;
    
    [SerializeField] bool targetVisible;
    bool onTheWatch;
    [SerializeField] bool targetAround;
    [SerializeField] bool targetDetected;

    Collider2D[] contacts;

    private void Awake()
    {
        contacts = new Collider2D[4];
    }

    private void FixedUpdate()
    {
        TargetDetection();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetContacts(contacts);
            targetPos = collision.gameObject.transform.position - transform.position;
        }
    }

    void TargetDetection()
    {
        Debug.DrawRay(transform.position, targetPos, Color.white);

        visibleZone = Physics2D.Raycast(transform.position, targetPos, vzRange, vzLayers);
        targetVisible = visibleZone.collider != null && (visibleZone.collider.tag == "Player");

        if (targetVisible)
        {
            onTheWatch = true;
        }

        if (onTheWatch)
        {
            memoryZone = Physics2D.Raycast(transform.position, targetPos, mzRange, mzLayers);
            targetAround = memoryZone.collider != null && (memoryZone.collider.tag == "Player");
            if (!memoryZone)
            {
                onTheWatch = false;
            }
        }

        targetDetected = targetVisible || targetAround;
    }
}
