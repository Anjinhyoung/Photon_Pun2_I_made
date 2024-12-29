using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class Bullet : MonoBehaviourPun,IPunObservable
{
    // 총알 속도
    [SerializeField] float bullet_Speed = 2.0f;

    // 총알의 네트워크, 로컬 포지션
    Vector3 otherPosition;
    void Update()
    {
        if (photonView.IsMine)
        {
            transform.position += transform.up * bullet_Speed * Time.deltaTime;
        }

        else
        {
            transform.position = otherPosition; 
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // RPC 호출 때문에 1. 적을 먼저 제거 2. 그 다음 총알을 제거
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // GetComponent는 상대적으로 비용이 큰 메서드이므로 반복 호출은 비효율적
            PhotonView enemyPhotonView = other.GetComponent<PhotonView>();

            // 방장이 아닌 경우
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, enemyPhotonView.ViewID);
            }
            // 방장인 경우
            else
            {
                // 네트워크 오류 방지 코드
                if(enemyPhotonView == null)
                {
                    return;
                }
                // 소유자가 아니면 오브젝트를 삭제할 권리가 없다.
                PhotonNetwork.Destroy(other.gameObject);
            }

            // 네트워크 지연 오류 방지 코드
            if(photonView == null)
            {
                return;
            }

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject); // 오브젝트 파괴
                ScoreManager.scoreManager.CurrentScore = 1; // 내 클라이언트 점수 증가
            }
        }
    }

    [PunRPC]
    void RpcDestroy(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            // 네트워크 지연 오류 방지 코드
            if(targetView == null)
            {
                return;
            }
            PhotonNetwork.Destroy(targetView.gameObject);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            otherPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
