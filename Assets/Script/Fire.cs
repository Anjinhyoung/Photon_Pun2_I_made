using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviourPun
{
    // �Ѿ� prefab
    [SerializeField] GameObject bulletPrefab;

    // �ѱ�
    [SerializeField] GameObject firePos;

    // �Ѿ��� �߻� ������ ���� (ó���� �߻� �����ϰ� �ҷ��� true�� �ʱ���)
    bool can_Fire = true;

    

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && can_Fire && photonView.IsMine)
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
