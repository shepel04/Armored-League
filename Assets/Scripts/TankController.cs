using Photon.Pun;
using System;
using UnityEngine;

public class TankController : MonoBehaviourPunCallbacks
{
    public event Action<bool> EngineLaden; // engine radiators animation event

    public event Action<float> LeftTrackMoved; // left track animation event
    public event Action<float> RightTrackMoved; // right track animation event

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
    public bool _isControllerEnabled = true;

    void Start()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Mine");
            _rb = GetComponent<Rigidbody>();
            SetWheelFriction(ForwardFriction, SidewaysFriction);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float moveInput = 0;
            float turnInput = 0;
            bool isHandBrake = false;
            
            if (_isControllerEnabled)
            {
                moveInput = Input.GetAxis("Vertical");
                turnInput = Input.GetAxis("Horizontal");
                isHandBrake = Input.GetKey(KeyCode.Space);
            }
            

            // radiator animation influencer
            if (moveInput != 0 || turnInput != 0)
            {
                EngineLaden?.Invoke(true);
            }
            else
            {
                EngineLaden?.Invoke(false);
            }

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
            float rpmAverageLeft = 0.0f;
            foreach (WheelCollider wheel in LeftWheels)
            {
                wheel.motorTorque = leftTorque;
                wheel.brakeTorque = (moveInput == 0 && turnInput == 0) ? BrakeTorque : 0f;

                rpmAverageLeft += wheel.rpm;
            }
            rpmAverageLeft /= 4.0f;
            LeftTrackMoved?.Invoke(rpmAverageLeft); // event invocation for animation adjusting

            // Apply forces to right wheels
            float rpmAverageRight = 0.0f;
            foreach (WheelCollider wheel in RightWheels)
            {
                wheel.motorTorque = rightTorque;
                wheel.brakeTorque = (moveInput == 0 && turnInput == 0) ? BrakeTorque : 0f;

                rpmAverageRight += wheel.rpm;
            }
            rpmAverageRight /= 4.0f;
            RightTrackMoved?.Invoke(rpmAverageRight); // event invocation for animation adjusting

            // Fast braking
            if (isHandBrake)
            {
                ApplyBrakes();
            }
            else if (moveInput == 0 && turnInput == 0)
            {
                //ApplyBrakes();
                ReleaseBrakes();
            }
            else
            {
                ReleaseBrakes();
            }
        }
    }

    public void StopTank()
    {
        foreach (WheelCollider wheel in LeftWheels)
        {
            wheel.motorTorque = 0f;
            wheel.brakeTorque = float.MaxValue;
            wheel.rotationSpeed = 0f;
        }
        foreach (WheelCollider wheel in RightWheels)
        {
            wheel.motorTorque = 0f;
            wheel.brakeTorque = float.MaxValue;
            wheel.rotationSpeed = 0f;
        }
    }

    public void ApplyBrakes()
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
