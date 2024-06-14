using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Random = System.Random;

namespace Tank.Sounds
{
    public class TracksEffect : MonoBehaviourPun
    {
        private Random random = new Random();

        private AudioSource audioSource;
        public TankController tankController;

        [Min(0.0f)]
        public float maxTracksRunVolume;
        //public float maxTracksRunPitch;
        public List<AudioClip> onTracksRunClips;
        public float minTrackWheelRPM;
        public float maxTrackWheelRPM;
        private bool isTracksRunClipPlaying = false;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            tankController.TracksSounded += OnTracksSound;
        }

        private void OnTracksSound(float rpm)
        {
            float volume =
                rpm < minTrackWheelRPM ?
                    0.0f :
                    Mathf.Clamp(rpm / maxTrackWheelRPM, 0.0f, maxTracksRunVolume);
            
            audioSource.volume = volume;
            //audioSource.pitch = Mathf.Clamp(volume / maxTracksRunVolume, 0.0f, maxTracksRunPitch);

            if (volume != 0.0f && !isTracksRunClipPlaying)
                photonView.RPC("PlayEngineWorkClip", RpcTarget.All);
        }

        [PunRPC]
        private IEnumerator PlayEngineWorkClip()
        {
            int clipIndex = random.Next(onTracksRunClips.Count);

            isTracksRunClipPlaying = true;
            audioSource.PlayOneShot(onTracksRunClips[clipIndex]);

            yield return new WaitForSeconds(onTracksRunClips[clipIndex].length * 0.95f);

            isTracksRunClipPlaying = false;
        }
    }
}