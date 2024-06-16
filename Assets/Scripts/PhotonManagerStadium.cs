using System;
using System.Collections.Generic;
using Ball;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.Serialization;

public class PhotonManagerStadium : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject PlayerPref;
    [SerializeField] private Transform[] BlueTeamSpawns;
    [SerializeField] private Transform[] OrangeTeamSpawns;
    [SerializeField] private TMP_Text PlayerTeamText;
    [SerializeField] private TMP_Text PlayerWaitingText;
    [SerializeField] private GameObject WinCanvas;
    [SerializeField] private GameObject LoseCanvas;
    [SerializeField] private GameObject DrawCanvas;
    [SerializeField] private GameObject MatchHUD;
    public bool IsMatchStarted;
    

    private GameObject _player;
    private GameObject[] _afterMatchCanvases;

    private const string TeamProperty = "team";
    private string _playerTeam;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            AssignTeam();
            SpawnPlayer();
            CheckRoomStatus();
            
        }
    }

    private void Update()
    {
        if (!IsMatchStarted)
        {
            photonView.RPC("StartPlayerAmountChecking", RpcTarget.All);
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
            _player.GetComponent<PlayerSetup>().PlayerTeam = team;
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
    
    void CheckRoomStatus()
    {
        Debug.Log("Check room status");

        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PlayerWaitingText.text = string.Empty;
            
            photonView.RPC("StartCountdown", RpcTarget.All);
            Debug.Log("RPC sent");
        }
        else
        {
            
        }
    }
    
    [PunRPC]
    void StartPlayerAmountChecking()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers && !IsMatchStarted)
        {
            PlayerWaitingText.text = "Waiting for players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
                                     PhotonNetwork.CurrentRoom.MaxPlayers;
        }
        else
        {
            PlayerWaitingText.text = string.Empty;
            IsMatchStarted = true;
        }
        
    }

    [PunRPC]
    public void MatchOver()
    {
        IsMatchStarted = false;
        
        //Time.timeScale = 0; 
        MatchHUD.SetActive(false);
        
        _afterMatchCanvases = GameObject.FindGameObjectsWithTag("AfterMatchCanvas");
        
        Debug.Log("MatchIsOver");

        string matchResult;

        if (ScoreManager.Instance.BlueTeamScore > ScoreManager.Instance.OrangeTeamScore)
        {
            matchResult = "blue";
        }
        else if (ScoreManager.Instance.BlueTeamScore < ScoreManager.Instance.OrangeTeamScore)
        {
            matchResult = "orange";
        }
        else
        {
            matchResult = "draw";
        }

        Player localPlayer = PhotonNetwork.LocalPlayer;

        if ((string)localPlayer.CustomProperties["team"] == matchResult)
        {
            // win canvas
            WinCanvas.SetActive(true);
            Debug.Log("Win");
            UpdatePlayerScore(3);
        }
        else if (matchResult != "draw") 
        {
            // lose canvas
            LoseCanvas.SetActive(true);
            Debug.Log("Lose");
            UpdatePlayerScore(-3);
        }
        else
        {
            // draw canvas
            DrawCanvas.SetActive(true);
            Debug.Log("Draw");
            UpdatePlayerScore(1);
        }
    }
    
    private void UpdatePlayerScore(int scoreChange)
    {
        int currentScore = 0;

        PlayfabManager.Instance.GetPlayerScore((currentScore) => 
        {
            int newScore = currentScore + scoreChange;
            
            if (newScore < 0)
            {
                newScore = 0;
            }
            
            Debug.Log(newScore);
            
            PlayfabManager.Instance.UpdatePlayerScore(newScore);
        });
    }
}