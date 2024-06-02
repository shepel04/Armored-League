using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class NewMatchManager : MonoBehaviourPunCallbacks
{
    public Transform[] blueTeamSpawnPoints;
    public Transform[] orangeTeamSpawnPoints;
    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (photonView.IsMine)
            {
                AsignTeam();
            }
        }
    }

    public void AsignTeam()
    {
    }
    

}
