using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviour, IPunObservable
{
    // 속력
    public float speed = 3.0f;

    // Enemy 방향
    Vector3 enemy_Dir;

    // Player (player가 처음부터 있는 건 아니니깐 나중에 생기는 거니까, 잠깐만 전역 변수로 필요 없는 거 같아서 일단은 지워 둠)
    // GameObject player;

    bool can_trace = false;

    // 마스터 클라이언트가 아닐 때의 포지션, 위치
    Vector3 worldPostion;
    Quaternion worldRotation;

    // 마스터 클라이언트가 아닐 때의 이전 포지션
    Vector3 prevWorldPosition;


    void Start()
    {
        StartCoroutine(TraceOrDown());
    }
    void Update()
    {
        if (can_trace)
        {
            // 모든 클라이언트에서 동일하게 적 이동
            transform.position += enemy_Dir * speed * Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        /*

        // 만약에 부딪힌 오브젝트의 LayerMask가 Bullet이며
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet")) 
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            // ScoreManager.scoreManager.CurrentScore = 10; 일단 다른데에서 사용할려고 주석 처리 했다.
        }
        */

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 게임 종료 하게 만들어야 하는데 일단은 두 개의 오브젝트만 파괴하기
            Destroy(gameObject);
            HpSystem.hpSystem.Damage(1);
        }
    }

    IEnumerator TraceOrDown()
    {
        yield return new WaitUntil(() => GameManager.gameManager.myPlayer != null);

        // 랜덤한 값을 뽑자. (0 ~ 9)
        int random = Random.Range(1, 10);
        can_trace = true;
        //  GameObject player = GameObject.Find("Aircraft(Clone)"); => 이것 때문에 처음에 나타난 오브젝트에만 추격을 하는 기능이 있음
        // GameObject player = GameManager.gameManager.myPlayer;  => 일단 잠깐만 굳이 변수를 더 만들지 말고 static있는 거 활용하기

        // 40% 확률은 그냥 쭈욱 내려가게 하기
        if (random < 4)
        {
            // 아 이제 알겠다. (update에서 사용할려면 전역 변수가 필요하다. ,  방향 구하기) 
            enemy_Dir = Vector3.down;

            // Enemy 생성할 때 이 x: 90, y: 180, z:0 으로 해놓기
            // rotation = Quaternion.Euler(90, 180, 0);
            transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        // 60% 확률은 유도탄 처럼 플레이어를 찾아가게 하기
        else
        {
           // player = GameObject.Find("Aircraft(Clone)"); // => 이것 때문에 다양하게 유도탄이 안 되는 거 였구나

            // 방향 구하기
            enemy_Dir = GameManager.gameManager.myPlayer.transform.position - transform.position;
            // 구한 방향을 정규화
            enemy_Dir.Normalize();

            // rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 방장 조건을 제거해야 모든 클라이언트들한테 쫓을 수 있다.
        if (stream.IsWriting)
        {
            // 모든 클라이언트가 적의 위치와 회전을 동기화
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(enemy_Dir);
        }
        else
        {
            // 모든 클라이언트가 데이터를 받아서 적용
            worldPostion = (Vector3)stream.ReceiveNext();
            worldRotation = (Quaternion)stream.ReceiveNext();
            enemy_Dir = (Vector3)stream.ReceiveNext();

            // 이전 위치 저장하기
            prevWorldPosition = transform.position;
        }
    }
}
