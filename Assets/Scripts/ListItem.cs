using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine.UI;

[Serializable]
public class RoomData
{
    public string Name;
    public string Password;
    public int PlayerCount;
    public int MaxPlayers;
}

public class ListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    //[SerializeField] private TMP_Text _roomScore;
    [SerializeField] private TMP_Text _roomPlayersAmount;
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private GameObject _passwordInputPrefab;
    
    private RoomData _roomData;
    private bool _isInstantiated;
    private const string RoomDataKey = "RoomData";
    
    public void SetInfo(RoomInfo info)
    {
        _isInstantiated = false;
        _roomData = new RoomData
        {
            Name = info.Name,
            Password = info.CustomProperties.ContainsKey("password") ? info.CustomProperties["password"] as string : null,
            PlayerCount = info.PlayerCount,
            MaxPlayers = info.MaxPlayers
        };

        Debug.Log($"Setting Room Info: Name={_roomData.Name}, Password={_roomData.Password}, PlayerCount={_roomData.PlayerCount}, MaxPlayers={_roomData.MaxPlayers}");
        
        _roomName.text = _roomData.Name;
        if (_roomData.Password != null)
        {
            _lockIcon.SetActive(true);
        }
        else
        {
            _lockIcon.SetActive(false);
        }
        //_roomScore
        _roomPlayersAmount.text = _roomData.PlayerCount + "/" + _roomData.MaxPlayers;
        
        PlayerPrefs.SetString(RoomDataKey, JsonUtility.ToJson(_roomData));
        PlayerPrefs.Save();
    }

    public void CheckPassword()
    {
        if (!PlayerPrefs.HasKey(RoomDataKey))
        {
            Debug.LogError("Room data not found in PlayerPrefs!");
            return;
        }

        _roomData = JsonUtility.FromJson<RoomData>(PlayerPrefs.GetString(RoomDataKey));

        Debug.Log($"Checking Password: Name={_roomData.Name}, Password={_roomData.Password}");

        var inputPasswordObj = GameObject.FindWithTag("PasswordInputField").GetComponent<TMP_InputField>();
        if (inputPasswordObj == null)
        {
            Debug.LogError("Password input field not found!");
            return;
        }

        Debug.Log($"Input Password: {inputPasswordObj.text}");

        if (inputPasswordObj.text == _roomData.Password)
        {
            PhotonNetwork.JoinRoom(_roomData.Name);
        }
        else
        {
            inputPasswordObj.text = String.Empty;
            Debug.Log("Password does not match");
        }
    }

    public void JoinToListRoom()
    {
        if (!PlayerPrefs.HasKey(RoomDataKey))
        {
            Debug.LogError("Room data not found in PlayerPrefs!");
            return;
        }

        _roomData = JsonUtility.FromJson<RoomData>(PlayerPrefs.GetString(RoomDataKey));

        Debug.Log($"Joining Room: Name={_roomData.Name}, Password={_roomData.Password}");

        if (_roomData.Password != null)
        {
            if (!_isInstantiated)
            {
                Instantiate(_passwordInputPrefab, new Vector3(6.7f, 1.2f, 10f), quaternion.identity);
                _isInstantiated = true;
            }
        }
        else
        {
            PhotonNetwork.JoinRoom(_roomData.Name);
        }
    }
}
