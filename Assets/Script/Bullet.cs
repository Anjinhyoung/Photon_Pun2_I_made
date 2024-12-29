using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class Bullet : MonoBehaviourPun,IPunObservable
{
    // �Ѿ� �ӵ�
    [SerializeField] float bullet_Speed = 2.0f;

    // �Ѿ��� ��Ʈ��ũ, ���� ������
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
        // RPC ȣ�� ������ 1. ���� ���� ���� 2. �� ���� �Ѿ��� ����
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // GetComponent�� ��������� ����� ū �޼����̹Ƿ� �ݺ� ȣ���� ��ȿ����
            PhotonView enemyPhotonView = other.GetComponent<PhotonView>();

            // ������ �ƴ� ���
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("RpcDestroy", RpcTarget.MasterClient, enemyPhotonView.ViewID);
            }
            // ������ ���
            else
            {
                // ��Ʈ��ũ ���� ���� �ڵ�
                if(enemyPhotonView == null)
                {
                    return;
                }
                // �����ڰ� �ƴϸ� ������Ʈ�� ������ �Ǹ��� ����.
                PhotonNetwork.Destroy(other.gameObject);
            }

            // ��Ʈ��ũ ���� ���� ���� �ڵ�
            if(photonView == null)
            {
                return;
            }

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject); // ������Ʈ �ı�
                ScoreManager.scoreManager.CurrentScore = 1; // �� Ŭ���̾�Ʈ ���� ����
            }
        }
    }

    [PunRPC]
    void RpcDestroy(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView targetView = PhotonView.Find(viewID);
            // ��Ʈ��ũ ���� ���� ���� �ڵ�
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
