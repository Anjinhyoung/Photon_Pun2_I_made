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
        // 접속을 위한 설정
        if(LoginUI.loginUI.id.text.Length > 0 && LoginUI.loginUI.password.text.Length > 0)
        {
            PhotonNetwork.GameVersion = "1.0.0";
            PhotonNetwork.NickName = LoginUI.loginUI.id.text;
            PhotonNetwork.AutomaticallySyncScene = true; // 자동으로 싱크를 맞춘다.(방장의 기준으로 ex: 물건 배치 같은 거)

            // Name Server에 관한 요청 
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

        // Name Server에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + "is Call");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        // 실패 원인을 출력한다.
        Debug.LogError("Disconntected from Server" + cause);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // Master Server에 접속이 완려되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + "is Call");

        // 서버 로비에 들어간다.
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        LoginUI.loginUI.CreateOrJoin();
    }
}
