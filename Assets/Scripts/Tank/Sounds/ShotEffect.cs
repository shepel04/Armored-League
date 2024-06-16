using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = System.Random;

namespace Tank.Sounds
{
    public class ShotEffect : MonoBehaviourPun
    {
        private Random random = new Random();

        private AudioSource audioSource;
        public TankShooting tankShooting;

        public Transform transformTarget;

        [Min(0.5f)]
        public float maxShotVolume;
        public List<AudioClip> onShotClips;
        public float minRatioShotForce;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            tankShooting.ShotSounded += OnShotSound;
        }

        private void OnShotSound(float ratioFireForce)
        {
            float volume =
                ratioFireForce < minRatioShotForce ?
                    0.1f :
                    ratioFireForce * maxShotVolume;

            photonView.RPC("PlayShotSound", RpcTarget.All, volume, transformTarget.position);
        }

        [PunRPC]
        private void PlayShotSound(float volume, Vector3 position)
        {
            audioSource.transform.parent = null;
            audioSource.transform.position = position;
            audioSource.PlayOneShot(onShotClips[random.Next(onShotClips.Count)], volume);
        }
    }
}