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

        // ���� ��ǥ
        // Vector3 dirH = Vector3.right * horizontal;
        // Vector3 driV = Vector3.up * vertical;
        // Vector3 dir = dirH + dirV;
        // Vector3 dir = new Vector3(horizontal, vertical, 0);

        // ���� ��ǥ
        Vector3 dirH = transform.right * horizontal;
        Vector3 dirV = transform.up * vertical;

        Vector3 dir = dirH + dirV;
        dir.Normalize();

        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);

        transform.position = Camera.main.ViewportToWorldPoint(pos);

        transform.position += dir * moveSpeed * Time.deltaTime;

        // ���� �� �����Ŀ��� �ϴ� ����� ���� ��ġ�� ����ϰų� ������Ʈ���� ũ�⸦ ���ؼ� �ϱ� (12.07 ���� �ذ� �� �� ���� �ֿ� �ذ��ϱ�)
    }
}
