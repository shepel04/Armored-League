using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Ball
{
    public class BallProjection : MonoBehaviour
    {
        void Start()
        {
            GetComponent<PositionConstraint>()
                .AddSource(
                    new ConstraintSource()
                    {
                        sourceTransform = GameObject.FindWithTag("Ball").transform,
                        weight = 1
                    });
        }
    }
}
