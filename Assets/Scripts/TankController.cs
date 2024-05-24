using UnityEngine;
using UnityEngine.Serialization;

public class TankController : MonoBehaviour
{
    public float MotorTorque = 1000f;         
    public float BrakeTorque = 5000f;        
    public float MaxSpeed = 60f;             
    public float TurnSpeed = 1000f;           

    public WheelCollider[] LeftWheels;        
    public WheelCollider[] RightWheelColliders; 
    
    //public Animator leftTrackAnimator;
    //public Animator rightTrackAnimator;

    public float ForwardFriction = 1.0f;      
    public float SidewaysFriction = 1.0f;     

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        SetWheelFriction(ForwardFriction, SidewaysFriction);
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");  
        float turnInput = Input.GetAxis("Horizontal");  

        // speed limit
        float speed = _rb.velocity.magnitude;
        if (speed > MaxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * MaxSpeed;
        }

        // forward/back movement
        float currentMotorTorque = moveInput * MotorTorque;

        // stay rotation
        float leftTorque = currentMotorTorque;
        float rightTorque = currentMotorTorque;

        if (turnInput != 0)
        {
            float turnAdjustment = turnInput * TurnSpeed;
            leftTorque += turnAdjustment * Mathf.Sign(moveInput);
            rightTorque -= turnAdjustment * Mathf.Sign(moveInput);
        }

        // apply forces to left wheels
        foreach (WheelCollider wheel in LeftWheels)
        {
            wheel.motorTorque = leftTorque;
            wheel.brakeTorque = (moveInput == 0 && turnInput == 0) ? BrakeTorque : 0f;
        } 
        
        // apply forces to right wheels
        foreach (WheelCollider wheel in RightWheelColliders)
        {
            wheel.motorTorque = rightTorque;
            wheel.brakeTorque = (moveInput == 0 && turnInput == 0) ? BrakeTorque : 0f;
        }

        // fast braking
        if (moveInput == 0 && turnInput == 0)
        {
            ApplyBrakes();
        }
        else
        {
            ReleaseBrakes();
        }
        
        //UpdateTrackAnimations(moveInput, turnInput);
    }

    private void ApplyBrakes()
    {
        foreach (WheelCollider wheel in LeftWheels)
        {
            wheel.brakeTorque = BrakeTorque;
        }
        foreach (WheelCollider wheel in RightWheelColliders)
        {
            wheel.brakeTorque = BrakeTorque;
        }
    }

    private void ReleaseBrakes()
    {
        foreach (WheelCollider wheel in LeftWheels)
        {
            wheel.brakeTorque = 0f;
        }
        foreach (WheelCollider wheel in RightWheelColliders)
        {
            wheel.brakeTorque = 0f;
        }
    }

    private void SetWheelFriction(float forwardFriction, float sidewaysFriction)
    {
        foreach (WheelCollider wheel in LeftWheels)
        {
            WheelFrictionCurve forward = wheel.forwardFriction;
            forward.extremumSlip = forwardFriction;
            forward.asymptoteSlip = forwardFriction;
            wheel.forwardFriction = forward;

            WheelFrictionCurve sideways = wheel.sidewaysFriction;
            sideways.extremumSlip = sidewaysFriction;
            sideways.asymptoteSlip = sidewaysFriction;
            wheel.sidewaysFriction = sideways;
        }

        foreach (WheelCollider wheel in RightWheelColliders)
        {
            WheelFrictionCurve forward = wheel.forwardFriction;
            forward.extremumSlip = forwardFriction;
            forward.asymptoteSlip = forwardFriction;
            wheel.forwardFriction = forward;

            WheelFrictionCurve sideways = wheel.sidewaysFriction;
            sideways.extremumSlip = sidewaysFriction;
            sideways.asymptoteSlip = sidewaysFriction;
            wheel.sidewaysFriction = sideways;
        }
    }
    
    // private void UpdateTrackAnimations(float moveInput, float turnInput)
    // {
    //     if (moveInput != 0 || turnInput != 0)
    //     {
    //         leftTrackAnimator.SetBool("isMoving", true);
    //         rightTrackAnimator.SetBool("isMoving", true);
    //
    //         leftTrackAnimator.SetFloat("Speed", moveInput + turnInput);
    //         rightTrackAnimator.SetFloat("Speed", moveInput - turnInput);
    //     }
    //     else
    //     {
    //         leftTrackAnimator.SetBool("isMoving", false);
    //         rightTrackAnimator.SetBool("isMoving", false);
    //     }
    // }
}
