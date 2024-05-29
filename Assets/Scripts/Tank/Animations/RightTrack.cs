using UnityEngine;

namespace Tank.Animations
{
    public class RightTrack : MonoBehaviour
    {
        public TankController tankController;
        private Animator animator;

        void Start()
        {
            tankController.RightTrackMoved += OnTrackMoved;
            animator = GetComponent<Animator>();
        }

        private void OnTrackMoved(float rpm)
        {
            float rpmDefault = 24;

            float ratio = rpm / rpmDefault;

            animator.SetFloat("speedRatio", ratio);
        }
    }
}