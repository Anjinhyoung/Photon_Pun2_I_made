using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    // �Ѿ� prefab
    [SerializeField] GameObject bulletPrefab;

    // �ѱ�
    [SerializeField] GameObject firePos;

    // �Ѿ� �ӵ�
    [SerializeField] float bullet_Speed = 10.0f;

    // �Ѿ��� �߻� ������ ���� (ó���� �߻� �����ϰ� �ҷ��� true�� �ʱ���)
    bool can_Fire = true;

    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && can_Fire)
        {
            BulletFire();
        }
    }

    void BulletFire()
    {
        GameObject realBullet;

        realBullet = Instantiate(bulletPrefab, firePos.transform.position, firePos.transform.rotation);

        // Ȥ�� �𸣴ϱ�
        realBullet.transform.up = transform.up;

        StartCoroutine(WaitFire());
    }

    IEnumerator WaitFire()
    {
        can_Fire = false;
        yield return new WaitForSeconds(0.5f);
        can_Fire = true;
    }
}
