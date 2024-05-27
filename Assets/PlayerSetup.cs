using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public TankController Controller;
    public GameObject Camera;
    
    public void IsLocalPlayer()
    {
        Controller.enabled = true;
        Camera.SetActive(true);
    }
}
