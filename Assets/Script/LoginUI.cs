using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public static LoginUI loginUI;

    public GameObject login;
    public Button startButton;
    public Button exitButton;
    public TMP_InputField id;
    public TMP_InputField password;


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
        login.SetActive(false);
        startButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
    }



}
