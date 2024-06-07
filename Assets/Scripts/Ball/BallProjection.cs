using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;

namespace Ball
{
    public class BallProjection : MonoBehaviour
    {
        public Material[] materials;
        private bool isTeamOneColorSet = false;
        private bool isTeamTwoColorSet = false;

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

        void FixedUpdate()
        {
            if (transform.position.z < 0)  // team1 materail
            {
                if (!isTeamOneColorSet)
                {
                    GetComponent<DecalProjector>().material = materials[0];
                    isTeamOneColorSet = true;
                    isTeamTwoColorSet = false;
                }
            }
            else if (transform.position.z > 0)  // team2 material
            {
                if (!isTeamTwoColorSet)
                {
                    GetComponent<DecalProjector>().material = materials[1];
                    isTeamOneColorSet = false;
                    isTeamTwoColorSet = true;
                }
            }
        }
    }
}
