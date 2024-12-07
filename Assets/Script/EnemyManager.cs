using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject EnemyPrefab;

    private void Start()
    {
        StartCoroutine(SpawnTime());
    }

    IEnumerator SpawnTime()
    {
        // ���� ������ ���� ��� ����
        while(true)
        {
            GameObject enemy = Instantiate(EnemyPrefab);
            enemy.transform.position = transform.position;
            yield return new WaitForSeconds(1.5f);
        }
    }
}
