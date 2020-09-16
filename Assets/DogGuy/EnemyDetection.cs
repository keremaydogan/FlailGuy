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

    public delegate Vector3 GetPosition();
    public static event GetPosition TargetPosition;
    Vector3 targetPos;
    
    [SerializeField] bool targetVisible;
    bool onTheWatch;
    [SerializeField] bool targetAround;
    [SerializeField] bool targetDetected;

    private void FixedUpdate()
    {
        targetPos = TargetPosition() - transform.position;
        TargetDetection();
    }

    void TargetDetection()
    {

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
