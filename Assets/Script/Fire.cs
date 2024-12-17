using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviourPun
{
    // 총구
    [SerializeField] GameObject firePos;

    // 총알을 발사 가능지 여부 (처음에 발사 가능하게 할려고 true로 초기함)
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

        realBullet = PhotonNetwork.Instantiate("Bullet", firePos.transform.position, firePos.transform.rotation);

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
