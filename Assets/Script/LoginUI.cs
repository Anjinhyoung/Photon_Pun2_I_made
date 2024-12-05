using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public static LoginUI loginUI;

    // 로그인
    public Button startButton;
    public Button exitButton;
    public GameObject login;
    public TMP_InputField id;
    public TMP_InputField password;

    // 방 만들기 or 방 입장
    public GameObject createOrJoin;
    public TMP_InputField[] createRoomSettings;

    // 방 입장
    public GameObject enterRoomButton;




    private void Awake()
    {
        if(loginUI == null)
        {
            loginUI = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Button_Interactive()
    {
        startButton.interactable = true;
    }

    public void CreateOrJoin()
    {
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        login.SetActive(false);
        createOrJoin.SetActive(true);
    }

    public void Out()
    {
        Application.Quit();
    }


}
