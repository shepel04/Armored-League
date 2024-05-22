using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class Ping : MonoBehaviour
{
    public TMP_Text PingText;
    
    void Update()
    {
        int ping = PhotonNetwork.GetPing();
        PingText.text = "Ping: " + ping + " ms";
        
        if (ping <= 80)
        {
            PingText.color = Color.green;
        }
        else if (ping > 80 && ping < 150)
        {
            PingText.color = Color.yellow;
        }
        else
        {
            PingText.color = Color.red;
        }
    }
}
