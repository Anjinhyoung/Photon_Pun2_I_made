using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    // �ӷ�
    public float speed = 3.0f;

    // Enemy ����
    Vector3 enemy_Dir;

    // ������ Ŭ���̾�Ʈ�� �ƴ� ���� ������, ��ġ, ����
    Vector3 worldPostion;
    Quaternion worldRotation;
    Vector3 worldEnemyDir;

    PhotonView pv;

    void Start()
    {
        StartCoroutine(TraceOrDown());
        pv = GetComponent<PhotonView>();
    }

    IEnumerator TraceOrDown()
    {
        yield return new WaitUntil(() => GameManager.gameManager.myPlayer != null);

        // ������ ���� ����. (0 ~ 9) => �� ��� ���⵵ ����ȭ�� �ؾ� �� �� ������ ����ī ���� �غ���
        int random = Random.Range(1, 10);

        // 40% Ȯ���� �׳� �޿� �������� �ϱ�
        if (random < 4)
        {
            // �� ���� �˰ڴ�. (update���� ����ҷ��� ���� ������ �ʿ��ϴ�. ,  ���� ���ϱ�) 
            enemy_Dir = Vector3.down;

            // Enemy ������ �� �� x: 90, y: 180, z:0 ���� �س���
            transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        // 60% Ȯ���� ����ź ó�� �÷��̾ ã�ư��� �ϱ�
        else
        {
            int randomPlayerCount = Random.Range(0, PhotonNetwork.PlayerList.Length);

            Player randomPlayer = PhotonNetwork.PlayerList[randomPlayerCount];

            GameObject player = randomPlayer.TagObject as GameObject;

            if (player != null)
            {
                // RPC�� ����� ��� Ŭ���̾�Ʈ�� ���� ���� ����ȭ
                pv.RPC("SetEnemyDirection", RpcTarget.All, player.transform.position);
            }
        }
    }

    [PunRPC]
    void SetEnemyDirection(Vector3 playerPosition)
    {
        // ���� ���� ���
        enemy_Dir = playerPosition - transform.position;
        enemy_Dir.Normalize();

        // �� ȸ�� ����
        transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
    }

    void Update()
    {
        // ��� Ŭ���̾�Ʈ���� ���� �̵� ���� ����
        transform.position += enemy_Dir * speed * Time.deltaTime;

        // ����ȭ�� �����ʹ� �ε巴�� �����Ͽ� �̵� �� ȸ�� ����
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, worldPostion, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, worldRotation, Time.deltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ���� ������ �����ؾ� ��� Ŭ���̾�Ʈ������ ���� �� �ִ�.
        if (stream.IsWriting)
        {
            // ��� Ŭ���̾�Ʈ�� ���� ��ġ�� ����, ȸ���� ����ȭ
            stream.SendNext(transform.position);
            stream.SendNext(enemy_Dir);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // ��� Ŭ���̾�Ʈ�� �����͸� �޾Ƽ� ����
            worldPostion = (Vector3)stream.ReceiveNext();
            worldEnemyDir = (Vector3)stream.ReceiveNext(); 
            worldRotation = (Quaternion)stream.ReceiveNext();

            // �� ���� ������Ʈ�� �ε巴�� ��ȯ
            enemy_Dir = Vector3.Lerp(enemy_Dir, worldEnemyDir, Time.deltaTime * 10f);
        }
    }
}
