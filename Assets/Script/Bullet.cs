using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun,IPunObservable
{
    // �Ѿ� �ӵ�
    [SerializeField] float bullet_Speed = 2.0f;

    // �Ѿ��� ��Ʈ��ũ, ���� ������
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
        // ���࿡ �� �Ѿ��� ���̶� �ε����� ���
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && pv.IsMine) 
        {
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Destroy(other.gameObject);
        }
        // �ٸ� ����� �Ѿ��� �ε����� ���

        else if(other.gameObject.layer == LayerMask.NameToLayer("Enemy") && !pv.IsMine)
        {
            // rpc�� �ؾ� �ϳ�?
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
