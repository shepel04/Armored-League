using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class MatchManager : MonoBehaviourPunCallbacks
{
    private const string TeamProperty = "team";
    private List<Player> blueTeam = new List<Player>();
    private List<Player> orangeTeam = new List<Player>();

    public override void OnJoinedRoom()
    {
        AssignTeams();
    }

    private void AssignTeams()
    {
        Player[] players = PhotonNetwork.PlayerList;
        
        Debug.Log(players[0].NickName);

        blueTeam.Clear();
        orangeTeam.Clear();

        foreach (Player player in players)
        {
            if (blueTeam.Count <= orangeTeam.Count)
            {
                blueTeam.Add(player);
                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TeamProperty, "blue" } });
            }
            else
            {
                orangeTeam.Add(player);
                player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { TeamProperty, "orange" } });
            }
        }

        DebugTeams();
    }

    private void DebugTeams()
    {
        Debug.Log("Blue Team:");
        foreach (Player player in blueTeam)
        {
            Debug.Log(player.NickName);
        }

        Debug.Log("Orange Team:");
        foreach (Player player in orangeTeam)
        {
            Debug.Log(player.NickName);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AssignTeams();
    }

    // Викликаємо, коли гравець виходить з кімнати
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        AssignTeams();
    }
}
