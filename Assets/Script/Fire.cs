using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    // 총알 prefab
    [SerializeField] GameObject bulletPrefab;

    // 총구
    [SerializeField] GameObject firePos;

    // 총알 속도
    [SerializeField] float bullet_Speed = 10.0f;

    // 총알을 발사 가능지 여부 (처음에 발사 가능하게 할려고 true로 초기함)
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

        // 혹시 모르니까
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
