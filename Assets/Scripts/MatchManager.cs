using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MatchManager : MonoBehaviourPunCallbacks
{
    public TMP_Text CountdownText;
    public TMP_Text TimerText;
    public RespawnManager RespawnManager;
    
    private GameObject[] _allTanks;
    private int _countdownTime = 10;
    private int _smallCountdownTime = 3;
    private float _matchCountdownTime = 150f; 
    private double _startTime;
    private bool _timerRunning = false;
    private GameObject _ball;
    private PhotonManagerStadium _photonManagerObject;

    [PunRPC]
    void StartCountdown()
    {
        _ball = GameObject.FindWithTag("Ball");
        Debug.Log("Start Countdown");
        _allTanks = GameObject.FindGameObjectsWithTag("Player");
        RespawnManager.Initialize(_ball, _allTanks);
        
        StartCoroutine(Countdown());
    }

    private void Start()
    {
        //_smallCountdownTime = 3;
        _photonManagerObject = GameObject.FindWithTag("PhotonManager").GetComponent<PhotonManagerStadium>();
    }

    [PunRPC]
    public void StartSmallMatchCountdown()
    {
        StartCoroutine(SmallCountdown());
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
        RespawnManager.RespawnPlayersAndBall();
        StartCoroutine(FirstSmallCountdown());
        _photonManagerObject.IsMatchStarted = true;
    }

    private IEnumerator FirstSmallCountdown()
    {
        int _smallCountdownTimeTmp = _smallCountdownTime;
        while (_smallCountdownTimeTmp > 0)
        {
            CountdownText.text = _smallCountdownTimeTmp.ToString();
            yield return new WaitForSeconds(1);
            _smallCountdownTimeTmp--;
        }
        CountdownText.text = "Go!";
        RespawnManager.EnablePlayerControllers(true);
        CountdownText.text = string.Empty;
        
        
        StartMatchTimer();
        
        
        
    }
    private IEnumerator SmallCountdown()
    {
        int _smallCountdownTimeTmp = _smallCountdownTime;
        while (_smallCountdownTimeTmp > 0)
        {
            CountdownText.text = _smallCountdownTimeTmp.ToString();
            yield return new WaitForSeconds(1);
            _smallCountdownTimeTmp--;
        }
        CountdownText.text = "Go!";
        RespawnManager.EnablePlayerControllers(true);
        CountdownText.text = string.Empty;
    }

    void StartMatchTimer()
    {
        _startTime = PhotonNetwork.Time;
        _timerRunning = true;
    }

    [PunRPC]
    void PauseMatchTimer()
    {
        _timerRunning = false;
    }
    
    [PunRPC]
    void ResumeMatchTimer()
    {
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
            photonView.RPC("MatchOver", RpcTarget.All);
        }
    }

    //[PunRPC]
    public void OnGoalScored()
    {
        StartCoroutine(RespawnAfterGoal());
    }

    private IEnumerator RespawnAfterGoal()
    {
        yield return new WaitForSeconds(5);
        RespawnManager.RespawnPlayersAndBall();
        RespawnManager.EnablePlayerControllers(false);
        StartCoroutine(SmallCountdown());
        //photonView.RPC("ResumeMatchTimer", RpcTarget.All);
    }
}
