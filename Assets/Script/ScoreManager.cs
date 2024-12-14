using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager scoreManager;

    // 현재 점수
    int currentScore;


    /* RPC 때문에 잠깐만
    public int CurrentScore
    {
        get { return currentScore; }
        set { AddScore(value); }
    }
    */

    // 현재 내 점수 Text
    public TMP_Text textCurrentScroe;

    // 현재 상대 점수 Text
    public TMP_Text other_textCurrentScroe;

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
    public void AddScore(int addScore)
    {
        // 현재 점수를 addValue 만큼 더하자
        currentScore += addScore;

        // 현재 점수를 UI에 갱신시키자
        textCurrentScroe.text = $"My Score:{currentScore}";
    }


}
