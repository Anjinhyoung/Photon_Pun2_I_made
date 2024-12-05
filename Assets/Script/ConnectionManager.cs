using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Reflection;
using Photon.Realtime;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable; // photon hashtable�� ����
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    // �κ�(create or join ȭ��)
    [SerializeField] Button enterRoomButton;

    [SerializeField] GameObject roomInfo_Prefab;
    [SerializeField] Transform scrollContent;
    List<RoomInfo> cachedRoomList = new List<RoomInfo>();

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
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        LoginUI.loginUI.CreateOrJoin();
    }

    public void CreateRoom() // ���� ����� �ݹ� �Լ��� ���� ��� ���� �����.
    {
        string roomName = LoginUI.loginUI.createRoomSettings[0].text;
        int maxPlayer = Convert.ToInt32(LoginUI.loginUI.createRoomSettings[1].text);

        if(roomName.Length > 0 && maxPlayer > 1)
        {
            // ���� ���� �����.(������ ��)
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = maxPlayer;
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true; // �������� �� ���� �˻��� �� �ְ� �ϴ���

            // ���� Ŀ���� ������ �߰��Ѵ�.
            roomOptions.CustomRoomPropertiesForLobby = new string[] { "Master_Name" }; // Ű�� ����ؾ� �Ѵ�.

            // Ű�� �´� �ؽ� ���̺� �߰��ϱ�.
            Hashtable roomTable = new Hashtable();
            roomTable.Add("Master_Name", PhotonNetwork.NickName);
            roomOptions.CustomRoomProperties = roomTable;

            // ���� ����� �Լ�
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default); 
        }
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // ���������� ���� �����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
    }

    // �� �Լ��� �־�� �ٸ� Scene���� ������ �� �ִ�.
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // ���������� �濡 ����Ǿ����� �˷��ش�.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // �濡 ������ ģ������ ��� 1�� ������ �̵�����!
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);

        // �뿡 ������ ������ ������ ���
        Debug.LogError(message);
    }

    public void EnterRoom()
    {
        LoginUI.loginUI.enterRoomButton.SetActive(true);
        LoginUI.loginUI.createOrJoin.SetActive(false);
    }

    // ���� �κ񿡼� ���� ��������� �˷��ִ� �ݹ� �Լ�(���������� �� ������ ����ȭ�ϸ�, �̸� �������� roomList�� ������Ʈ �ȴ�. �ڵ����� ����ȴ�.)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach(RoomInfo room in roomList)
        {
            // ����, ���ŵ� �� ������ ���� ����Ʈ��  �ִٸ�
            if (room.RemovedFromList)
            {
                // cachedRoomList���� �ش� ���� �����Ѵ�.
                cachedRoomList.Remove(room);
            }

            else
            {
                // ����, �̹� cachedRoomList�� �ִ� ���̶��... (�ο� �� ���� �̷� �� ������)
                if (cachedRoomList.Contains(room))
                {
                    // ���� �� ������ �����Ѵ�.
                    cachedRoomList.Remove(room);
                }

                // �� ���� cachedRoomList�� �߰��Ѵ�.
                cachedRoomList.Add(room);
            }
        }

        // UI ������Ʈ�� ���ؼ� ������ ��� �� ������ �����ϱ�
        for(int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i)); // Transform�� ���� �ص� ������Ʈ�� �����ȴ�.
        }

        // �� ���� �����ֱ�
        foreach (RoomInfo room in cachedRoomList)
        {
            // cachedRoomList�� �ִ� ��� ���� ���� ��ũ�Ѻ信 �߰��Ѵ�.
            GameObject realRoom = Instantiate(roomInfo_Prefab, scrollContent);
            RoomInfo_Image roomInfo_Image = realRoom.GetComponent<RoomInfo_Image>();
            roomInfo_Image.SetInfo(room);

            // ��ư�� �� ���� ��� �����ϱ�
            roomInfo_Image.button.onClick.AddListener(() =>
            {
                PhotonNetwork.JoinRoom(room.Name);
            });
        }
    }

    // �뿡 �ٸ� �÷��̾ �������� �� �ݹ� �Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }

    // �뿡 �ٸ� �÷��̾ �������� ���� �ݹ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

}
