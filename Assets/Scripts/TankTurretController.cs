using UnityEngine;

public class TankTurretController : MonoBehaviour
{
    public Transform Turret;
    public Camera MainCamera;
    public float RotationSpeed = 100f;  

    void LateUpdate()
    {
        Vector3 cameraDirection = MainCamera.transform.forward;
        cameraDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(cameraDirection);

        Turret.rotation = Quaternion.RotateTowards(Turret.rotation, targetRotation, RotationSpeed * Time.deltaTime * 50f);
    }
}