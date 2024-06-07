using Ball;
using UnityEngine;

namespace Goal.Effects
{
    public class Goal : MonoBehaviour
    {
        public GameObject[] goalEffects;

        void Start()
        {
            GameObject.FindWithTag("Ball")
                .GetComponent<GoalTrigger>()
                .GoalIntoTeam += OnGoalTeam;
        }

        private void OnGoalTeam(
            int teamIndex,
            Vector3 ballPosition,
            Vector3 goalNormal)
        {
            GameObject effect = goalEffects[teamIndex];
            effect.transform.position = ballPosition;
            effect.transform.up = goalNormal;

            Instantiate(effect);
        }
    }
}
