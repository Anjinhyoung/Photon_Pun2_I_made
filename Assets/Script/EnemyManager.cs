using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyManager : MonoBehaviourPun
{

    private void Start()
    {
        StartCoroutine(SpawnTime());
    }

    IEnumerator SpawnTime()
    {
        // 무한 루프로 적을 계속 생성 또한 조건이 방장일 때만
        while(true && PhotonNetwork.IsMasterClient)
        {
            GameObject enemy = PhotonNetwork.Instantiate("Enemy",transform.position,Quaternion.identity);
            yield return new WaitForSeconds(1.5f);
        }
    }
}
