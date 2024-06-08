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

        [PunRPC]
        public void TeamOneScored()
        {
            teamOneScore++;
            UpdateScoreUI();
        }

        [PunRPC]
        public void TeamTwoScored()
        {
            teamTwoScore++;
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            teamOneScoreText.text = teamOneScore / 2 + " : " + teamTwoScore / 2;
            teamTwoScoreText.text = teamOneScore / 2 + " : " + teamTwoScore / 2;
        }
    }
}