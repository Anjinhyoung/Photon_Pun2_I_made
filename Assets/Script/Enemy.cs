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
        yield return new WaitUntil(() => PhotonNetwork.PlayerList.Length != 0);

        // ������ ���� ����. (0 ~ 9) => �� ��� ���⵵ ����ȭ�� �ؾ� �� �� ������, ���常 �� �� �ְ� �������� ��Ʈ��ũ ����
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ���� ������ ����
            int random = Random.Range(1, 10);
            int randomPlayerCount = Random.Range(0, PhotonNetwork.PlayerList.Length);

            // ���� ���� RPC�� ����ȭ
            pv.RPC("RPCTraceOrDown", RpcTarget.AllBuffered, random, randomPlayerCount);
        }
    }

    [PunRPC]
    void RPCTraceOrDown(int random, int randomPlayerCount)
    {
        // ������ �÷��̾� ����
        Player randomPlayer = PhotonNetwork.PlayerList[randomPlayerCount];
        int playerID = randomPlayer.ActorNumber;

        if (GameManager.gameManager.playerCollection.TryGetValue(playerID, out GameObject playerObject))
        {
            // �÷��̾� ������Ʈ�� ��ġ�� ������� ���� ���
            enemy_Dir = playerObject.transform.position - transform.position;
            enemy_Dir.Normalize();
            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
        }
        else
        {
            Debug.LogWarning($"Player with ID {playerID} not found in playerCollection.");
        }
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
