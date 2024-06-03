using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TankShooting : MonoBehaviourPunCallbacks
{
    public event Action Shot; // shooting animation event
    public event Action<float> FireHeld; // ui event
    public event Action<float, float> FireReleased; // ui event

    public float Range = 100f;
    public float FireForce = 500f;
    public float Cooldown = 0.5f;
    public float RecoilForce = 10f;
    
    public float maxPowerHoldTime = 3.0f;
    private float currentHoldTime = 0.0f;

    public Transform FirePoint;
    public Transform Tank;

    private bool _canShoot = true;
    private Rigidbody _tankRigidbody;

    private void Start()
    {
        if (photonView.IsMine)
        {
            _tankRigidbody = Tank.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            InitiateFireLogic();
        }
    }

    private void InitiateFireLogic()
    {
        if (!_canShoot)
            return;

        if (Input.GetButton("Fire1"))
        {
            if (currentHoldTime > maxPowerHoldTime)
                currentHoldTime = maxPowerHoldTime;
            else if (currentHoldTime < maxPowerHoldTime)
                currentHoldTime += Time.deltaTime;

            float ratioFireForce = currentHoldTime / maxPowerHoldTime;
            FireHeld?.Invoke(ratioFireForce); // ui change invocation
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            float ratioFireForce = currentHoldTime / maxPowerHoldTime;
            FireReleased?.Invoke(ratioFireForce, Cooldown); // ui change invocation

            Shoot(ratioFireForce);
            Shot?.Invoke(); // barrel animation trigger

            currentHoldTime = 0.0f;
        }
    }

    private void Shoot(float ratioFireForce)
    {
        Ray ray = new Ray(FirePoint.position, FirePoint.forward);
        RaycastHit hit;

        Debug.DrawRay(
            FirePoint.position,
            FirePoint.forward * Range,
            Color.magenta,
            5.0f);

        // checks ray hit
        if (Physics.Raycast(ray, out hit, Range))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                PhotonView targetPhotonView = hit.collider.GetComponent<PhotonView>();

                // Start a coroutine to handle ownership transfer and force application
                StartCoroutine(TransferOwnershipAndApplyForce(targetPhotonView, hit, rb, ratioFireForce));
            }
        }

        // apply recoil
        _tankRigidbody.AddForce(-transform.forward * RecoilForce, ForceMode.Impulse);

        // cooldown
        StartCoroutine(ShotCooldown());
    }

    private IEnumerator TransferOwnershipAndApplyForce(PhotonView targetPhotonView, RaycastHit hit, Rigidbody rb, float ratioFireForce)
    {
        // Transfer ownership to the current player
        targetPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);

        // Wait for ownership transfer to complete
        yield return new WaitUntil(() => targetPhotonView.Owner == PhotonNetwork.LocalPlayer);

        // Optional: Add a small delay to ensure ownership has propagated
        yield return new WaitForSeconds(0.1f);

        // Apply force after ownership transfer is confirmed
        GiveImpulseByHit(hit, rb, ratioFireForce);
    }

    private void GiveImpulseByHit(RaycastHit hit, Rigidbody rb, float ratioFireForce)
    {
        Vector3 hitByDirection = (hit.point - FirePoint.position).normalized;
        Vector3 hitByNormal = -hit.normal;

        rb.AddForceAtPosition(
            ((hitByDirection + hitByNormal) / 2) * FireForce * ratioFireForce,
            hit.point,
            ForceMode.Impulse);
    }

    IEnumerator ShotCooldown()
    {
        // Set canShoot to false to prevent shooting during cooldown
        _canShoot = false;

        // Wait for the specified cooldown duration
        yield return new WaitForSeconds(Cooldown);

        // Set canShoot back to true to allow shooting again
        _canShoot = true;
    }
    
    [PunRPC]
    void ApplyForceToBall(Vector3 force, Vector3 hitPoint)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        }
    }
}
