using Photon.Pun;
using System;
using System.Collections;
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

    // jump section
    private ConstantForce _cf;
    private bool isJumpState = false;
    private bool isReadyToJump = true;
    private bool isAllowedGroundCheck = true;
    [Min(1)]
    public float jumpCooldown;
    [Range(1, 5)]
    public float rotationInJumpSensitivity;
    public float jumpForce;
    //private float lastInAirYRotation = 0.0f; // temp variable
    //private float currentInAirYRotation = 0.0f; // temp variable

    void Start()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Mine");
            _rb = GetComponent<Rigidbody>();
            _cf = GetComponent<ConstantForce>();
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
            bool isJump = false;
            
            if (_isControllerEnabled)
            {
                moveInput = Input.GetAxis("Vertical");
                turnInput = Input.GetAxis("Horizontal");
                isHandBrake = Input.GetKey(KeyCode.Space);
                isJump = Input.GetKey(KeyCode.Mouse1);
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

            // jump logic
            if (isJump && isReadyToJump && !isJumpState)
            {
                StartCoroutine(InitiateJump());
            }
            if (isJumpState && isAllowedGroundCheck && WhetherOnGround())
            {
                CompleteJump();
            }
            if (!WhetherOnGround())
            {
                StabilizeAirRotation();
                if (turnInput != 0)
                {
                    ApplyTurnInAir(turnInput);
                }
            }
        }
    }

    private IEnumerator InitiateJump()
    {
        isAllowedGroundCheck = false;
        isReadyToJump = false;
        isJumpState = true;
        
        UseFakeGravity(true);
        StabilizeRigidBodyStart();
        ApplyJumpStartForce();

        yield return new WaitForSeconds(0.5f); // delay to gain some height
        isAllowedGroundCheck = true;

        yield return new WaitForSeconds(jumpCooldown - 0.5f);
        isReadyToJump = true;
    }
    private void CompleteJump()
    {
        isJumpState = false;
        UseFakeGravity(false);
    }
    private bool WhetherOnGround()
    {
        bool result = false;
        RaycastHit hit;
        result = Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f);
        return
            result &&
            hit.transform.CompareTag("Stadium");
    }
    private void ApplyJumpStartForce()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    private void UseFakeGravity(bool isEnabled)
    {
        _rb.useGravity = !isEnabled;
        _cf.enabled = isEnabled;
    }
    private void StabilizeAirRotation()
    {
        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(
                    0.0f,
                    transform.rotation.eulerAngles.y,
                    0.0f),
                0.2f);
    }
    private void StabilizeRigidBodyStart()
    {
        _rb.angularVelocity = Vector3.zero;

        _rb.velocity = new Vector3(
            _rb.velocity.normalized.x,
            0.0f,
            _rb.velocity.normalized.z) * 
            _rb.velocity.magnitude;
    }
    private void ApplyTurnInAir(float turnInput)
    {
        //lastInAirYRotation = transform.rotation.eulerAngles.y;

        transform.rotation *=
            Quaternion.Euler(
                0.0f,
                turnInput * rotationInJumpSensitivity,
                0.0f);

        //currentInAirYRotation = transform.rotation.eulerAngles.y;

        //_rb.velocity =
        //    Quaternion.Euler(
        //        0.0f,
        //        currentInAirYRotation - lastInAirYRotation,
        //        0.0f) *
        //    _rb.velocity;
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
