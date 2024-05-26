using UnityEngine;

public class TurretController : MonoBehaviour
{
    public Transform CameraTransform;
    public Transform TankBodyTransform;
    public float RotationSpeed = 5f;

    void Update()
    {
        RotateTurret();
    }

    void RotateTurret()
    {
        Vector3 directionToTarget = CameraTransform.forward;

        directionToTarget.y = 0;

        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            float bodyXRotation = TankBodyTransform.eulerAngles.x;
            float bodyZRotation = TankBodyTransform.eulerAngles.z;

            Quaternion turretRotation = Quaternion.Euler(bodyXRotation, targetRotation.eulerAngles.y, bodyZRotation);

            transform.rotation = Quaternion.Slerp(transform.rotation, turretRotation, RotationSpeed * Time.deltaTime);
        }
    }
}