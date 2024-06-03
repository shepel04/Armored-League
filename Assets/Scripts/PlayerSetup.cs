using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public string PlayerTeam;
    public TankController Controller;
    public GameObject Camera;
    public GameObject Nick;
    public GameObject HUD;
    
    public void IsLocalPlayer()
    {
        HUD.SetActive(true);
        Controller.enabled = true;
        Nick.SetActive(false);
        Camera.SetActive(true);
    }
}
