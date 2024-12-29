using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviourPun
{
    public static ScoreManager scoreManager;

    // ���� �� ���� Text
    public TMP_Text textCurrentScroe;

    // ���� ��� ���� Text
    public TMP_Text other_textCurrentScroe;

    // ���� ���� ����
    int my_CurrentScore;
    public int CurrentScore
    {
        get { return my_CurrentScore; }
        set { AddScore(value); }
    }

    // ���� ��� ����
    int other_Score;

    public int OtherScore
    {
        get { return other_Score; }
        set { photonView.RPC("Other_AddScore", RpcTarget.Others, value); }
    }

    private void Awake()
    {
        if(scoreManager == null)
        {
            scoreManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���� ������Ű��
    [PunRPC]
    public void AddScore(int addScore)
    {
        // ���� ������ addValue ��ŭ ������
        my_CurrentScore += addScore;

        // ���� ������ UI�� ���Ž�Ű��
        textCurrentScroe.text = $"My Score:{my_CurrentScore}";
    }

    [PunRPC]
    void Other_AddScore(int other_addScore)
    {
        // ���� ������ other_addScore��ŭ ������
        other_Score += other_addScore;
        other_textCurrentScroe.text = $"My Score:{other_Score}";
    }
}


