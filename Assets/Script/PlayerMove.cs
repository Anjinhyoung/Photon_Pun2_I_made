using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;


    void Update()
    {
        float dirH = Input.GetAxis("Horizontal");
        float dirV = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(dirH, dirV, 0);

        dir.Normalize();

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
