using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager scoreManager;

    // ���� ����
    int currentScore;


    /* RPC ������ ���
    public int CurrentScore
    {
        get { return currentScore; }
        set { AddScore(value); }
    }
    */

    // ���� �� ���� Text
    public TMP_Text textCurrentScroe;

    // ���� ��� ���� Text
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

    // ���� ������Ű��
    public void AddScore(int addScore)
    {
        // ���� ������ addValue ��ŭ ������
        currentScore += addScore;

        // ���� ������ UI�� ���Ž�Ű��
        textCurrentScroe.text = $"My Score:{currentScore}";
    }


}
