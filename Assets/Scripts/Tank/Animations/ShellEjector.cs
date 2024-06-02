using Random = System.Random;
using UnityEngine;
using Tank.Shell;

namespace Tank.Animations
{
    public class ShellEjector : MonoBehaviour
    {
        private Random random = new Random();
        private Animator animator;

        public Transform leftEjectionEmpty;
        public Transform rightEjectionEmpty;

        public float shellMass = 0.1f;
        public float shellDrag = 0.0f;
        public float shellAngularDrag = 2.0f;

        public float ejectionForce = 1; 

        private int sideIndex;

        public PoolController poolController;

        private GameObject shellInstance;

        public Transform turret;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void OnShotFinished()
        {
            sideIndex = random.Next(1, 3);

            animator.SetTrigger("open" + sideIndex.ToString());
        }
        
        // used by animation
        public void InstantiateShell()
        {
            shellInstance = poolController.GetInstance();

            if (sideIndex == 1)
                shellInstance.transform.SetParent(leftEjectionEmpty);
            else
                shellInstance.transform.SetParent(rightEjectionEmpty);

            shellInstance.transform.localPosition = Vector3.zero;
            shellInstance.transform.localRotation = Quaternion.identity;
        }
        // used by animation
        public void Ejection()
        {
            // enable rigidbody physics
            Rigidbody rb = shellInstance.AddComponent<Rigidbody>();
            rb.mass = shellMass;
            rb.drag = shellDrag;
            rb.angularDrag = shellAngularDrag;

            // remove from parent
            shellInstance.transform.parent = null;

            // add impulse
            Vector3 vectorSide;
            if (sideIndex == 1)
                vectorSide = Vector3.left;
            else
                vectorSide = Vector3.right;
            rb.AddRelativeForce(
                Quaternion.Euler(0, turret.transform.localRotation.y, 0) * vectorSide * ejectionForce,
                ForceMode.Impulse);
        }
    }
}
