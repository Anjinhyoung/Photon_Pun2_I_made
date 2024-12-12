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

    // Player (player�� ó������ �ִ� �� �ƴϴϱ� ���߿� ����� �Ŵϱ�)
    GameObject player;

    bool can_trace = false;

    void Start()
    {
        StartCoroutine(TraceOrDown());
    }
    void Update()
    {
        if (can_trace)
        {
            // �����̰� �ϱ� (����, �ӵ�, �ð� ����)
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
        yield return new WaitUntil(() => GameObject.Find("Aircraft(Clone)") != null);

        // ������ ���� ����. (0 ~ 9)
        int random = Random.Range(1, 10);
        can_trace = true;
        player = GameObject.Find("Aircraft(Clone)");

        // 40% Ȯ���� �׳� �޿� �������� �ϱ�
        if (random < 4)
        {
            // �� ���� �˰ڴ�. (update���� ����ҷ��� ���� ������ �ʿ��ϴ�. ,  ���� ���ϱ�) 
            enemy_Dir = Vector3.down;

            // Enemy ������ �� �� x: 90, y: 180, z:0 ���� �س���
            transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        // 60% Ȯ���� ����ź ó�� �÷��̾ ã�ư��� �ϱ�
        else
        {
           // player = GameObject.Find("Aircraft(Clone)"); // => �̰� ������ �پ��ϰ� ����ź�� �� �Ǵ� �� ������

            // ���� ���ϱ�
            enemy_Dir = player.transform.position - transform.position;
            // ���� ������ ����ȭ
            enemy_Dir.Normalize();

            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
