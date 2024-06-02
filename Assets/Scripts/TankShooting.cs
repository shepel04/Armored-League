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
            FireHeld?.Invoke(ratioFireForce); // ui change invokation
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            float ratioFireForce = currentHoldTime / maxPowerHoldTime;
            FireReleased?.Invoke(ratioFireForce, Cooldown); // ui change invokation

            Shoot(ratioFireForce);
            Shot?.Invoke(); // barrel animation trigger

            currentHoldTime = 0.0f;
        }
    }

    private void Shoot(float ratioFireForce)
    {
        Ray ray = new Ray(FirePoint.position, FirePoint.forward);
        RaycastHit hit;

        //Array values = Enum.GetValues(typeof(Color));
        //Random random = new Random();
        //int randomIndex = random.Next(values.Length);
        //(Color)values.GetValue(randomIndex)

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
                GiveImpulseByHit(hit, rb, ratioFireForce);
            }
        }

        // apply recoil
        _tankRigidbody.AddForce(-transform.forward * RecoilForce, ForceMode.Impulse);

        // cooldown
        StartCoroutine(ShotCooldown());
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
}
