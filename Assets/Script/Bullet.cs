using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun,IPunObservable
{
    // �Ѿ� �ӵ�
    [SerializeField] float bullet_Speed = 2.0f;

    // �Ѿ��� ��Ʈ��ũ, ���� ������
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
            // ������ Ŭ���̾�Ʈ�� �ƴ϶�� ������ Ŭ���̾�Ʈ���� Enemy ���� ��û
            if (!PhotonNetwork.IsMasterClient)
            {
                if(other.GetComponent<PhotonView>().ViewID == null)
                {
                    print("�� Enemy�� id�� ����?");
                }
                // ���⼭ ������ ����? ���� => ���⼭ �� ������ ����. �ٵ� �� ������ �� �� ����?
                pv.RPC("RpcDestroy", RpcTarget.MasterClient, other.GetComponent<PhotonView>().ViewID);
            }
            // ������ Ŭ���̾�Ʈ�̸� Enemy ���� ����
            else
            {
                PhotonNetwork.Destroy(other.gameObject);
            }

            // bullet ���� �����غ��ϱ� if �� �ȿ� if���� ���� �ʿ� ���� �� ����.  
            if (pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [PunRPC]
    void RpcDestroy(int viewID)
    {
        // Ȥ�� �̰� ������ �׷� �ǰ�? �ƹ� Ŭ���̾�Ʈ���� �� ����Ǵ� �Ŵϱ� ���� ���� �ǰ�?
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
