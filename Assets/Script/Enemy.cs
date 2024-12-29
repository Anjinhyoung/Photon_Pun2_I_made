using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class Enemy : MonoBehaviourPun, IPunObservable
{
    // �ӷ�
    public float speed = 3.0f;

    // Enemy ����
    Vector3 enemy_Dir;

    // ������ Ŭ���̾�Ʈ�� �ƴ� ���� ������, ��ġ, ����
    Vector3 worldPostion;
    Quaternion worldRotation;

    void Start()
    {
        StartCoroutine(Determine_Dir());
        // Ȥ�� ���� �ʱ� �� �����ְ�(���� �� ����)
        worldPostion = transform.position;
        worldRotation = transform.rotation;
    }
    IEnumerator Determine_Dir()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length >= 1);

        // ������ ��� �� �����ϱ�
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ���� ������ ����
            int random = Random.Range(0, PhotonNetwork.PlayerList.Length);

            // ���� ���� RPC�� ����ȭ (���� ��Ʈ��ũ ȯ�濡���� ��� Ŭ���̾�Ʈ �� ��Ȯ�� ����ȭ�� �ﰢ������ �̷����� �ʴ´�.)
            photonView.RPC("RPC_Determine_Dir", RpcTarget.AllBuffered, random);
        }
    }

    [PunRPC]
    void RPC_Determine_Dir(int random)
    {
        // ������ �÷��̾� ����
        Player randomPlayer = PhotonNetwork.PlayerList[random];
        // Player ��ü������ ActorNumber�� ���� �� �� �ִ�.
        int playerID = randomPlayer.ActorNumber;


        // TryGetValue�� C#�� Dictionary Ŭ�������� �����ϴ� �޼����Դϴ�. �� �޼���� ��ųʸ����� ������ Ű�� �ش��ϴ� ���� ã�� �� ��� (������ Ű, �������� value)
        if (GameManager.gameManager.playerCollection.TryGetValue(playerID, out GameObject playerObject))
        {
            // �÷��̾� ������Ʈ�� ��ġ�� ������� ���� ���
            enemy_Dir = playerObject.transform.position - transform.position;
            enemy_Dir.Normalize();
            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back); // Vector3 back�� ����? �� �� �̰͵� �ñ��ϳ� �־ �Ǵ��� ��� �Ǵ���?
        }
    }

    void Update()
    {
        transform.position += enemy_Dir * speed * Time.deltaTime;

        // ������ Ŭ���̾�Ʈ�κ��� ���� ������ ������� ���� ��ġ�� ����
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, worldPostion, Time.deltaTime * 10f);
            // ���⼭ ������ ���µ�...? b�� ������ ���
            transform.rotation = Quaternion.Lerp(transform.rotation, worldRotation, Time.deltaTime * 10f);
        }


        // ��ü���� ����ȭ ����
        // 1. ������ Ŭ���̾�Ʈ���� ���� �ʱ� ����� �̵��� �����Ѵ�.
        // 2. �񸶽��� Ŭ���̾�Ʈ���� ������ Ŭ���̾�Ʈ���Լ� ������ ���� ������ ����
        // 3. �� Ŭ���̾�Ʈ�� transform.position += enemy_Dir * speed * Time.deltaTime; �� �̵�
        // 4. ��Ʈ��ũ �������� ���� ��ġ ������ Lerp�� ����
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ��� Ŭ���̾�Ʈ�� ���� ��ġ�� ����, ȸ���� ����ȭ
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)
        {
            // ��� Ŭ���̾�Ʈ�� �����͸� �޾Ƽ� ����
            worldPostion = (Vector3)stream.ReceiveNext();
            worldRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            HpSystem.hpSystem.Damage(1);
            photonView.RPC("OnDestroy",RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
