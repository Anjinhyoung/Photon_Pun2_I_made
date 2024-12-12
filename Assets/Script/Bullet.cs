using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    // �Ѿ� �ӵ�
    [SerializeField] float bullet_Speed = 5.0f;

    private void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        transform.position += transform.up * bullet_Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���࿡ �� �Ѿ��� ���̶� �ε����� ���
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && photonView.IsMine) 
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            ScoreManager.scoreManager.CurrentScore = 10; 
        }

        // �ٸ� ����� �Ѿ��� ���̶� �ε����� ���
        else
        {

        }
        
    }
}
