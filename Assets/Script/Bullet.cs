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

    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (pv == null)
        {
            Debug.LogError("PhotonView is not assigned or missing!");
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 마스터 클라이언트가 아니라면 마스터 클라이언트에게 Enemy 삭제 요청
            if (!PhotonNetwork.IsMasterClient)
            {
                if(other.GetComponent<PhotonView>().ViewID == null)
                {
                    print("왜 Enemy의 id가 없지?");
                }
                // 여기서 오류가 나네? 왜지 => 여기서 또 오류가 난다. 근데 또 오류가 안 나 뭐지?
                pv.RPC("RpcDestroy", RpcTarget.MasterClient, other.GetComponent<PhotonView>().ViewID);
            }
            // 마스터 클라이언트이면 Enemy 직접 삭제
            else
            {
                PhotonNetwork.Destroy(other.gameObject);
            }

            // bullet 삭제 생각해보니까 if 문 안에 if문은 따로 필요 없는 거 같다.  
            if (pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    void RpcDestroy(int viewID)
    {
        // 혹시 이거 때문에 그런 건가? 아무 클라이언트에서 다 실행되는 거니까 오류 나는 건가?
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            if (targetView != null)
            {
                PhotonNetwork.Destroy(targetView.gameObject);
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
