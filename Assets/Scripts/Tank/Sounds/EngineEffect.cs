using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Tank.Sounds
{
    public class EngineEffect : MonoBehaviour
    {
        private Random random = new Random();

        private AudioSource audioSource;
        public TankController tankController;

        [Min(0.0f)]
        public float minEngineWorkVolume;
        public float maxEngineWorkVolume;
        //public float maxEngineWorkPitch;
        public List<AudioClip> onEngineWorkClips;
        public float minInput;
        private bool isEngineWorkClipPlaying = false;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            tankController.EngineSounded += OnEngineSound;
        }

        private void OnEngineSound(float input)
        {
            float volume =
                input < minInput ?
                minEngineWorkVolume :
                input * maxEngineWorkVolume;

            audioSource.volume = volume;
            //audioSource.pitch = Mathf.Clamp(volume / maxEngineWorkVolume, 0.0f, maxEngineWorkPitch);

            if (!isEngineWorkClipPlaying)
                StartCoroutine(PlayEngineWorkClip());
        }
        private IEnumerator PlayEngineWorkClip()
        {
            int clipIndex = random.Next(onEngineWorkClips.Count);

            isEngineWorkClipPlaying = true;
            audioSource.PlayOneShot(onEngineWorkClips[clipIndex]);

            yield return new WaitForSeconds(onEngineWorkClips[clipIndex].length * 0.95f);

            isEngineWorkClipPlaying = false;
        }
    }
}
