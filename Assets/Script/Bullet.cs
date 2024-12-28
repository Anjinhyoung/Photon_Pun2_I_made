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

    Vector3 prevPosition;

    PhotonView pv;
    private void Start()
    {
        pv = GetComponent<PhotonView>();

        if (pv == null)
        {
            Debug.LogError("���� �䰡 �����ϴ�.");
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
        // RPC ȣ�� ������ 1. ���� ���� ���� 2. �� ���� �Ѿ��� ����
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
                PhotonView otherPhotonView = other.GetComponent<PhotonView>();

                // ������ Ŭ���̾�Ʈ�� �ƴ� ���
                if (!PhotonNetwork.IsMasterClient)
                {
                    try
                    {
                        pv.RPC("RpcDestroy", RpcTarget.MasterClient, otherPhotonView.ViewID);
                    }
                    catch (NullReferenceException)
                    {
                        // ��Ʈ��ũ �������� ���� NullReference�� ����
                        // ������ ��ü���� �ᱹ ���ŵ� ���̹Ƿ� ��� �α׵� ������� ����
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
                        // ��Ʈ��ũ �������� ���� NullReference�� ����
                        // ������ ��ü���� �ᱹ ���ŵ� ���̹Ƿ� ��� �α׵� ������� ����
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
                // �ٷ� ����
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
