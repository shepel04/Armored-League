using UnityEngine;

namespace Tank.Effects
{
    public class ShootEffect : MonoBehaviour
    {
        public GameObject shootPrefab;
        public Transform targetTransform;

        private void OnShoot()
        {
            shootPrefab.transform.position = targetTransform.position;
            shootPrefab.transform.up = targetTransform.forward;

            Instantiate(shootPrefab);
        }
    }
}
