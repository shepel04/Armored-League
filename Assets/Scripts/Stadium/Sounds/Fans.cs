using Ball;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Stadium.Sounds
{
    public class Fans : MonoBehaviour
    {
        public enum FansType { OrangeTeamFans, BlueTeamFans, NeutralFans }
        public FansType fansType;

        private Random random = new Random();

        public enum ClipType { Goal, Action, Neutral }
        private float volumeGoal;
        private List<AudioClip> clipsGoal;
        private float volumeAction;
        private List<AudioClip> clipsAction;
        private float volumeNeutral;
        private List<AudioClip> clipsNeutral;

        private List<bool> isPlayingClip;  // for each audio source

        private List<AudioSource> audioSources;
        private Transform ballTransform;
        private Rigidbody ballRigidbody;

        private float updateTimeBallPosition;
        private float currentUpdateTime = 0.0f;

        private float lastBallPositionZ;

        void Start()
        {
            FansSoundsList fansSoundsList = transform.parent.GetComponent<FansSoundsList>();

            volumeGoal = fansSoundsList.volumeGoal;
            clipsGoal = fansSoundsList.clipsGoal;
            volumeAction = fansSoundsList.volumeAction;
            clipsAction = fansSoundsList.clipsAction;
            volumeNeutral = fansSoundsList.volumeNeutral;
            clipsNeutral = fansSoundsList.clipsNeutral;

            isPlayingClip = new bool[transform.childCount].ToList();

            audioSources = transform.GetComponentsInChildren<AudioSource>().ToList();

            ballTransform = GameObject.FindWithTag("Ball").transform;
            ballRigidbody = ballTransform.GetComponent<Rigidbody>();
            ballTransform.GetComponent<GoalTrigger>().ScoreTeam += OnScoreTeam;

            updateTimeBallPosition = fansSoundsList.updateTimeBallPosition;
            lastBallPositionZ = ballTransform.position.z;
        }

        void FixedUpdate()
        {
            currentUpdateTime += Time.fixedDeltaTime;
            if (currentUpdateTime >= updateTimeBallPosition)
            {
                currentUpdateTime = 0.0f;
                CheckBallCrossFieldCenter();
            }

            //if (ballRigidbody.velocity.z > maxValue)
            //{
            //    maxValue = ballRigidbody.velocity.z;
            //    Debug.Log(ballRigidbody.velocity.z);
            //}

            for (int i = 0; i < audioSources.Count; i++)
            {
                if (!isPlayingClip[i])
                    StartCoroutine(PlayClip(i, ClipType.Neutral));
            }
        }
        private IEnumerator PlayClip(int indexSource, ClipType clipType)
        {
            isPlayingClip[indexSource] = true;

            AudioClip audioClip;
            if (clipType == ClipType.Goal)
            {
                audioClip = clipsGoal[random.Next(clipsGoal.Count)];
                audioSources[indexSource].volume = volumeGoal;
            }
            else if (clipType == ClipType.Action)
            {
                audioClip = clipsAction[random.Next(clipsAction.Count)];
                audioSources[indexSource].volume = volumeAction;
            }
            else
            {
                audioClip = clipsNeutral[random.Next(clipsNeutral.Count)];
                audioSources[indexSource].volume = volumeNeutral;
            }

            audioSources[indexSource].clip = audioClip;
            audioSources[indexSource].Play();

            yield return new WaitForSeconds(audioClip.length);

            isPlayingClip[indexSource] = false;
        }

        private void CheckBallCrossFieldCenter()
        {
            float currentBallPositionZ = ballTransform.position.z;
            if (Mathf.Sign(currentBallPositionZ) != Mathf.Sign(lastBallPositionZ) &&
                Mathf.Abs(ballRigidbody.velocity.z) >= 6.0f)
            {
                lastBallPositionZ = currentBallPositionZ;
            }
            else
            {
                lastBallPositionZ = currentBallPositionZ;
                return;
            }

            bool isPlayActionClips = false;
            if (fansType == FansType.BlueTeamFans && currentBallPositionZ > 0)
                isPlayActionClips = true;
            else if (fansType == FansType.OrangeTeamFans && currentBallPositionZ < 0)
                isPlayActionClips = true;

            if (!isPlayActionClips)
                return;

            for (int i = 0; i < audioSources.Count; i++)
            {
                audioSources[i].Stop();
                isPlayingClip[i] = true;  // prevent fixed update to interrupt with neutral clips

                StartCoroutine(PlayClip(i, ClipType.Action));
            }
        }

        private void OnScoreTeam(int indexTeam)
        {   // 0 - blue team scored
            // 1 - orange team scored

            bool isPlayGoalClips = false;
            if (indexTeam == 0 && fansType == FansType.BlueTeamFans)
                isPlayGoalClips = true;
            else if (indexTeam == 1 && fansType == FansType.OrangeTeamFans)
                isPlayGoalClips = true;
            else if (fansType == FansType.NeutralFans)
                isPlayGoalClips = true;

            if (!isPlayGoalClips)
                return;

            for (int i = 0; i < audioSources.Count; i++)
            {
                audioSources[i].Stop();
                isPlayingClip[i] = true;  // prevent fixed update to interrupt with neutral clips

                StartCoroutine(PlayClip(i, ClipType.Goal));
            }
        }
    }
}
