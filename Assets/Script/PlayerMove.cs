using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 월드 좌표
        // Vector3 dirH = Vector3.right * horizontal;
        // Vector3 driV = Vector3.up * vertical;
        // Vector3 dir = dirH + dirV;
        // Vector3 dir = new Vector3(horizontal, vertical, 0);

        // 로컬 좌표
        Vector3 dirH = transform.right * horizontal;
        Vector3 dirV = transform.up * vertical;

        Vector3 dir = dirH + dirV;
        dir.Normalize();

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);

        transform.position = Camera.main.ViewportToWorldPoint(pos);

        transform.position += dir * moveSpeed * Time.deltaTime;

        // 반쯤 안 삐져냐오게 하는 방법은 이전 위치를 기억하거나 오브젝트마다 크기를 구해서 하기 (12.07 아직 해결 못 함 다음 주에 해결하기)
    }
}
