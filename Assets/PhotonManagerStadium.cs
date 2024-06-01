using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManagerStadium : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PlayerPref;
    [SerializeField] private Transform[] blueTeamSpawns;
    [SerializeField] private Transform[] orangeTeamSpawns;

    private GameObject _player;

    private const string TeamProperty = "team";

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        string team = (string)PhotonNetwork.LocalPlayer.CustomProperties[TeamProperty];
        Transform spawnPoint = GetSpawnPoint(team);
        _player = PhotonNetwork.Instantiate(PlayerPref.name, spawnPoint.position, spawnPoint.rotation);
        if (_player != null)
        {
            _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        }
    }

    private Transform GetSpawnPoint(string team)
    {
        Transform[] spawns = team == "blue" ? blueTeamSpawns : orangeTeamSpawns;
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
}