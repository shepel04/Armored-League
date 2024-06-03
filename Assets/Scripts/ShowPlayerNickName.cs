using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Animations;

public class ShowPlayerNickName : MonoBehaviourPunCallbacks
{
    private GameObject[] _tank;
    private GameObject _playerTank;
    private TMP_Text _nick;
    private Player _player;
    private GameObject _localTank;
    void Start()
    {
        _tank = GameObject.FindGameObjectsWithTag("Player");

        foreach (var tank in _tank)
        {
            _nick.color = tank.GetComponent<PlayerSetup>().PlayerTeam == "blue" ? Color.blue : new Color(1f, 0.5f, 0f);
        
            if (!tank.GetComponent<PhotonView>().IsMine)
            {
                _playerTank = tank;
                
            }
            else
            {
                _localTank = tank;
            }
        }
        
        _nick = GetComponent<TMP_Text>();
        _nick.text = photonView.Owner.NickName;

        gameObject.AddComponent<LookAtConstraint>().AddSource(new ConstraintSource() {sourceTransform = _playerTank.transform, weight = 1f});
    }

    void Update()
    {
        if (_localTank == null)
        {
            return;
        }

        GameObject[] nickObjects = GameObject.FindGameObjectsWithTag("PlayerNick");

        foreach (GameObject nickObject in nickObjects)
        {
            if (nickObject != null)
            {
                nickObject.transform.LookAt(_localTank.transform);
                nickObject.transform.Rotate(0, 180, 0); 
            }
        }
    }
}
