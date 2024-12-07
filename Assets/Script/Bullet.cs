using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3.0f);
    }

    void Update()
    {
        transform.position += transform.up * 5.0f * Time.deltaTime;
    }
}
