using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;

public class Enemy : MonoBehaviourPun, IPunObservable
{
    // 속력
    public float speed = 3.0f;

    // Enemy 방향
    Vector3 enemy_Dir;

    // 마스터 클라이언트가 아닐 때의 포지션, 위치, 방향
    Vector3 worldPostion;
    Quaternion worldRotation;

    void Start()
    {
        StartCoroutine(Determine_Dir());
        // 혹시 몰라서 초기 값 정해주고(에러 안 나게)
        worldPostion = transform.position;
        worldRotation = transform.rotation;
    }
    IEnumerator Determine_Dir()
    {
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length >= 1);

        // 방장이 모든 걸 결정하기
        if (PhotonNetwork.IsMasterClient)
        {
            // 랜덤 값을 방장이 설정
            int random = Random.Range(0, PhotonNetwork.PlayerList.Length);

            // 랜덤 값을 RPC로 동기화 (실제 네트워크 환경에서는 모든 클라이언트 간 정확한 동기화가 즉각적으로 이뤄지지 않는다.)
            photonView.RPC("RPC_Determine_Dir", RpcTarget.AllBuffered, random);
        }
    }

    [PunRPC]
    void RPC_Determine_Dir(int random)
    {
        // 랜덤한 플레이어 추적
        Player randomPlayer = PhotonNetwork.PlayerList[random];
        // Player 자체에서도 ActorNumber를 갖고 올 수 있다.
        int playerID = randomPlayer.ActorNumber;


        // TryGetValue는 C#의 Dictionary 클래스에서 제공하는 메서드입니다. 이 메서드는 딕셔너리에서 지정한 키에 해당하는 값을 찾을 때 사용 (왼쪽이 키, 오른쪽이 value)
        if (GameManager.gameManager.playerCollection.TryGetValue(playerID, out GameObject playerObject))
        {
            // 플레이어 오브젝트의 위치를 기반으로 방향 계산
            enemy_Dir = playerObject.transform.position - transform.position;
            enemy_Dir.Normalize();
            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back); // Vector3 back은 뭐지? 한 번 이것도 궁금하네 있어도 되는지 없어도 되는지?
        }
    }

    void Update()
    {
        transform.position += enemy_Dir * speed * Time.deltaTime;

        // 마스터 클라이언트로부터 받은 정보를 기반으로 적의 위치를 보정
        if (!PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, worldPostion, Time.deltaTime * 10f);
            // 여기서 오류가 났는데...? b가 방장일 경우
            transform.rotation = Quaternion.Lerp(transform.rotation, worldRotation, Time.deltaTime * 10f);
        }


        // 구체적인 동기화 과정
        // 1. 마스터 클라이언트에서 적의 초기 방향과 이동을 결정한다.
        // 2. 비마스터 클라이언트들이 마스터 클라이언트에게서 적들이 방향 정보를 받음
        // 3. 각 클라이언트는 transform.position += enemy_Dir * speed * Time.deltaTime; 로 이동
        // 4. 네트워크 지연으로 인한 위치 오차를 Lerp로 보정
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 모든 클라이언트가 적의 위치와 방향, 회전을 동기화
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)
        {
            // 모든 클라이언트가 데이터를 받아서 적용
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
