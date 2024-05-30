using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TeamManager : MonoBehaviourPunCallbacks
{
    private const string TeamPropertyKey = "Team";

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        AssignTeam();
    }

    void AssignTeam()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Photon.Realtime.Player player = PhotonNetwork.LocalPlayer;

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