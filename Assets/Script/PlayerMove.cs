using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMove : MonoBehaviour, IPunObservable
{
    public float moveSpeed = 5.0f;
    PhotonView pv;

    // PhotonView.isMine == false의 데이터 값
    Vector3 otherPos;

    Vector3 otherPrevPos;

    private void Start()
    {
        otherPrevPos = transform.position;
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        // 만일, 내가 소유권을 가진 캐릭터라면 
        if (pv.IsMine)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // 월드 좌표
            // Vector3 dirH = Vector3.right * horizontal;
            // Vector3 driV = Vector3.up * vertical;
            // Vector3 dir = dirH + dirV;
            // Vector3 dir = new Vector3(horizontal, vertical, 0);

            // 로컬 좌표
            Vector3 dirH = transform.right * horizontal;
            Vector3 dirV = transform.up * vertical;

            Vector3 dir = dirH + dirV;
            dir.Normalize();

            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

            pos.x = Mathf.Clamp01(pos.x);
            pos.y = Mathf.Clamp01(pos.y);

            transform.position = Camera.main.ViewportToWorldPoint(pos);

            transform.position += dir * moveSpeed * Time.deltaTime;

            // 반쯤 안 삐져냐오게 하는 방법은 이전 위치를 기억하거나 오브젝트마다 크기를 구해서 하기 (12.07 아직 해결 못 함 다음 주에 해결하기)
        }

        else
        {
            // transform.position은 상대방의 현재 위치, otherPos는 네트워크를 통해 수신된 목표 위치 targetPos는 보간된 위치
            Vector3 targetPos = Vector3.Lerp(transform.position, otherPos, Time.deltaTime * 3);
            float dist = (targetPos - otherPrevPos).magnitude;
            transform.position = dist > 0.01f ? targetPos : otherPrevPos;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 매개변수  stream은 데이터를 주고받는 거

        // 만일, 데이터를 서버에 전송(PhotonView.IsMine == true)하는 상태라면
        if (stream.IsWriting)
        {
            // 데이터를 보낸다.
            stream.SendNext(transform.position);
        }

        // 그렇지 않고, 만일 데이터를 서버로부터 읽어오는 상태라면
        else if (stream.IsReading)
        {
            // otherPos가 네트워크를 통해 받은 위치로 업데이트 된다.
            otherPos = (Vector3)stream.ReceiveNext();
        }
    }
}
