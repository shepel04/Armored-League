using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UIElements;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _region = "eu";
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Dropdown _playersAmountDropdown;
    [SerializeField] private ListItem _roomRowPrefab;
    [SerializeField] private Transform _roomsList;
    
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion(_region);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("You are connected to: " + PhotonNetwork.CloudRegion);
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("You are disconnected from the server");
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("You are not connected to the server");
            return;
        }
        
        RoomOptions roomOptions = new RoomOptions();
        int selIndexDropdown = _playersAmountDropdown.value;
        roomOptions.MaxPlayers = Int32.Parse(_playersAmountDropdown.options[selIndexDropdown].text);

        if (!string.IsNullOrEmpty(_roomNameInputField.text))
        {
            PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions, TypedLobby.Default);
            PhotonNetwork.LoadLevel("Main");
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room " + PhotonNetwork.CurrentRoom.Name + " created!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            ListItem listItem = Instantiate(_roomRowPrefab, _roomsList);
            if (listItem != null)
            {
                listItem.SetInfo(info);
            }
        }
    }
}
