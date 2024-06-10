using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class RespawnManager : MonoBehaviourPunCallbacks
{
    public Transform[] BlueSpawnPoints;
    public Transform[] OrangeSpawnPoints;

    private GameObject[] _allTanks;
    private GameObject _ball;
    private Rigidbody _ballRigidbody;

    public void Initialize(GameObject ball, GameObject[] allTanks)
    {
        _ball = ball;
        _ballRigidbody = _ball.GetComponent<Rigidbody>();
        _allTanks = allTanks;
    }

    public void RespawnPlayersAndBall()
    {
        EnablePlayerControllers(false);
        foreach (var tank in _allTanks)
        {
            Transform spawnPoint = tank.GetComponent<PlayerSetup>().PlayerTeam == "blue"
                ? BlueSpawnPoints[Random.Range(0, BlueSpawnPoints.Length)]
                : OrangeSpawnPoints[Random.Range(0, OrangeSpawnPoints.Length)];
        
            tank.transform.position = spawnPoint.position;
            tank.transform.rotation = spawnPoint.rotation;
        }

        _ball.transform.position = new Vector3(0, 3, 0);
        _ballRigidbody.velocity = Vector3.zero;
        _ballRigidbody.angularVelocity = Vector3.zero;
        _ball.SetActive(true);
    }


    public void EnablePlayerControllers(bool parameter)
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
    }
}