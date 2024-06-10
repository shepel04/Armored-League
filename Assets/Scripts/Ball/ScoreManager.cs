using UnityEngine;
using TMPro;
using Photon.Pun;

namespace Ball
{
    public class ScoreManager : MonoBehaviourPunCallbacks
    {
        public static ScoreManager Instance;

        [SerializeField] private TMP_Text teamOneScoreText;
        [SerializeField] private TMP_Text teamTwoScoreText;

        private int teamOneScore;
        private int teamTwoScore;

        public int BlueTeamScore
        {
            get
            {
                return teamTwoScore;
            }
        }

        public int OrangeTeamScore {
            get
            {
                return teamOneScore;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            UpdateScoreUI();
        }

        //[PunRPC]
        public void TeamOneScored()
        {
            teamOneScore++;
            UpdateScoreUI();
        }

        //[PunRPC]
        public void TeamTwoScored()
        {
            teamTwoScore++;
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            teamOneScoreText.text = teamOneScore + " : " + teamTwoScore;
            teamTwoScoreText.text = teamOneScore + " : " + teamTwoScore;
        }
    }
}