using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManager;

    GameObject myPlayer;

    public Dictionary<int, GameObject> playerCollection = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if(gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnPlayer());

        // OnPhotonSerializeView ���� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SerializationRate = 60; //(�ʴ�) OnPhotonSerializeView �Լ� ȣ�� Ƚ��  


        // ��κ��� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SendRate = 60; // (�ʴ�)  Ŭ���̾�Ʈ���� ��Ʈ��ũ ��Ŷ(�޽���)�� ������ ������ Ƚ��

        // SendRate >= SerializationRate
    }
    IEnumerator SpawnPlayer()
    {
        // �뿡 ������ �Ϸ�� ������ ��ٸ���.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        float random = Random.Range(-9.0f, 9.0f);
        Vector3 randomPosition = new Vector3(random, -5, 10);
        Vector3 cameraPosition = Camera.main.WorldToViewportPoint(randomPosition);

        // ȭ�� ������ ������ ���� ������ (ī�޶� �ۿ� ������Ʈ�� -�� �ǰų� 1���� ū ���� �� �� �־ �װ��� �����Ѵ�.)
        cameraPosition.x = Mathf.Clamp01(cameraPosition.x);
        cameraPosition.y = Mathf.Clamp01(cameraPosition.y);

        Vector3 initPosition = Camera.main.ViewportToWorldPoint(cameraPosition);

        // ĳ���͸� ��Ʈ��ũ�󿡼� ����ȭ�ϸ� ���� Resource ���������� ã�´�
        myPlayer = PhotonNetwork.Instantiate("Aircraft", initPosition, Quaternion.identity);

        // ���� �÷��̾�(�ڽ�)�� ���� �� ������ �÷��̾ ������Ʈ�� ���� �����ϴ� ����̴�. 
        // PhotonNetwork.LocalPlayer.TagObject = myPlayer; => �׷��� ���� �̰� �ʿ䰡 ���µ�;;

        // ���� ���� �÷��̾�� ����� �ʿ䰡 ����.
        RegisterPlayer(myPlayer);
    }

    public void RegisterPlayer(GameObject player)
    {
        int playerID = player.GetComponent<PhotonView>().Owner.ActorNumber;
        int viewID = player.GetComponent<PhotonView>().ViewID;

        // ��� Ŭ���̾�Ʈ���� �ڽ��� �÷��̾ ������ Ŭ���̾�Ʈ���� ����ϵ��� ��� RPC ��û
        photonView.RPC("RPCRegisterPlayer", RpcTarget.MasterClient, playerID, viewID);

    }

    // �� �Լ��� ������ �ִ� �÷��̾����   ����ϰ� �ϴ� ���
    [PunRPC]
    void RPCRegisterPlayer(int playerID, int viewID)
    {
        // ������ Ŭ���̾�Ʈ���Ը� �� �޼��� ����
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonView.Find(viewID).gameObject;
            playerCollection[playerID] = player;
        }
    }

    // ���� ���� ����� ����� ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        photonView.RPC("RPCRegisterPlayer", RpcTarget.MasterClient, newPlayer.ActorNumber);
    }
}
