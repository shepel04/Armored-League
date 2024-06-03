using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Random = UnityEngine.Random;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public Transform[] BlueSpawnPoints;
    public Transform[] OrangeSpawnPoints;
    
    [SerializeField] private TMP_Text countdownText;
    private GameObject[] _allTanks;
    private List<TankController> _tankControllers;
    private int _countdownTime = 10;
    private int _smallCountdownTime = 3;

    [PunRPC]
    void StartCountdown()
    {
        Debug.Log("Start Countdown");
        _allTanks = GameObject.FindGameObjectsWithTag("Player");
        if (_allTanks != null)
        {
            _tankControllers = new List<TankController>();
            foreach (var tank in _allTanks)
            {
                _tankControllers.Add(tank.GetComponent<TankController>());
            }
        }
        
        StartCoroutine(Countdown());
        
    }

    private IEnumerator Countdown()
    {
        while (_countdownTime > 0)
        {
            countdownText.text = _countdownTime.ToString();
            Debug.Log(_countdownTime);
            yield return new WaitForSeconds(1);
            _countdownTime--;
        }
        countdownText.text = String.Empty;
        RespawnPlayers();
        StartCoroutine(SmallCountdown());

    }
    private IEnumerator SmallCountdown()
    {
        while (_smallCountdownTime > 0)
        {
            countdownText.text = _countdownTime.ToString();
            Debug.Log(_smallCountdownTime);
            yield return new WaitForSeconds(1);
            _smallCountdownTime--;
        }
        countdownText.text = "Go!";
        EnablePlayerControllers(true);
        yield return new WaitForSeconds(1);
        
        countdownText.text = string.Empty;
    }

    void RespawnPlayers()
    {
        EnablePlayerControllers(false);
        foreach (var tank in _allTanks)
        {
            tank.transform.position = tank.GetComponent<PlayerSetup>().PlayerTeam == "blue"
                ? BlueSpawnPoints[Random.Range(0, BlueSpawnPoints.Length)].position
                : OrangeSpawnPoints[Random.Range(0, OrangeSpawnPoints.Length)].position;
        }
        

    }

    void EnablePlayerControllers(bool parameter)
    {
        foreach (var tank in _allTanks)
        {
            Rigidbody rb = tank.GetComponent<Rigidbody>();
            TankController controller = tank.GetComponent<TankController>();
            controller.StopTank();
            controller._isControllerEnabled = parameter;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        /*foreach (var controller in _tankControllers)
        {
            controller.enabled = parameter;
        }*/
    }

    void StartMatchTimer()
    {
        // ---
    }
    
}
