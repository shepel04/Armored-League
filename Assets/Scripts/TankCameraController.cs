using UnityEngine;

public class TankCameraController : MonoBehaviour
{
    public Transform Target; 
    public float Distance = 10.0f; 
    public float XSpeed = 150.0f; // horizontal sensitivity
    public float YSpeed = 150.0f; // vertical sensitivity

    public float YMinLimit = -20f;  
    public float YMaxLimit = 80f;  
    public float DistanceMin = 5f;  
    public float DistanceMax = 20f;  

    private float _x = 0.0f;
    private float _y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _x = angles.y;
        _y = angles.x;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    void LateUpdate()
    {
        if (Target)
        {
            _x += Input.GetAxis("Mouse X") * XSpeed * Distance * 0.02f;
            _y -= Input.GetAxis("Mouse Y") * YSpeed * 0.02f;

            // angle camera limit
            _y = ClampAngle(_y, YMinLimit, YMaxLimit);

            // current camera position
            Quaternion rotation = Quaternion.Euler(_y, _x, 0);
            Distance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * 5, DistanceMin, DistanceMax);

            // checking collision with other objects
            RaycastHit hit;
            if (Physics.Linecast(Target.position, transform.position, out hit))
            {
                Distance -= hit.distance;
            }

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -Distance);
            Vector3 position = rotation * negDistance + Target.position;

            transform.rotation = rotation;
            transform.position = position;
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
