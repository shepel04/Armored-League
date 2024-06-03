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
    void Start()
    {
        _tank = GameObject.FindGameObjectsWithTag("Player");

        foreach (var tank in _tank)
        {
            if (!tank.GetComponent<PhotonView>().IsMine)
            {
                _playerTank = tank;
            }
        }
        
        _nick = GetComponent<TMP_Text>();
        _nick.text = photonView.Owner.NickName;

        gameObject.AddComponent<LookAtConstraint>().AddSource(new ConstraintSource() {sourceTransform = _playerTank.transform, weight = 1f});
    }

    private void Update()
    {
        //_nick.transform.rotation = player
    }
}
