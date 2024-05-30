using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


using Quaternion = UnityEngine.Quaternion;

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
    private GameObject _player;
    [SerializeField] private GameObject PlayerPref;
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.ConnectToRegion(_region);
        }
        
        // if (SceneManager.GetActiveScene().name == "Main")
        // {
        //     _player = PhotonNetwork.Instantiate(PlayerPref.name, new Vector3(0, 1, 0), Quaternion.identity);
        // }
        
        // if (_player != null)
        // {
        //     _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        // }
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
        //_player = PhotonNetwork.Instantiate(PlayerPref.name, new Vector3(0,1,0), Quaternion.identity);
        Debug.Log("Room " + PhotonNetwork.CurrentRoom.Name + " created!");
        PhotonNetwork.LoadLevel("Main");
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
        PhotonNetwork.Destroy(_player.gameObject);
        PhotonNetwork.LoadLevel("MainMenu");
    
    }
}
