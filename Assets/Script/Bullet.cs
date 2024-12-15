using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun,IPunObservable
{
    // 총알 속도
    [SerializeField] float bullet_Speed = 2.0f;

    // 총알의 네트워크, 로컬 포지션
    Vector3 otherPosition;

    // PhotonView
    PhotonView pv;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        PhotonNetwork.Destroy(gameObject);
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
        // 만약에 내 총알이 적이랑 부딪혔을 경우
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && pv.IsMine) 
        {
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Destroy(other.gameObject);
        }
        // 다른 사람의 총알이 부딪혔을 경우

        else if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !pv.IsMine)
        {
            // rpc로 해야 하나?
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
