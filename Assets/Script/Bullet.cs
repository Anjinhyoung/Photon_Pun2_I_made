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
            /*
            Vector3 targetPos = Vector3.Lerp(transform.position, otherPosition, Time.deltaTime * 50);
            float dist = (targetPos - otherPosition).magnitude;
            transform.position = dist > 0.01f ? targetPos : otherPosition;
            */
            transform.position = otherPosition; 
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && PhotonNetwork.IsMasterClient)
        {
            // a. 마스터 클라이언트 화면에서 마스터 클라이언트의 총알이 적을 맞는 경우
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // 마스터 클라이언트 총알 삭제
                PhotonNetwork.Destroy(gameObject);
                // Enemy 삭제
                PhotonNetwork.Destroy(other.gameObject);
            }
            // b. 마스터 클라이언트 화면에서 다른 클라이언트 총알이 적을 맞추는 경우
            else
            {
                // 다른 클라이언트 총알 삭제
                photonView.RPC("RpcBullet", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID);
                // Enemy 삭제
                PhotonNetwork.Destroy(other.gameObject);
            }
        }

        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !PhotonNetwork.IsMasterClient)
        {
            // c. 다른 클라이언트 화면에서 해당 클라이언트의 총알이 적을 맞는 경우
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // Enemy 삭제
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
                // 총알 삭제
                PhotonNetwork.Destroy(gameObject);
              
            }
            // 다른 클라이언트 사람에서 다른 사람의 총알이 적을 맞추는 경우
            else
            {
                // 다른 사람의 총알 삭제
                photonView.RPC("RpcBullet", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID);

                // Enemy 삭제
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            }
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // 마스터 클라이언트가 아니라면 마스터 클라이언트에게 Enemy 삭제 요청
            if (!PhotonNetwork.IsMasterClient)
            {
                // 이 if문이 추가 되니까 갑잘기 오류가 안 난다... 뭐지?
                if(other.GetComponent<PhotonView>().ViewID == null)
                {
                    Debug.Log("Enemy의 View id가 없습니다.");
                }
                // 여기서 오류가 나네? 왜지
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
