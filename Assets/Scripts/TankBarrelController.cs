using Photon.Pun;
using UnityEngine;

public class TankBarrelController : MonoBehaviourPunCallbacks
{
    public Transform TankBodyTransform;

    public Transform CameraTransform;
    public Camera Camera;
    public float MaxRaycastDistance = 50f;

    public float RotationSpeed = 5f;

    public float downRotationAngle = 10.0f;
    public float upRotationAngle = -30.0f;

    public RectTransform ActualShotPoint;

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            RotateBarrel();
        }
    }

    private void RotateBarrel()
    {
        // ----------------------------------------------------------------- raycast area
        RaycastHit hit;
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Physics.Raycast(ray, out hit, MaxRaycastDistance);
        Vector3 cameraHitPoint = hit.point; // camera ray hit point
        
        Vector3 rootPoint = transform.position; // barrel pivot

        Physics.Raycast(rootPoint, transform.forward, out hit, MaxRaycastDistance);
        Vector3 barrelHitPoint = hit.point; // barrel ray hit point

        // ----------------------------------------------------------------- vectors area

        Vector3 desiredVector = cameraHitPoint - rootPoint;
        Vector3 projectedVector = 
            new Vector3(
                cameraHitPoint.x,
                rootPoint.y,
                cameraHitPoint.z) -
            rootPoint;

        Vector3 barrelForward = barrelHitPoint - rootPoint;
        Vector3 barrelForwardHorizontal =
            new Vector3(
                barrelForward.x,
                0,
                barrelForward.z);

        // ----------------------------------------------------------------- raycast debug

        //Debug.DrawRay(rootPoint, desiredVector, Color.cyan, 0.01f);
        //Debug.DrawRay(rootPoint, projectedVector, Color.cyan, 0.01f);
        //Debug.DrawRay(rootPoint, barrelForward, Color.green, 0.01f);
        //Debug.DrawRay(rootPoint, barrelForwardHorizontal, Color.green, 0.01f);

        // ----------------------------------------------------------------- set ui-image actual barrel direction

        Vector3 actualDirectionToScreen = Camera.WorldToScreenPoint(barrelHitPoint);
        ActualShotPoint.position = actualDirectionToScreen;

        // ----------------------------------------------------------------- calculate rotation by X

        float rotationXByDesired = 
            Mathf.Acos(
                (Vector3.Dot(desiredVector, projectedVector)) /
                (desiredVector.magnitude * projectedVector.magnitude)) *
            Mathf.Rad2Deg;
        if (desiredVector.y >= projectedVector.y)
        {
            rotationXByDesired *= -1;
        };
        float rotationXByForward =
            Mathf.Acos(
                (Vector3.Dot(barrelForward, barrelForwardHorizontal)) /
                (barrelForward.magnitude * barrelForwardHorizontal.magnitude)) *
            Mathf.Rad2Deg;
        if (barrelForwardHorizontal.y >= barrelForward.y)
        {
            rotationXByForward *= -1;
        }

        float rotationX = rotationXByForward + rotationXByDesired;

        // angle correction
        float finalRotationX = transform.localRotation.eulerAngles.x + rotationX;
        if (finalRotationX > 180)
        {
            finalRotationX -= 360;
        }

        // ----------------------------------------------------------------- immediate rotation
        //transform.localRotation =
        //    Quaternion.Euler(
        //        Mathf.Clamp(
        //            finalRotationX,
        //            upRotationAngle,
        //            downRotationAngle),
        //        0,
        //        0);

        // can't set close to zero value to avoid error
        if (Mathf.Abs(finalRotationX) > 0.001)
        {
            // ------------------------------------------------------------- interpolated rotation
            transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation,
                Quaternion.Euler(
                    Mathf.Clamp(
                        finalRotationX,
                        upRotationAngle,
                        downRotationAngle),
                    0,
                    0),
                RotationSpeed * Time.fixedDeltaTime);
        }
    }
}
