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
    [SerializeField] private GameObject _passwordInputPrefab;
    
    private RoomInfo _roomInfo;
    private bool _isInstantiated;
    public void SetInfo(RoomInfo info)
    {
        _isInstantiated = false;
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
        var inputPasswordObj = GameObject.FindWithTag("PasswordInputField").GetComponent<TMP_InputField>();
         
        string roomPassword = _roomInfo.CustomProperties["password"] as string;
        
        if (inputPasswordObj.text == roomPassword)
        {
            PhotonNetwork.JoinRoom(_roomName.text);
        }
        else
        {
            inputPasswordObj.text = String.Empty;
        }
    }


    public void JoinToListRoom()
    {
        if (_roomInfo.CustomProperties.ContainsKey("password"))
        {
            if (!_isInstantiated)
            {
                Instantiate(_passwordInputPrefab, new Vector3(6.7f,1.2f,10f), quaternion.identity);
            }
            
            _isInstantiated = true;
        }
        else
        {
            PhotonNetwork.JoinRoom(_roomName.text);
        }
        
    }
}
