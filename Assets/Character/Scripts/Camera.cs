using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject target;
    Vector3 targetPosi;

    Vector3 velo = Vector3.zero;

    public float getKeyTime;
    float getKeyCounter;

    float verInput;
    private void Awake()
    {
        targetPosi.z = -10;
    }

    private void FixedUpdate()
    {
        targetPosi.x = target.transform.position.x;
        targetPosi.y = target.transform.position.y;

        FollowTarget();

        LookingVer();
    }

    private void Update()
    {
        verInput = Input.GetAxisRaw("Vertical");
    }

    void FollowTarget()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosi, ref velo, 0.1F);
    }

    void LookingVer()
    {
        
    }
    void LookingVerInput()
    {
    }

}
