using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _region = "eu";
    [SerializeField] private TMP_InputField _accountNameInputField;
    [SerializeField] private TMP_InputField _accountPasswordInputField;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Dropdown _playersAmountDropdown;
    [SerializeField] private ListItem _roomRowPrefab;
    [SerializeField] private Transform _roomsList;

    private List<RoomInfo> _allRoomsInfo = new List<RoomInfo>();

    private const string TeamProperty = "team";

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
        AssignTeam();
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

    private void AssignTeam()
    {
        Player[] players = PhotonNetwork.PlayerList;
        int blueTeamCount = 0;
        int orangeTeamCount = 0;

        foreach (Player player in players)
        {
            if (player.CustomProperties.ContainsKey(TeamProperty))
            {
                string team = (string)player.CustomProperties[TeamProperty];
                if (team == "blue") blueTeamCount++;
                else if (team == "orange") orangeTeamCount++;
            }
        }

        string assignedTeam = blueTeamCount <= orangeTeamCount ? "blue" : "orange";
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TeamProperty, assignedTeam } });

        Debug.Log($"Assigned team {assignedTeam} to player {PhotonNetwork.LocalPlayer.NickName}");
    }
}
