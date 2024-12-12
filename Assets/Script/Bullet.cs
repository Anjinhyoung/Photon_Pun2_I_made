using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    // 총알 속도
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
        // 만약에 내 총알이 적이랑 부딪혔을 경우
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") && photonView.IsMine) 
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            ScoreManager.scoreManager.CurrentScore = 10; 
        }

        // 다른 사람의 총알이 적이랑 부딪혔을 경우
        else
        {

        }
        
    }
}
