using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -10)
        {
            Respawn();
        }       
    }

    void Respawn()
    {
        transform.position = new Vector2(0, 2);
    }
}
