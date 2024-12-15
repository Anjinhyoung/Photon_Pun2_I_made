using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManager;

    // Enemy가 추격하기 위해서 전역 변수가 필요하다.
    public GameObject myPlayer;

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
        GameObject myPlayer = PhotonNetwork.Instantiate("Aircraft", initPosition, Quaternion.identity);

        // 로컬 플레이어(자신)과 게임 내 생성된 플레이어를 오브젝트를 서로 연결하는 방법이다.
        PhotonNetwork.LocalPlayer.TagObject = myPlayer;
    }
}
