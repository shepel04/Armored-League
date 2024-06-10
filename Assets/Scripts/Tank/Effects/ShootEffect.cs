using System;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Unity.Mathematics;
using UnityEngine;

namespace Tank.Effects
{
    public class ShootEffect : MonoBehaviour
    {
        public GameObject shootPrefab;
        private Transform _targetTransform;
        private Vector3 _offset = new Vector3(0, 0.01f, 0.0386f);

        private void Start()
        {
            var allTanks = GameObject.FindGameObjectsWithTag("Player");
            var shootPoints = GameObject.FindGameObjectsWithTag("TankShootVFX");
            
            foreach (var point in shootPoints)
            {
                if (point.GetComponentInParent<PhotonView>().IsMine)
                {
                    _targetTransform = point.transform;
                }
            }
        }

        private void OnShoot()
        {
            shootPrefab.transform.position = _targetTransform.position;
            shootPrefab.transform.up = _targetTransform.forward;

            PhotonNetwork.Instantiate(shootPrefab.name, shootPrefab.transform.position, quaternion.identity);
        }
    }
}
