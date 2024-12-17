using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun,IPunObservable
{
    // 총알 속도
    [SerializeField] float bullet_Speed = 2.0f;

    // 총알의 네트워크, 로컬 포지션
    Vector3 otherPosition;

    Vector3 prevPosition;

    // PhotonView
    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (pv.IsMine)
        {
            transform.position += transform.up * bullet_Speed * Time.deltaTime;
        }

        else
        {
            Vector3 targetPos = Vector3.Lerp(otherPosition, transform.position, Time.deltaTime * 50);
            float dist = (targetPos - otherPosition).magnitude;
            transform.position = dist > 0.01f ? targetPos : otherPosition;
        }
    }

    // 내가 지금 논리를 맞게 썼나?
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && PhotonNetwork.IsMasterClient)
        {
            // 총알이 내 마스터 클라이언트에 마스터 클라이언트의 총알인 경우
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // 내 총알 삭제
                PhotonNetwork.Destroy(gameObject);
                PhotonNetwork.Destroy(other.gameObject);
            }
            // 총알이 내 마스터 클라이언트에 다른 사람의 총알인 경우
            else
            {
                // 다른 사람의 총알 삭제
                photonView.RPC("RpcBullet", RpcTarget.Others, gameObject.GetComponent<PhotonView>().ViewID);
                // Enemy 삭제
                PhotonNetwork.Destroy(other.gameObject);
            }
        }

        // Enemy 소유권은 마스터 클라이언트에게 있어 다른 클라이언트가 Enemy를 처치했을 경우 MasterClient에게 삭제 요청을 해야 한다.
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !PhotonNetwork.IsMasterClient)
        {
            // 다른 클라이언트에서 나의 총알인 경우
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // Enemy 삭제를 마스터 클라이언트에게 요청
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
                // 내 총알 삭제
                PhotonNetwork.Destroy(gameObject);
              
            }
            // 다른 클라이언트 사람에서 다른 사람의 총알인 경우
            else
            {
                // 다른 사람의 총알 삭제
                photonView.RPC("RpcBullet", RpcTarget.Others, gameObject.GetComponent<PhotonView>().ViewID);

                // Enemy 삭제를 마스터 클라이언트에게 요청
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            }
        }
    }

    [PunRPC]
    void RpcBullet(int bulletViewID)
    {
        GameObject bullet = PhotonView.Find(bulletViewID).gameObject;
        PhotonNetwork.Destroy(bullet);
    }

    [PunRPC]
    void RpcDestroy(int enemyViewID)
    {
        GameObject enemy = PhotonView.Find(enemyViewID).gameObject;
        PhotonNetwork.Destroy(enemy);
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
