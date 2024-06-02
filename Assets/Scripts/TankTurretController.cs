using System;
using Photon.Pun;
using UnityEngine;

public class TurretController : MonoBehaviourPunCallbacks
{
    public Transform CameraTransform;
    public Transform TankBodyTransform;
    public float RotationSpeed = 5f;

    private float lastBodyRotationY;

    void Start()
    {
        lastBodyRotationY = TankBodyTransform.rotation.eulerAngles.y;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            RotateTurret();
        }
    }

    void RotateTurret()
    {
        float currentBodyRotationY = TankBodyTransform.rotation.eulerAngles.y;

        // set turret rotation correction by body rotation
        transform.localRotation =
            Quaternion.Euler(
                0,
                transform.localRotation.eulerAngles.y -
                    (currentBodyRotationY - lastBodyRotationY),
                0);

        lastBodyRotationY = currentBodyRotationY;

        // set interpolated turret rotation
        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                Quaternion.Euler(
                    0,
                    CameraTransform.rotation.eulerAngles.y -
                        currentBodyRotationY,
                    0),
                RotationSpeed * Time.fixedDeltaTime);

        //transform.localRotation =
        //    Quaternion.Euler(
        //        0,
        //        CameraTransform.rotation.eulerAngles.y -
        //            currentBodyRotationY,
        //        0);
    }
}
