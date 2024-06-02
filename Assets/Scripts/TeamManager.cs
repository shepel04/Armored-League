using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class TeamManager : MonoBehaviourPunCallbacks
{
    public Transform[] blueTeamSpawnPoints;
    public Transform[] orangeTeamSpawnPoints;

    private List<Player> blueTeam = new List<Player>();
    private List<Player> orangeTeam = new List<Player>();

    public enum Team
    {
        Blue,
        Orange
    }

    void AssignTeam(Player player)
    {
        Debug.Log("Assign");
        if (blueTeam.Count <= orangeTeam.Count)
        {
            blueTeam.Add(player);
            ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
            {
                { "team", Team.Blue }
            };
            player.SetCustomProperties(playerProps);
            Debug.Log("Player " + player.NickName + " has assigned to team blue");
        }
        else
        {
            orangeTeam.Add(player);
            ExitGames.Client.Photon.Hashtable playerProps = new ExitGames.Client.Photon.Hashtable
            {
                { "team", Team.Orange }
            };
            player.SetCustomProperties(playerProps);
            Debug.Log("Player " + player.NickName + " has assigned to team orange");
        }
    }

    void SpawnPlayer(Player player)
    {
        Debug.Log("Spawn");
        Team playerTeam = (Team)player.CustomProperties["team"];
        Transform spawnPoint;

        if (playerTeam == Team.Blue)
        {
            spawnPoint = blueTeamSpawnPoints[Random.Range(0, blueTeamSpawnPoints.Length)];
        }
        else
        {
            spawnPoint = orangeTeamSpawnPoints[Random.Range(0, orangeTeamSpawnPoints.Length)];
        }

        PhotonNetwork.Instantiate("PlayerPrefab", spawnPoint.position, spawnPoint.rotation);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnEntered");
        //base.OnPlayerEnteredRoom(newPlayer);
        AssignTeam(newPlayer);
        SpawnPlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Remove the player from the team list if they leave
        if (blueTeam.Contains(otherPlayer))
        {
            blueTeam.Remove(otherPlayer);
        }
        else if (orangeTeam.Contains(otherPlayer))
        {
            orangeTeam.Remove(otherPlayer);
        }
    }
}
