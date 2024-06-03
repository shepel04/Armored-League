using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Serialization;

public class PhotonManagerStadium : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PlayerPref;
    [SerializeField] private Transform[] BlueTeamSpawns;
    [SerializeField] private Transform[] OrangeTeamSpawns;
    [SerializeField] private TMP_Text PlayerTeamText;

    private GameObject _player;

    private const string TeamProperty = "team";
    private string _playerTeam;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            AssignTeam();
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        //string team = (string)PhotonNetwork.LocalPlayer.CustomProperties[TeamProperty];
        string team = _playerTeam;
        
        PlayerTeamText.text = team;
        PlayerTeamText.color = team == "blue" ? Color.blue : new Color(1f, 0.5f, 0f);
        
        Transform spawnPoint = GetSpawnPoint(team);
        
        _player = PhotonNetwork.Instantiate(PlayerPref.name, spawnPoint.position, spawnPoint.rotation);
        if (_player != null)
        {
            _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        }
    }

    private Transform GetSpawnPoint(string team)
    {
        Transform[] spawns = team == "blue" ? BlueTeamSpawns : OrangeTeamSpawns;
        int index = UnityEngine.Random.Range(0, spawns.Length);
        return spawns[index];
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (_player != null)
        {
            PhotonNetwork.Destroy(_player.gameObject);
        }
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
        _playerTeam = assignedTeam;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TeamProperty, assignedTeam } });

        Debug.Log($"Assigned team {assignedTeam} to player {PhotonNetwork.LocalPlayer.NickName}");
    }
}