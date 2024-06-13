using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Tank.Sounds
{
    public class ShotEffect : MonoBehaviour
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

            audioSource.transform.parent = null;
            audioSource.transform.position = transformTarget.position;

            audioSource.PlayOneShot(
                onShotClips[random.Next(onShotClips.Count)],
                volume);

        }
    }
}
