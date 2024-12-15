using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager gameManager;

    // Enemy�� �߰��ϱ� ���ؼ� ���� ������ �ʿ��ϴ�.
    public GameObject myPlayer;

    private void Awake()
    {
        if(gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(SpawnPlayer());

        // OnPhotonSerializeView ���� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SerializationRate = 60; //(�ʴ�) OnPhotonSerializeView �Լ� ȣ�� Ƚ��  


        // ��κ��� ������ ���� �� �� �����ϱ�(per seconds)
        PhotonNetwork.SendRate = 60; // (�ʴ�)  Ŭ���̾�Ʈ���� ��Ʈ��ũ ��Ŷ(�޽���)�� ������ ������ Ƚ��

        // SendRate >= SerializationRate
    }
    IEnumerator SpawnPlayer()
    {
        // �뿡 ������ �Ϸ�� ������ ��ٸ���.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        float random = Random.Range(-9.0f, 9.0f);
        Vector3 randomPosition = new Vector3(random, -5, 10);
        Vector3 cameraPosition = Camera.main.WorldToViewportPoint(randomPosition);

        // ȭ�� ������ ������ ���� ������ (ī�޶� �ۿ� ������Ʈ�� -�� �ǰų� 1���� ū ���� �� �� �־ �װ��� �����Ѵ�.)
        cameraPosition.x = Mathf.Clamp01(cameraPosition.x);
        cameraPosition.y = Mathf.Clamp01(cameraPosition.y);

        Vector3 initPosition = Camera.main.ViewportToWorldPoint(cameraPosition);

        // ĳ���͸� ��Ʈ��ũ�󿡼� ����ȭ�ϸ� ���� Resource ���������� ã�´�
        GameObject myPlayer = PhotonNetwork.Instantiate("Aircraft", initPosition, Quaternion.identity);

        // ���� �÷��̾�(�ڽ�)�� ���� �� ������ �÷��̾ ������Ʈ�� ���� �����ϴ� ����̴�.
        PhotonNetwork.LocalPlayer.TagObject = myPlayer;
    }
}
