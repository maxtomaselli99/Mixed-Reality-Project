using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] float speed;
    Vector3 direction;
    bool forward = true;
    void Start()
    {
        start = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (forward)
        {
            direction = end - transform.position;
            transform.position += direction.normalized * speed;
            if (direction.magnitude < 0.1)
            {
                forward = false;
            }
        }
        else
        {
            direction = start - transform.position;
            transform.position += direction.normalized * speed;
            if (direction.magnitude < 0.01)
            {
                forward = true;
            }
        }
    }
}
