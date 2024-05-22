using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    //[SerializeField] private TMP_Text _roomScore;
    [SerializeField] private TMP_Text _roomPlayersAmount;

    public void SetInfo(RoomInfo info)
    {
        _roomName.text = info.Name;
        //_roomScore
        _roomPlayersAmount.text = info.PlayerCount + "/" + info.MaxPlayers;
    }

    public void JoinToListRoom()
    {
        PhotonNetwork.JoinRoom(_roomName.text);
    }
}
