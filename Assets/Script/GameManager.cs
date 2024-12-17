using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManager;

    // Room�� �ִ� Player���� ��ųʸ��� ����
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
        GameObject myPlayer = PhotonNetwork.Instantiate("Aircraft", initPosition, Quaternion.identity);

        // �� Ŭ���̾�Ʈ���� Player�� ����ؾ� �Ѵ�. (�ڷ�ƾ ���� RegisterPlayer() �Լ������� ���ÿ��� ����, �Ѿ�, Enemy ������ �ʿ� ����. �Ű� ������ player�� �����ϱ�~)
        RegisterPlayer(myPlayer);
    }

    public void RegisterPlayer(GameObject player)
    {
        // ActorNumber�� Photonview������Ʈ�� �ִ�. (�Ʒ� �� ������ �ʱ�ȭ�� �ٸ� Ŭ���̾�Ʈ������ ���� �� �� ������ �׳�   ���Ǹ� ���ؼ� ���ÿ��� ���� �´�.) 
        int playerID = player.GetComponent<PhotonView>().Owner.ActorNumber;
        int viewID = player.GetComponent<PhotonView>().ViewID;

        // ��� Ŭ���̾�Ʈ���� �ڽ��� �÷��̾ ������ Ŭ���̾�Ʈ���� ����ϵ��� ��� RPC ��û (���߿� ���� Player�鵵 �����ϴ�.)
        photonView.RPC("RPC_RegisterPlayer", RpcTarget.MasterClient, playerID, viewID);
    }

    [PunRPC]
    void RPC_RegisterPlayer(int playerID, int viewID)
    {
        // �߰����� ������ġ
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonView.Find(viewID).gameObject;
            playerCollection[playerID] = player;
        }
    }
}
