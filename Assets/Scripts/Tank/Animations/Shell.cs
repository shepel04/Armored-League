using System.Collections;
using Tank.Shell;
using UnityEngine;

namespace Tank.Animations
{
    public class Shell : MonoBehaviour
    {
        public PoolController poolController;

        public float timeLife;

        public void StartLifeCycle()
        {
            StartCoroutine(SimulateLife());
        }

        private IEnumerator SimulateLife()
        {
            yield return new WaitForSeconds(timeLife);

            poolController.DeactivateInstance(gameObject);
        }
    }
}
