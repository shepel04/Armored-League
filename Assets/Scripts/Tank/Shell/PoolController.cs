using System.Collections.Generic;
using UnityEngine;

namespace Tank.Shell
{
    public class PoolController : MonoBehaviour
    {
        //public static PoolController sharedController;
        //public --vvv--
        private static List<GameObject> pool;
        public GameObject prefabToPool;
        public int amountInitial;

        //void Awake()
        //{
        //    sharedController = this;
        //}

        void Start()
        {
            pool = new List<GameObject>();

            for (int i = 0; i < amountInitial; i++)
            {
                AddInstanceToPool();
            }
        }

        private GameObject AddInstanceToPool()
        {
            GameObject instance = Instantiate(prefabToPool);

            instance.SetActive(false);
            pool.Add(instance);

            return instance;
        }

        public GameObject GetInstance()
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].activeInHierarchy)
                {
                    ActivateInstance(pool[i]);
                    return pool[i];
                }
            }

            GameObject instance = AddInstanceToPool();
            ActivateInstance(instance);
            return instance;
        }

        private void ActivateInstance(GameObject instance)
        {
            instance.SetActive(true);
            instance.GetComponent<Tank.Animations.Shell>().StartLifeCycle();
        }

        public void DeactivateInstance(GameObject instance)
        {
            instance.SetActive(false);

            Destroy(instance.GetComponent<Rigidbody>());
        }
    }
}
