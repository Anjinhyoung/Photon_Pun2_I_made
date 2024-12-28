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

    Vector3 prevPosition;

    PhotonView pv;
    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (pv == null)
        {
            Debug.LogError("포톤 뷰가 없습니다.");
        }
    }

    void Update()
    {
        if (pv.IsMine)
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
                PhotonView otherPhotonView = other.GetComponent<PhotonView>();

                // 마스터 클라이언트가 아닌 경우
                if (!PhotonNetwork.IsMasterClient)
                {
                    try
                    {
                        pv.RPC("RpcDestroy", RpcTarget.MasterClient, otherPhotonView.ViewID);
                    }
                    catch (NullReferenceException)
                    {
                        // 네트워크 지연으로 인한 NullReference는 무시
                        // 어차피 객체들은 결국 제거될 것이므로 경고 로그도 출력하지 않음
                        return;
                    }
                }
                else
                {
                    PhotonNetwork.Destroy(other.gameObject);
                }

                if (pv.IsMine)
                {
                    try
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                    catch (NullReferenceException)
                    {
                        // 네트워크 지연으로 인한 NullReference는 무시
                        // 어차피 객체들은 결국 제거될 것이므로 경고 로그도 출력하지 않음
                        return;
                    }
            }
        }
    }

    [PunRPC]
    void RpcDestroy(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            try
            {
                // 바로 삭제
                PhotonNetwork.Destroy(targetView.gameObject);
            }

            catch (NullReferenceException)
            {
                return;
            }            
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
