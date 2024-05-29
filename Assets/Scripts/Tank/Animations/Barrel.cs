using UnityEngine;

namespace Tank.Animations
{
    public class Barrel : MonoBehaviour
    {
        public TankShooting tankShooting;
        private Animator animator;

        void Start()
        {
            tankShooting.Shot += OnShot;
            animator = GetComponent<Animator>();
        }

        private void OnShot()
        {
            animator.SetTrigger("shoot");
        }
    }
}
