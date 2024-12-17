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

    // ���� ���� ���� �°� �質?
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && PhotonNetwork.IsMasterClient)
        {
            // �Ѿ��� �� ������ Ŭ���̾�Ʈ�� ������ Ŭ���̾�Ʈ�� �Ѿ��� ���
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // �� �Ѿ� ����
                PhotonNetwork.Destroy(gameObject);
                PhotonNetwork.Destroy(other.gameObject);
            }
            // �Ѿ��� �� ������ Ŭ���̾�Ʈ�� �ٸ� ����� �Ѿ��� ���
            else
            {
                // �ٸ� ����� �Ѿ� ����
                photonView.RPC("RpcBullet", RpcTarget.Others, gameObject.GetComponent<PhotonView>().ViewID);
                // Enemy ����
                PhotonNetwork.Destroy(other.gameObject);
            }
        }

        // Enemy �������� ������ Ŭ���̾�Ʈ���� �־� �ٸ� Ŭ���̾�Ʈ�� Enemy�� óġ���� ��� MasterClient���� ���� ��û�� �ؾ� �Ѵ�.
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !PhotonNetwork.IsMasterClient)
        {
            // �ٸ� Ŭ���̾�Ʈ���� ���� �Ѿ��� ���
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                // Enemy ������ ������ Ŭ���̾�Ʈ���� ��û
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, gameObject.GetComponent<PhotonView>().ViewID);
                // �� �Ѿ� ����
                PhotonNetwork.Destroy(gameObject);
              
            }
            // �ٸ� Ŭ���̾�Ʈ ������� �ٸ� ����� �Ѿ��� ���
            else
            {
                // �ٸ� ����� �Ѿ� ����
                photonView.RPC("RpcBullet", RpcTarget.Others, gameObject.GetComponent<PhotonView>().ViewID);

                // Enemy ������ ������ Ŭ���̾�Ʈ���� ��û
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
