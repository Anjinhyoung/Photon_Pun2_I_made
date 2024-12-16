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
            // ���常 �� �����ϰ� ����� �ϴ� �� ���� ������ ���常 �ϴ� �� ���� �̰� ������ ���� �߱� �������� ����ϱ� ������ ����
            if (PhotonNetwork.IsMasterClient) 
            {
                // RPC�� ���� �ʿ� ����.
                PhotonNetwork.Instantiate("Enemy", transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(1.5f);
        }
    }
}
