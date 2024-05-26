using UnityEngine;

public class TankController : MonoBehaviour
{
    public float MotorTorque = 1000f;
    public float BrakeTorque = 5000f;
    public float MaxSpeed = 60f;
    public float TurnSpeed = 1000f;
    public float TurnInPlaceSpeedMultiplier = 3f;  

    public WheelCollider[] LeftWheels;
    public WheelCollider[] RightWheels;

    public float ForwardFriction = 2.0f;
    public float SidewaysFriction = 2.0f;
    public float TurnForwardFriction = 1.0f;
    public float TurnSidewaysFriction = 0.5f;

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
        bool isHandBrake = Input.GetKey(KeyCode.Space);  // Check if space is pressed for hand brake

        // Speed limit
        float speed = _rb.velocity.magnitude;
        if (speed > MaxSpeed)
        {
            _rb.velocity = _rb.velocity.normalized * MaxSpeed;
        }

        // Forward/back movement
        float currentMotorTorque = moveInput * MotorTorque;

        // Stay rotation
        float leftTorque = currentMotorTorque;
        float rightTorque = currentMotorTorque;

        if (moveInput == 0 && turnInput != 0)
        {
            // stay turn
            float turnAdjustment = turnInput * TurnSpeed * TurnInPlaceSpeedMultiplier;  
            leftTorque = turnAdjustment;
            rightTorque = -turnAdjustment;

            // change friction
            SetWheelFriction(TurnForwardFriction, TurnSidewaysFriction);
        }
        else
        {
            // Reset friction when not turning in place
            SetWheelFriction(ForwardFriction, SidewaysFriction);

            if (turnInput != 0)
            {
                float turnAdjustment = turnInput * TurnSpeed;
                leftTorque += turnAdjustment;
                rightTorque -= turnAdjustment;
            }
        }

        // Correcting the torque for reverse direction
        if (moveInput < 0)
        {
            leftTorque = moveInput * MotorTorque - turnInput * TurnSpeed;
            rightTorque = moveInput * MotorTorque + turnInput * TurnSpeed;
        }

        // Apply forces to left wheels
        foreach (WheelCollider wheel in LeftWheels)
        {
            wheel.motorTorque = leftTorque;
            wheel.brakeTorque = (moveInput == 0 && turnInput == 0) ? BrakeTorque : 0f;
        }

        // Apply forces to right wheels
        foreach (WheelCollider wheel in RightWheels)
        {
            wheel.motorTorque = rightTorque;
            wheel.brakeTorque = (moveInput == 0 && turnInput == 0) ? BrakeTorque : 0f;
        }

        // Fast braking
        if (isHandBrake)
        {
            ApplyBrakes();
        }
        else if (moveInput == 0 && turnInput == 0)
        {
            ApplyBrakes();
        }
        else
        {
            ReleaseBrakes();
        }
    }

    private void ApplyBrakes()
    {
        foreach (WheelCollider wheel in LeftWheels)
        {
            wheel.brakeTorque = BrakeTorque;
        }
        foreach (WheelCollider wheel in RightWheels)
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
        foreach (WheelCollider wheel in RightWheels)
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

        foreach (WheelCollider wheel in RightWheels)
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
}
