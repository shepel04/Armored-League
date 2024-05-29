using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tank.Animations
{
    public class Shell : MonoBehaviour
    {
        public float timeLife;
        private float timeCurrent;

        void FixedUpdate()
        {
            timeCurrent += Time.fixedDeltaTime;
            if (timeCurrent >= timeLife)
            {
                Destroy(gameObject);
                // move to pool instead of destroyment -------------------------------- !!!
                // btw, you need a pool implementation first ... ---------------------- !!!
            }
        }
    }
}
