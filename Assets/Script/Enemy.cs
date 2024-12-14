using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviour, IPunObservable
{
    // �ӷ�
    public float speed = 3.0f;

    // Enemy ����
    Vector3 enemy_Dir;

    // Player (player�� ó������ �ִ� �� �ƴϴϱ� ���߿� ����� �Ŵϱ�, ��� ���� ������ �ʿ� ���� �� ���Ƽ� �ϴ��� ���� ��)
    // GameObject player;

    bool can_trace = false;

    // ������ Ŭ���̾�Ʈ�� �ƴ� ���� ������, ��ġ
    Vector3 worldPostion;
    Quaternion worldRotation;

    // ������ Ŭ���̾�Ʈ�� �ƴ� ���� ���� ������
    Vector3 prevWorldPosition;


    void Start()
    {
        StartCoroutine(TraceOrDown());
    }
    void Update()
    {
        if (can_trace)
        {
            // ��� Ŭ���̾�Ʈ���� �����ϰ� �� �̵�
            transform.position += enemy_Dir * speed * Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        /*

        // ���࿡ �ε��� ������Ʈ�� LayerMask�� Bullet�̸�
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet")) 
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            // ScoreManager.scoreManager.CurrentScore = 10; �ϴ� �ٸ������� ����ҷ��� �ּ� ó�� �ߴ�.
        }
        */

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // ���� ���� �ϰ� ������ �ϴµ� �ϴ��� �� ���� ������Ʈ�� �ı��ϱ�
            Destroy(gameObject);
            HpSystem.hpSystem.Damage(1);
        }
    }

    IEnumerator TraceOrDown()
    {
        yield return new WaitUntil(() => GameManager.gameManager.myPlayer != null);

        // ������ ���� ����. (0 ~ 9)
        int random = Random.Range(1, 10);
        can_trace = true;
        //  GameObject player = GameObject.Find("Aircraft(Clone)"); => �̰� ������ ó���� ��Ÿ�� ������Ʈ���� �߰��� �ϴ� ����� ����
        // GameObject player = GameManager.gameManager.myPlayer;  => �ϴ� ��� ���� ������ �� ������ ���� static�ִ� �� Ȱ���ϱ�

        // 40% Ȯ���� �׳� �޿� �������� �ϱ�
        if (random < 4)
        {
            // �� ���� �˰ڴ�. (update���� ����ҷ��� ���� ������ �ʿ��ϴ�. ,  ���� ���ϱ�) 
            enemy_Dir = Vector3.down;

            // Enemy ������ �� �� x: 90, y: 180, z:0 ���� �س���
            // rotation = Quaternion.Euler(90, 180, 0);
            transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        // 60% Ȯ���� ����ź ó�� �÷��̾ ã�ư��� �ϱ�
        else
        {
           // player = GameObject.Find("Aircraft(Clone)"); // => �̰� ������ �پ��ϰ� ����ź�� �� �Ǵ� �� ������

            // ���� ���ϱ�
            enemy_Dir = GameManager.gameManager.myPlayer.transform.position - transform.position;
            // ���� ������ ����ȭ
            enemy_Dir.Normalize();

            // rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // ���� ������ �����ؾ� ��� Ŭ���̾�Ʈ������ ���� �� �ִ�.
        if (stream.IsWriting)
        {
            // ��� Ŭ���̾�Ʈ�� ���� ��ġ�� ȸ���� ����ȭ
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(enemy_Dir);
        }
        else
        {
            // ��� Ŭ���̾�Ʈ�� �����͸� �޾Ƽ� ����
            worldPostion = (Vector3)stream.ReceiveNext();
            worldRotation = (Quaternion)stream.ReceiveNext();
            enemy_Dir = (Vector3)stream.ReceiveNext();

            // ���� ��ġ �����ϱ�
            prevWorldPosition = transform.position;
        }
    }
}
