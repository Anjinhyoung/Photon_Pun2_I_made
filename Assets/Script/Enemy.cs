using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // �ӷ�
    public float speed = 3.0f;

    // Enemy ����
    Vector3 enemy_Dir;

    // Player
    GameObject player;

    void Start()
    {
        // ������ ���� ����. (0 ~ 9)
        int random = Random.Range(1, 10);

        // 40% Ȯ���� �׳� �޿� �������� �ϱ�
        if(random < 4)
        {
            // �� ���� �˰ڴ�. (update���� ����ҷ��� ���� ������ �ʿ��ϴ�. ,  ���� ���ϱ�) 
            enemy_Dir = Vector3.down;

            // Enemy ������ �� �� x: 90, y: 180, z:0 ���� �س���
            transform.rotation = Quaternion.Euler(90, 180, 0);
        }
        // 60% Ȯ���� ����ź ó�� �÷��̾ ã�ư��� �ϱ�
        else
        {
            player = GameObject.Find("Aircraft");

            // ���� ���ϱ�
            enemy_Dir = player.transform.position - transform.position;
            // ���� ������ ����ȭ
            enemy_Dir.Normalize();

            transform.rotation = Quaternion.LookRotation(enemy_Dir, Vector3.back);
        }


        
    }
    void Update()
    {
        // �����̰� �ϱ� (����, �ӵ�, �ð� ����)
        transform.position += enemy_Dir * speed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        // ���࿡ �ε��� ������Ʈ�� LayerMask�� Bullet�̸�
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            ScoreManager.scoreManager.CurrentScore = 10;
        }

        else if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // ���� ���� �ϰ� ������ �ϴµ� �ϴ��� �� ���� ������Ʈ�� �ı��ϱ�
            Destroy(gameObject);
            HpSystem.hpSystem.Damage(1);
        }
    }
}
