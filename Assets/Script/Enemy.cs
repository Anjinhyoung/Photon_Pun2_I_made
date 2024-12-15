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
        yield return new WaitUntil(() => GameManager.gameManager.myPlayer != null);

        // 랜덤한 값을 뽑자. (0 ~ 9) => 아 잠깐만 여기도 동기화를 해야 할 것 같은데 유레카 내일 해보자
        int random = Random.Range(1, 10);

        // 40% 확률은 그냥 쭈욱 내려가게 하기
        if (random < 4)
        {
            // 아 이제 알겠다. (update에서 사용할려면 전역 변수가 필요하다. ,  방향 구하기) 
            enemy_Dir = Vector3.down;

            // Enemy 생성할 때 이 x: 90, y: 180, z:0 으로 해놓기
            transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        // 60% 확률은 유도탄 처럼 플레이어를 찾아가게 하기
        else
        {
            int randomPlayerCount = Random.Range(0, PhotonNetwork.PlayerList.Length);

            Player randomPlayer = PhotonNetwork.PlayerList[randomPlayerCount];

            GameObject player = randomPlayer.TagObject as GameObject;

            if (player != null)
            {
                // RPC를 사용해 모든 클라이언트에 추적 방향 동기화
                pv.RPC("SetEnemyDirection", RpcTarget.All, player.transform.position);
            }
        }
    }

    [PunRPC]
    void SetEnemyDirection(Vector3 playerPosition)
    {
        // 적의 방향 계산
        enemy_Dir = playerPosition - transform.position;
        enemy_Dir.Normalize();

        // 적 회전 설정
        transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
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
