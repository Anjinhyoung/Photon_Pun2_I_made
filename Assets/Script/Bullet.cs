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
            // a. ������ Ŭ���̾�Ʈ ȭ�鿡�� ������ Ŭ���̾�Ʈ�� �Ѿ��� ���� �´� ���
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // ������ Ŭ���̾�Ʈ �Ѿ� ����
                PhotonNetwork.Destroy(gameObject);
                // Enemy ����
                PhotonNetwork.Destroy(other.gameObject);
            }
            // b. ������ Ŭ���̾�Ʈ ȭ�鿡�� �ٸ� Ŭ���̾�Ʈ �Ѿ��� ���� ���ߴ� ���
            else
            {
                // �ٸ� Ŭ���̾�Ʈ �Ѿ� ����
                photonView.RPC("RpcBullet", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID);
                // Enemy ����
                PhotonNetwork.Destroy(other.gameObject);
            }
        }

        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !PhotonNetwork.IsMasterClient)
        {
            // c. �ٸ� Ŭ���̾�Ʈ ȭ�鿡�� �ش� Ŭ���̾�Ʈ�� �Ѿ��� ���� �´� ���
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // Enemy ����
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
                // �Ѿ� ����
                PhotonNetwork.Destroy(gameObject);
              
            }
            // �ٸ� Ŭ���̾�Ʈ ������� �ٸ� ����� �Ѿ��� ���� ���ߴ� ���
            else
            {
                // �ٸ� ����� �Ѿ� ����
                photonView.RPC("RpcBullet", RpcTarget.All, gameObject.GetComponent<PhotonView>().ViewID);

                // Enemy ����
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
            }
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // ������ Ŭ���̾�Ʈ�� �ƴ϶�� ������ Ŭ���̾�Ʈ���� Enemy ���� ��û
            if (!PhotonNetwork.IsMasterClient)
            {
                // �� if���� �߰� �Ǵϱ� ���߱� ������ �� ����... ����?
                if(other.GetComponent<PhotonView>().ViewID == null)
                {
                    Debug.Log("Enemy�� View id�� �����ϴ�.");
                }
                // ���⼭ ������ ����? ����
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
