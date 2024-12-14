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
        // ���� ������ ���� ��� ���� ���� ������ ������ ����
        while(true && PhotonNetwork.IsMasterClient)
        {
            GameObject enemy = PhotonNetwork.Instantiate("Enemy",transform.position,Quaternion.identity);
            yield return new WaitForSeconds(1.5f);
        }
    }
}
