using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine.UI;

public class ListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    //[SerializeField] private TMP_Text _roomScore;
    [SerializeField] private TMP_Text _roomPlayersAmount;
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private GameObject _passwordInputObject;
    
    private RoomInfo _roomInfo;
    public bool IsInstantiated;
    public void SetInfo(RoomInfo info)
    {
        IsInstantiated = false;
        _roomInfo = info;
        _roomName.text = info.Name;
        if (info.CustomProperties.ContainsKey("password"))
        {
            _lockIcon.SetActive(true);
        }
        //_roomScore
        _roomPlayersAmount.text = info.PlayerCount + "/" + info.MaxPlayers;
    }

    public void CheckPassword()
    {
        string roomPassword = _roomInfo.CustomProperties["password"] as string;
        
        if (_passwordInput.text == roomPassword)
        {
            PhotonNetwork.JoinRoom(_roomName.text);
        }
        else
        {
            _passwordInput.text = String.Empty;
        }
    }


    public void JoinToListRoom()
    {
        if (_roomInfo.CustomProperties.ContainsKey("password"))
        {
            if (!IsInstantiated)
            {
               _passwordInputObject.SetActive(true);
            }
            
            IsInstantiated = true;
        }
        else
        {
            PhotonNetwork.JoinRoom(_roomName.text);
        }
        
    }

    public void InstantStatus(bool parameter)
    {
        IsInstantiated = parameter;
    }
}