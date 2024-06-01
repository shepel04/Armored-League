using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerTeam : MonoBehaviourPunCallbacks
{
    public enum Team
    {
        Blue,
        Orange
    }

    public Team playerTeam;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            AssignTeam();
        }
    }

    public override void OnJoinedRoom()
    {
        AssignTeam();
    }

    public void AssignTeam()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;

        playerTeam = playerCount % 2 == 0 ? Team.Blue : Team.Orange;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "Team", playerTeam } });

        Debug.Log("Player assigned to: " + playerTeam.ToString());
    }

    public Team GetTeam()
    {
        return playerTeam;
    }
}