using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    // 속력
    public float speed = 3.0f;

    // Enemy 방향
    Vector3 enemy_Dir;

    // 마스터 클라이언트가 아닐 때의 포지션, 위치, 방향
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

        // 랜덤한 값을 뽑자. (0 ~ 9) => 아 잠깐만 여기도 동기화를 해야 할 것 같은데, 방장만 할 수 있고 나머지는 네트워크 전송
        if (PhotonNetwork.IsMasterClient)
        {
            // 랜덤 값을 방장이 설정
            int random = Random.Range(1, 10);
            int randomPlayerCount = Random.Range(0, PhotonNetwork.PlayerList.Length);

            // 랜덤 값을 RPC로 동기화
            pv.RPC("RPCTraceOrDown", RpcTarget.AllBuffered, random, randomPlayerCount);
        }
    }

    [PunRPC]
    void RPCTraceOrDown(int random, int randomPlayerCount)
    {
        // 랜덤한 플레이어 추적
        Player randomPlayer = PhotonNetwork.PlayerList[randomPlayerCount];
        int playerID = randomPlayer.ActorNumber;

        if (GameManager.gameManager.playerCollection.TryGetValue(playerID, out GameObject playerObject))
        {
            // 플레이어 오브젝트의 위치를 기반으로 방향 계산
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
        // 모든 클라이언트에서 적의 이동 로직 실행
        transform.position += enemy_Dir * speed * Time.deltaTime;

        // 동기화된 데이터는 부드럽게 보간하여 이동 및 회전 보정
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, worldPostion, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, worldRotation, Time.deltaTime * 10f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 방장 조건을 제거해야 모든 클라이언트들한테 쫓을 수 있다.
        if (stream.IsWriting)
        {
            // 모든 클라이언트가 적의 위치와 방향, 회전을 동기화
            stream.SendNext(transform.position);
            stream.SendNext(enemy_Dir);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 모든 클라이언트가 데이터를 받아서 적용
            worldPostion = (Vector3)stream.ReceiveNext();
            worldEnemyDir = (Vector3)stream.ReceiveNext(); 
            worldRotation = (Quaternion)stream.ReceiveNext();

            // 적 방향 업데이트는 부드럽게 전환
            enemy_Dir = Vector3.Lerp(enemy_Dir, worldEnemyDir, Time.deltaTime * 10f);
        }
    }
}
