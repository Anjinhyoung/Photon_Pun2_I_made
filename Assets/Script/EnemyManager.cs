using System.Collections;
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
        while (true)
        {
            // 방장만 적 생성하게 만들기 일단 적 생성 관리는 방장만 하는 게 맞음 이게 없으면 적이 중구 난방으로 생기니까 문제가 있음
            if (PhotonNetwork.IsMasterClient) 
            {
                // RPC가 딱히 필요 없다. 
                PhotonNetwork.Instantiate("Enemy", transform.position, Quaternion.Euler(90, 180, 0));
            }
            yield return new WaitForSeconds(1.5f);
        }
    }
}
