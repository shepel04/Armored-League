using Random = System.Random;
using UnityEngine;

namespace Tank.Animations
{
    public class ShellEjector : MonoBehaviour
    {
        private Random random = new Random();
        private Animator animator;

        public Transform leftEjectionEmpty;
        public Transform rightEjectionEmpty;

        private int sideIndex;

        public GameObject shellPrefab;

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
            if (sideIndex == 1)
                shellInstance = Instantiate(shellPrefab, leftEjectionEmpty);
            else
                shellInstance = Instantiate(shellPrefab, rightEjectionEmpty);
        }
        // used by animation
        public void Ejection()
        {
            // add rigidbody
            Rigidbody rb = shellInstance.AddComponent<Rigidbody>();
            rb.mass = 1.0f;
            rb.drag = 0.0f;
            rb.angularDrag = 2.0f;

            // remove from parent
            shellInstance.transform.parent = null;

            // add impulse
            Vector3 vectorSide;
            if (sideIndex == 1)
                vectorSide = Vector3.left;
            else
                vectorSide = Vector3.right;
            rb.AddRelativeForce(
                Quaternion.Euler(0, turret.transform.localRotation.y, 0) * vectorSide * 10,
                ForceMode.Impulse);
        }
    }
}
