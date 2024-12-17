using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 5.0f;
    PhotonView pv;

    // PhotonView.isMine == false�� ������ ��
    Vector3 otherPos;

    Vector3 otherPrevPos;

    private void Start()
    {
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // ����, ���� �������� ���� ĳ���Ͷ�� 
        if (pv.IsMine)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // ���� ��ǥ
            // Vector3 dirH = Vector3.right * horizontal;
            // Vector3 driV = Vector3.up * vertical;
            // Vector3 dir = dirH + dirV;
            // Vector3 dir = new Vector3(horizontal, vertical, 0);

            // ���� ��ǥ
            Vector3 dirH = transform.right * horizontal;
            Vector3 dirV = transform.up * vertical;

            Vector3 dir = dirH + dirV;
            dir.Normalize();

            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

            pos.x = Mathf.Clamp01(pos.x);
            pos.y = Mathf.Clamp01(pos.y);

            transform.position = Camera.main.ViewportToWorldPoint(pos);

            transform.position += dir * moveSpeed * Time.deltaTime;

            // ���� �� �����Ŀ��� �ϴ� ����� ���� ��ġ�� ����ϰų� ������Ʈ���� ũ�⸦ ���ؼ� �ϱ� (12.07 ���� �ذ� �� �� ���� �ֿ� �ذ��ϱ�)
        }

        else
        {
            // transform.position�� ������ ���� ��ġ, otherPos�� ��Ʈ��ũ�� ���� ���ŵ� ��ǥ ��ġ targetPos�� ������ ��ġ
             Vector3 targetPos = Vector3.Lerp(otherPos, transform.position, Time.deltaTime * 50); // ���� �ʹ� ���Ƽ� 50������ �ϴ� �� ���� �� �׳��� 50������ ��ũ�� �� ����
             float dist = (targetPos - otherPos).magnitude;
             transform.position = dist > 0.01f ? targetPos : otherPos;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // �Ű�����  stream�� �����͸� �ְ�޴� ��

        // ����, �����͸� ������ ����(PhotonView.IsMine == true)�ϴ� ���¶��
        if (stream.IsWriting)
        {
            // �����͸� ������.
            stream.SendNext(transform.position);
        }

        // �׷��� �ʰ�, ���� �����͸� �����κ��� �о���� ���¶��
        else if (stream.IsReading)
        {
            // otherPos�� ��Ʈ��ũ�� ���� ���� ��ġ�� ������Ʈ �ȴ�. (�갡 �� ������.)
            otherPos = (Vector3)stream.ReceiveNext();

            // ���� ��ġ ���� => ��� �̰� ���̵� �������� �� �� �غ���
            // otherPrevPos = otherPos;
        }
    }
}
