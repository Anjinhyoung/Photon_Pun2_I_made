using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System;

public class RoomInfo_Image : MonoBehaviour
{
    [SerializeField]
    TMP_Text[] info;
    public Button button;

    public void SetInfo(RoomInfo room)
    {
        info[0].text = room.Name;

        string masterName = Convert.ToString(room.CustomProperties["Master_Name"]);
        info[1].text = masterName;

        info[2].text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }
}
