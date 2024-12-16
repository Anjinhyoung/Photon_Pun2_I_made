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

        // OnPhotonSerializeView 에서 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SerializationRate = 60; //(초당) OnPhotonSerializeView 함수 호출 횟수  


        // 대부분의 데이터 전송 빈도 수 설정하기(per seconds)
        PhotonNetwork.SendRate = 60; // (초당)  클라이언트에서 네트워크 패킷(메시지)을 서버로 보내는 횟수

        // SendRate >= SerializationRate
    }
    IEnumerator SpawnPlayer()
    {
        // 룸에 입장이 완료될 때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        float random = Random.Range(-9.0f, 9.0f);
        Vector3 randomPosition = new Vector3(random, -5, 10);
        Vector3 cameraPosition = Camera.main.WorldToViewportPoint(randomPosition);

        // 화면 밖으로 나가는 것을 강제함 (카메라 밖에 오브젝트는 -가 되거나 1보다 큰 값이 될 수 있어서 그것을 강제한다.)
        cameraPosition.x = Mathf.Clamp01(cameraPosition.x);
        cameraPosition.y = Mathf.Clamp01(cameraPosition.y);

        Vector3 initPosition = Camera.main.ViewportToWorldPoint(cameraPosition);

        // 캐릭터를 네트워크상에서 동기화하며 생성 Resource 폴더에서만 찾는다
        myPlayer = PhotonNetwork.Instantiate("Aircraft", initPosition, Quaternion.identity);

        // 로컬 플레이어(자신)과 게임 내 생성된 플레이어를 오브젝트를 서로 연결하는 방법이다. 
        // PhotonNetwork.LocalPlayer.TagObject = myPlayer; => 그러면 딱히 이게 필요가 없는데;;

        // 딱히 로컬 플레이어로 등록할 필요가 없다.
        RegisterPlayer(myPlayer);
    }

    public void RegisterPlayer(GameObject player)
    {
        int playerID = player.GetComponent<PhotonView>().Owner.ActorNumber;
        int viewID = player.GetComponent<PhotonView>().ViewID;

        // 모든 클라이언트들이 자신의 플레이어를 마스터 클라이언트에게 등록하도록 등록 RPC 요청
        photonView.RPC("RPCRegisterPlayer", RpcTarget.MasterClient, playerID, viewID);

    }

    // 이 함수는 기존에 있는 플레이어들을   등록하게 하는 방법
    [PunRPC]
    void RPCRegisterPlayer(int playerID, int viewID)
    {
        // 마스터 클라이언트에게만 이 메서드 실행
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonView.Find(viewID).gameObject;
            playerCollection[playerID] = player;
        }
    }

    // 새로 들어온 사람이 등록할 때
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        photonView.RPC("RPCRegisterPlayer", RpcTarget.MasterClient, newPlayer.ActorNumber);
    }
}
