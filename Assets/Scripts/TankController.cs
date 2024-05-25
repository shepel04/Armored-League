using UnityEngine;

public class TankController : MonoBehaviour
{
    public float MotorTorque = 1000f;
    public float BrakeTorque = 5000f;
    public float MaxSpeed = 60f;
    public float TurnSpeed = 1000f;
    public float TurnInPlaceSpeedMultiplier = 3f; // Змінна для множника швидкості повороту на місці

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

            // Zmenshuye friktsiyu pid chas povorotu na mistsi
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
        if (moveInput == 0 && turnInput == 0)
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
