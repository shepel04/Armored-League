using System;
using UnityEngine;

namespace Ball
{
    public class GoalTrigger : MonoBehaviour
    {
        public event Action<int, Vector3, Vector3> GoalIntoTeam; // team index, ball position, goal normal

        public event Action<int> ScoreTeam; // team index

        void Start()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Team1Goal"))
            {
                GoalIntoTeam?.Invoke(
                    0,
                    transform.position,
                    other.transform.forward);
                ScoreTeam?.Invoke(1);

                DisableBall();
            }
            else if (other.transform.CompareTag("Team2Goal"))
            {
                GoalIntoTeam?.Invoke(
                    1,
                    transform.position,
                    other.transform.forward);
                ScoreTeam?.Invoke(0);

                DisableBall();
            }
        }

        private void DisableBall()
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);

            GameObject.FindWithTag("BallProjection").SetActive(false);
        }
    }
}
