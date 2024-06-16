using UnityEngine;

namespace Tank.Sounds
{
    public class JumpEffect : MonoBehaviour
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
            audioSource.PlayOneShot(onJumpClip, volumeJump);
        }
    }
}
