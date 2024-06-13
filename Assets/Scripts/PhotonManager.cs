using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _region = "eu";
    [SerializeField] private TMP_InputField _accountNameInputField;
    [SerializeField] private TMP_InputField _accountPasswordInputField;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_InputField _roomPasswordInputField;
    [SerializeField] private TMP_Dropdown _playersAmountDropdown;
    [SerializeField] private ListItem _roomRowPrefab;
    [SerializeField] private Transform _roomsList;
    [SerializeField] private Toggle _passwordToggle;

    private List<RoomInfo> _allRoomsInfo = new List<RoomInfo>();

    private const string TeamProperty = "team";
    private string _roomPassword;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion(_region);
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("You are connected to: " + PhotonNetwork.CloudRegion);

        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
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
        
        if (_passwordToggle.isOn)
        {
            _roomPassword = _roomPasswordInputField.text;
            if (!string.IsNullOrEmpty(_roomPassword))
            {
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "password", _roomPassword } };
                roomOptions.CustomRoomPropertiesForLobby = new string[] { "password" };
            }
        }

        if (!string.IsNullOrEmpty(_roomNameInputField.text))
        {
            PhotonNetwork.CreateRoom(_roomNameInputField.text, roomOptions, TypedLobby.Default);
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room " + PhotonNetwork.CurrentRoom.Name + " created!");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room " + message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            for (int i = 0; i < _allRoomsInfo.Count; i++)
            {
                if (_allRoomsInfo[i].masterClientId == info.masterClientId)
                {
                    return;
                }
            }
            
            ListItem listItem = Instantiate(_roomRowPrefab, _roomsList);
            
            if (listItem != null)
            {
                listItem.SetInfo(info);
                _allRoomsInfo.Add(info);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main");
    }

    public void JoinRandRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    
}
