using UnityEngine;

namespace Tank.Animations
{
    public class EngineRadiator : MonoBehaviour
    {
        public TankController tankController;
        private Animator animator;

        public float pointsNoLaden = 2.0f;
        public float pointsLaden = 5.0f;
        private float pointsCurrent = 0.0f;

        private bool isOpened = false;
        private bool isLaden = false;

        void Start()
        {
            tankController.EngineLaden += OnEngineLaden;
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (isOpened)
            {
                if (isLaden)
                {
                    if (pointsCurrent - Time.fixedDeltaTime >= 0)
                        pointsCurrent -= Time.fixedDeltaTime;
                }
                else
                {
                    pointsCurrent += Time.fixedDeltaTime;
                }

                if (pointsCurrent >= pointsNoLaden)
                {
                    animator.SetTrigger("changeState");
                    isOpened = false;
                    pointsCurrent = 0.0f;
                }
            }
            else
            {
                if (isLaden)
                {
                    pointsCurrent += Time.fixedDeltaTime;
                }
                else
                {
                    if (pointsCurrent - Time.fixedDeltaTime >= 0)
                        pointsCurrent -= Time.fixedDeltaTime;
                }

                if (pointsCurrent >= pointsLaden)
                {
                    animator.SetTrigger("changeState");
                    isOpened = true;
                    pointsCurrent = 0.0f;
                }
            }
        }

        private void OnEngineLaden(bool isLaden)
        {
            this.isLaden = isLaden;
        }
    }
}
