using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using Photon.Realtime;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public void StartLogin()
    {
        // ������ ���� ����
        if(LoginUI.loginUI.id.text.Length > 0 && LoginUI.loginUI.password.text.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LoginUI.loginUI.id.text;
            PhotonNetwork.AutomaticallySyncScene = true; // �ڵ����� ��ũ�� �����.(������ �������� ex: ���� ��ġ ���� ��)

            // Name Server�� ���� ��û 
            PhotonNetwork.ConnectUsingSettings();
        }

        else
        {
            LoginUI.loginUI.startButton.interactable = false;
        }
    }

    public override void OnConnected()
    {
        base.OnConnected();

        // Name Server�� ������ �Ϸ�Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + "is Call");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        // ���� ������ ����Ѵ�.
        Debug.LogError("Disconntected from Server" + cause);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // Master Server�� ������ �Ϸ��Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + "is Call");

        // ���� �κ� ����.
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        LoginUI.loginUI.CreateOrJoin();
    }
}
