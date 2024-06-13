using System.Collections.Generic;
using UnityEngine;

namespace Stadium.Sounds
{
    public class FansSoundsList : MonoBehaviour
    {
        public float volumeGoal;
        public List<AudioClip> clipsGoal;
        [Space]
        public float volumeAction;
        public List<AudioClip> clipsAction;
        [Space]
        public float volumeNeutral;
        public List<AudioClip> clipsNeutral;

        public float updateTimeBallPosition;
    }
}
