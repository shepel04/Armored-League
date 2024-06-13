using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Random = System.Random;

namespace Ball
{
    public class SoundEffects : MonoBehaviour
    {
        private Random random = new Random();
        private Rigidbody rb;

        [Min(0.0f)]
        public float maxBounceVolume;
        public List<AudioClip> onBounceClips;
        public float minBounceForce;
        [Space]
        [Min(0.0f)]
        public float maxRollVolume;
        public float maxRollPitch;
        public List<AudioClip> onRollClips;
        public float minRollVelocity;
        public float maxRollVelocity;
        private bool isRollClipPlaying = false;
        [Space]
        [Min(0.0f)]
        public float minHitVolume;
        public float maxHitVolume;
        public AudioClip onHitClip;
        public float minRatioHitForce;
        [Space]
        [Min(0.0f)]
        public float volumeGoal;
        public AudioClip onGoalClip;
        public float goalSoundDelay;
        [Space]
        public AudioSource audioSourceContinuous;
        public AudioSource audioSourceInstant;
        public AudioSource audioSourceGoal;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnBounce(collision.relativeVelocity.magnitude);
        }
        private void OnBounce(float bounceForce)
        {
            float volume = 
                bounceForce < minBounceForce ?
                1.0f :
                maxBounceVolume;

            audioSourceInstant.PlayOneShot(
                onBounceClips[random.Next(onBounceClips.Count)],
                volume);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnRoll(rb.angularVelocity.magnitude);
        }
        private void OnRoll(float rollVelocity)
        {
            float volume =
                rollVelocity < minRollVelocity ?
                0.0f :
                Mathf.Clamp(rollVelocity / maxRollVelocity, 0.0f, maxRollVolume);

            audioSourceContinuous.volume = volume;
            audioSourceContinuous.pitch = Mathf.Clamp(rollVelocity / maxRollVelocity, 0.0f, maxRollPitch);

            if (volume != 0.0f && !isRollClipPlaying)
                StartCoroutine(PlayRollClip());
        }
        private IEnumerator PlayRollClip()
        {
            int clipIndex = random.Next(onRollClips.Count);

            isRollClipPlaying = true;
            audioSourceContinuous.PlayOneShot(onRollClips[clipIndex]);

            yield return new WaitForSeconds(onRollClips[clipIndex].length * 0.95f);

            isRollClipPlaying = false;
        }
        private void OnCollisionExit(Collision collision)
        {
            isRollClipPlaying = false;
            audioSourceContinuous.volume = 0.0f;
        }

        public void OnHit(float ratioFireForce)
        {
            float volume =
                ratioFireForce < minRatioHitForce ?
                minHitVolume :
                ratioFireForce * maxHitVolume;

            audioSourceInstant.PlayOneShot(onHitClip, volume);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("TeamBlueGoal") || other.CompareTag("TeamOrangeGoal"))
            {
                OnGoal();
            }
        }
        private void OnGoal()
        {
            StartCoroutine(WaitGoalSoundDelay(volumeGoal));
        }
        private IEnumerator WaitGoalSoundDelay(float volume)
        {
            yield return new WaitForSeconds(goalSoundDelay);
            audioSourceGoal.PlayOneShot(onGoalClip, volume);
        }
    }
}
