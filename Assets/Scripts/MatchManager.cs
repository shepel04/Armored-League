using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchManager : MonoBehaviourPunCallbacks
{
    public GameObject PlayerPrefab;
    public Transform[] BlueTeamSpawnPoints;
    public Transform[] OrangeTeamSpawnPoints;

    private GameObject _player;
    private const string TeamPropertyKey = "Team";

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.InRoom)
            {
                AssignTeam();
                SpawnPlayer();  
            }
            else
            {
                Debug.Log("Not yet in a room, waiting to join a room before spawning player.");
            }
        }
    }

    void SpawnPlayer()
    {
        Debug.Log("SpawnScript");
        if (PlayerPrefab == null)
        {
            Debug.LogError("Player prefab is not set.");
            return;
        }

        Transform spawnPoint = GetSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogError("No spawn point found.");
            return;
        }

        _player = PhotonNetwork.Instantiate(PlayerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Player " + PhotonNetwork.LocalPlayer.NickName + " spawned");

        if (_player != null)
        {
            _player.GetComponent<PlayerSetup>().IsLocalPlayer();
        }
    }

    Transform GetSpawnPoint()
    {
        // if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(TeamPropertyKey))
        // {
        //     Debug.LogError("Team property not set for local player.");
        //     return null;
        // }
        
        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("LocalPlayer is null.");
            return null;
        }

        string team = PhotonNetwork.LocalPlayer.CustomProperties[TeamPropertyKey].ToString();
        Transform[] spawnPoints = team == "Blue" ? BlueTeamSpawnPoints : OrangeTeamSpawnPoints;

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points set for team " + team);
            return null;
        }
        
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }
    
    void AssignTeam()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        var player = PhotonNetwork.LocalPlayer;

        string team = DetermineTeam(playerCount);
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TeamPropertyKey, team } });
    }

    string DetermineTeam(int playerCount)
    {
        if (playerCount % 2 == 1)
        {
            return "Blue";
        }
        else
        {
            return "Orange";
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(TeamPropertyKey))
        {
            Debug.Log($"Player {targetPlayer.NickName} assigned to Team {changedProps[TeamPropertyKey]}");
        }
    }
}
