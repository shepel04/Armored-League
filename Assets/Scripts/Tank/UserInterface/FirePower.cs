using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Tank.UserInterface
{
    public class FirePower : MonoBehaviour
    {
        public TankShooting tankShooting;

        public Slider powerIndicator;
        public Image powerImage;
        public Color accumulationColor;
        public Color cooldownColor;

        //private float timeCooldown;
        //private float currenTimeWaitCooldown = 0.0f;
        //private float ratioPower;

        private bool isPowerAccumulating = false;
        public int stepsFullOverall = 10;
        //private bool isCoolingDown = false;

        void Start()
        {
            tankShooting.FireHeld += OnFireHold;
            tankShooting.FireReleased += OnFireRelease;
        }

        void Update()
        {
            //if (isCoolingDown)
            //{
            //    if (currenTimeWaitCooldown >= timeCooldown)
            //    {
            //        isCoolingDown = false;
            //        currenTimeWaitCooldown = 0.0f;
            //    }
            //    else
            //    {
            //        currenTimeWaitCooldown += Time.deltaTime;

            //        float ratioOnUpdate = ratioPower * (Time.deltaTime / timeCooldown);
            //        powerIndicator.value -= ratioOnUpdate;
            //    }
            //}
        }

        private void OnFireHold(float ratioPower)
        {
            if (!isPowerAccumulating)
            {
                powerImage.color = accumulationColor;

                isPowerAccumulating = true;
            }

            powerIndicator.value = ratioPower;
        }

        private void OnFireRelease(float ratioPower, float timeCooldown)
        {
            if (isPowerAccumulating)
            {
                powerImage.color = cooldownColor;

                isPowerAccumulating = false;
                //isCoolingDown = true;

                //this.timeCooldown = timeCooldown;
                //this.ratioPower = ratioPower;
            }

            if (ratioPower < 0.01f)
                ratioPower = 0.01f;

            StartCoroutine(
                PowerIndicatorDecrement(
                    ratioPower,
                    timeCooldown));
        }

        private IEnumerator PowerIndicatorDecrement(
            float ratioPower,
            float timeCooldown)
        {
            //calculations area
            int fullStepsCurrent = (int)(ratioPower / (1.0f / stepsFullOverall));

            float ratioRemainder = ratioPower % (1.0f / stepsFullOverall);
            float ratioFullStep = 1.0f / stepsFullOverall;

            float timeRemainder = timeCooldown * (ratioRemainder / ratioPower);
            float timeFullStep = timeCooldown * (ratioFullStep / ratioPower);

            // waiting area
            float r = ratioPower;

            yield return new WaitForSeconds(timeRemainder);
            r -= ratioRemainder;
            powerIndicator.value = r;

            for (int i = 0; i < fullStepsCurrent; i++)
            {
                yield return new WaitForSeconds(timeFullStep);
                r -= ratioFullStep;
                powerIndicator.value = r;
            }
        }
    }
}
