using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManager;

    // Room에 있는 Player들을 딕셔너리로 저장
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
        GameObject myPlayer = PhotonNetwork.Instantiate("Aircraft", initPosition, Quaternion.identity);

        // 각 클라이언트마다 Player를 등록해야 한다. (코루틴 부터 RegisterPlayer() 함수까지는 로컬에서 실행, 총알, Enemy 걱정은 필요 없다. 매개 변수로 player을 넣으니까~)
        RegisterPlayer(myPlayer);
    }

    public void RegisterPlayer(GameObject player)
    {
        // ActorNumber도 Photonview컴포넌트에 있다. (아래 두 변수의 초기화는 다른 클라이언트에서도 갖고 올 수 있지만 그냥   편의를 위해서 로컬에서 갖고 온다.) 
        int playerID = player.GetComponent<PhotonView>().Owner.ActorNumber;
        int viewID = player.GetComponent<PhotonView>().ViewID;

        // 모든 클라이언트들이 자신의 플레이어를 마스터 클라이언트에게 등록하도록 등록 RPC 요청 (나중에 들어온 Player들도 가능하다.)
        photonView.RPC("RPC_RegisterPlayer", RpcTarget.MasterClient, playerID, viewID);
    }

    [PunRPC]
    void RPC_RegisterPlayer(int playerID, int viewID)
    {
        // 추가적인 안전장치
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject player = PhotonView.Find(viewID).gameObject;
            playerCollection[playerID] = player;
        }
    }
}
