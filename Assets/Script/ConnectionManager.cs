using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable; // photon hashtable로 강제

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject roomInfo_Prefab;
    [SerializeField] Transform scrollContent;
    List<RoomInfo> cachedRoomList = new List<RoomInfo>();

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

    public void CreateRoom() // 방을 만드는 콜백 함수가 따로 없어서 직접 만든다.
    {
        string roomName = LoginUI.loginUI.createRoomSettings[0].text;
        int maxPlayer = Convert.ToInt32(LoginUI.loginUI.createRoomSettings[1].text);

        if(roomName.Length > 0 && maxPlayer > 1)
        {
            // 나의 룸을 만든다.(서버의 룸)
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayer;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true; // 누군가가 내 방을 검색할 수 있게 하느냐

            // 룸의 커스텀 정보를 추가한다.
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "Master_Name" }; // 키를 등록해야 한다.

            // 키에 맞는 해시 테이블 추가하기.
            Hashtable roomTable = new Hashtable();
            roomTable.Add("Master_Name", PhotonNetwork.NickName);
            roomOptions.CustomRoomProperties = roomTable;

            // 방을 만드는 함수
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default); 
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // 성공적으로 방이 개설되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    // 현재 로비에서 룸의 변경사항을 알려주는 콜백 함수(내부적으로 룸 정보를 동기화하며, 이를 바탕으로 roomList가 업데이트 된다.
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        // 방 정보 보여주기
        foreach (RoomInfo room in cachedRoomList)
        {
            // cachedRoomList에 있는 모든 방을 만들어서 스크롤뷰에 추가한다.
            GameObject realRoom = Instantiate(roomInfo_Prefab, scrollContent);
            RoomInfo_Image roomInfo_Image = realRoom.GetComponent<RoomInfo_Image>();
            roomInfo_Image.SetInfo(room);

            // 버튼에 방 입장 기능 연결하기
            roomInfo_Image.button.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }
}
