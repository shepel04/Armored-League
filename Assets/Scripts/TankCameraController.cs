using Photon.Pun;
using UnityEngine;

public class TankCameraController : MonoBehaviourPunCallbacks
{
    public Transform Target; 
    public float Distance = 10.0f; 
    public float XSpeed = 150.0f; // horizontal sensitivity
    public float YSpeed = 150.0f; // vertical sensitivity

    public float YMinLimit = -20f;  
    public float YMaxLimit = 80f;  
    public float DistanceMin = 5f;  
    public float DistanceMax = 20f;

    [Tooltip("Whether to apply dynamic distance adjustment for camera based on objects between camera and its target")]
    public bool isDynamicCamera;

    private float _x = 0.0f;
    private float _y = 0.0f;

    void Start()
    {
        if (photonView.IsMine)
        {
            Vector3 angles = transform.eulerAngles;
            _x = angles.y;
            _y = angles.x;

            if (GetComponent<Rigidbody>())
            {
                GetComponent<Rigidbody>().freezeRotation = true;
            }
        }
        else
        {
            // disable camera
        }
    }

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            if (Target)
            {
                _x += Input.GetAxis("Mouse X") * XSpeed * Distance * 0.02f;
                _y -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;

                // angle camera limit
                _y = ClampAngle(_y, YMinLimit, YMaxLimit);

                // handle distance and collisions
                Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * 5, DistanceMin, DistanceMax);

                // calculate desired position
                Quaternion rotation = Quaternion.Euler(_y, _x, 0);
                Vector3 desiredPosition = rotation * new Vector3(0.0f, 0.0f, -Distance) + Target.position;

                // check for collisions
                RaycastHit hit;
                if (Physics.Linecast(Target.position, desiredPosition, out hit))
                {
                    // set distance to the point of collision
                    Distance = Vector3.Distance(Target.position, hit.point) - 0.2f; // subtract a small offset to prevent clipping
                }

                // recalculate position based on the new distance
                Vector3 finalPosition = rotation * new Vector3(0.0f, 0.0f, -Distance) + Target.position;

                // update camera position and rotation
                transform.rotation = rotation;
                transform.position = finalPosition;
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
