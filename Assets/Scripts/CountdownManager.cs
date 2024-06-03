using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TMP_Text CountdownText;
    public TMP_Text TimerText;
    public Transform[] BlueSpawnPoints;
    public Transform[] OrangeSpawnPoints;
    
    private GameObject[] _allTanks;
    private List<TankController> _tankControllers;
    private int _countdownTime = 10;
    private int _smallCountdownTime = 3;
    private float _matchCountdownTime = 300f;
    private double _startTime;
    private bool _timerRunning = false;
    

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
            CountdownText.text = _countdownTime.ToString();
            Debug.Log(_countdownTime);
            yield return new WaitForSeconds(1);
            _countdownTime--;
        }
        CountdownText.text = String.Empty;
        RespawnPlayers();
        StartCoroutine(SmallCountdown());

    }
    private IEnumerator SmallCountdown()
    {
        while (_smallCountdownTime > 0)
        {
            CountdownText.text = _smallCountdownTime.ToString();
            Debug.Log(_smallCountdownTime);
            yield return new WaitForSeconds(1);
            _smallCountdownTime--;
        }
        CountdownText.text = "Go!";
        EnablePlayerControllers(true);
        yield return new WaitForSeconds(1);
        StartMatchTimer();
        
        CountdownText.text = string.Empty;
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
        _startTime = PhotonNetwork.Time;
        _timerRunning = true;
    }

    private void FixedUpdate()
    {
        if (_timerRunning)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        float elapsedTime = (float)(PhotonNetwork.Time - _startTime);
        float remainingTime = _matchCountdownTime - elapsedTime;

        if (remainingTime > 0)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            TimerText.text = "00:00";
            _timerRunning = false;
        }
    }
    
}
