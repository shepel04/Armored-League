using UnityEngine;
using Photon.Pun;

namespace Tank.Sounds
{
    public class JumpEffect : MonoBehaviourPun
    {
        private AudioSource audioSource;
        public TankController tankController;

        [Min(0.0f)]
        public float volumeJump;
        
        public AudioClip onJumpClip;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            tankController.JumpSounded += OnJumpSound;
        }

        private void OnJumpSound()
        {
            photonView.RPC("PlayJumpSound", RpcTarget.All, volumeJump);
        }

        [PunRPC]
        private void PlayJumpSound(float volume)
        {
            audioSource.PlayOneShot(onJumpClip, volume);
        }
    }
}