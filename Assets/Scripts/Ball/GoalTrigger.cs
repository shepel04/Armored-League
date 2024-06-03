using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ball
{
    public class GoalTrigger : MonoBehaviour
    {
        public event Action<Vector3, Vector3> GoalIntoTeamOne; // ball position, goal normal
        public event Action<Vector3, Vector3> GoalIntoTeamTwo; // ball position, goal normal

        public event Action ScoreTeamOne;
        public event Action ScoreTeamTwo;

        void Start()
        {
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Team1Goal"))
            {
                GoalIntoTeamOne?.Invoke(
                    transform.position,
                    other.transform.forward);
                ScoreTeamTwo?.Invoke();

                DisableBall();
            }
            else if (other.transform.CompareTag("Team2Goal"))
            {
                GoalIntoTeamTwo?.Invoke(
                    transform.position,
                    other.transform.forward);
                ScoreTeamOne?.Invoke();

                DisableBall();
            }
        }

        private void DisableBall()
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}
