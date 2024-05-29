using UnityEngine;

public class SmoothCameraTransition : MonoBehaviour
{
    public Transform StartTransform;  
    public Transform EndTransform;  
    public float MoveSpeed = 1.0f;  

    private bool _isMoving;  
    private float _startTime;  
    private Transform _originalEndTransform;  

    void Start()
    {
        _originalEndTransform = EndTransform;
    }

    public void StartMoving()
    {
        _startTime = Time.time;
        _isMoving = true;
    }

    void Update()
    {
        if (_isMoving)
        {
            float JourneyLength = Vector3.Distance(StartTransform.position, _originalEndTransform.position); // Distance to travel
            float DistanceCovered = (Time.time - _startTime) * MoveSpeed; // Distance covered 
            float FracJourney = DistanceCovered / JourneyLength; // Percentage of journey completed

            transform.position = Vector3.Lerp(StartTransform.position, _originalEndTransform.position, FracJourney);  
            transform.rotation = Quaternion.Slerp(StartTransform.rotation, _originalEndTransform.rotation, FracJourney);

            // stop
            if (DistanceCovered >= JourneyLength)
            {
                _isMoving = false;
                // swap
                Transform TempTransform = StartTransform;
                StartTransform = _originalEndTransform;
                _originalEndTransform = TempTransform;
            }
        }
    }
}