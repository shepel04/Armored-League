using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public TankController Controller;
    public GameObject Camera;
    public GameObject Nick;
    
    public void IsLocalPlayer()
    {
        Controller.enabled = true;
        Nick.SetActive(false);
        Camera.SetActive(true);
    }
}
