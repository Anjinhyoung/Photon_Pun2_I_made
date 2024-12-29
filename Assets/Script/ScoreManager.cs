using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ScoreManager : MonoBehaviourPun
{
    public static ScoreManager scoreManager;

    // 현재 내 점수 Text
    public TMP_Text textCurrentScroe;

    // 현재 상대 점수 Text
    public TMP_Text other_textCurrentScroe;

    // 현재 나의 점수
    int my_CurrentScore;
    public int CurrentScore
    {
        get { return my_CurrentScore; }
        set { AddScore(value); }
    }

    // 현재 상대 점수
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

    // 점수 증가시키기
    [PunRPC]
    public void AddScore(int addScore)
    {
        // 현재 점수를 addValue 만큼 더하자
        my_CurrentScore += addScore;

        // 현재 점수를 UI에 갱신시키자
        textCurrentScroe.text = $"My Score:{my_CurrentScore}";
    }

    [PunRPC]
    void Other_AddScore(int other_addScore)
    {
        // 현재 점수를 other_addScore만큼 더하자
        other_Score += other_addScore;
        other_textCurrentScroe.text = $"My Score:{other_Score}";
    }
}


