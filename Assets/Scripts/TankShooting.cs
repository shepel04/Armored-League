using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class TankShooting : MonoBehaviourPunCallbacks
{
    public event Action Shot; // shooting animation event

    public float Range = 100f;  
    public float FireForce = 500f;   
    public float Cooldown = 0.5f;  
    public float RecoilForce = 10f;   
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
            if (Input.GetButtonDown("Fire1") && _canShoot)
            {
                Shoot();
                Shot?.Invoke(); // barrel animation trigger
            }
        }
    }

    void Shoot()
    {
        Ray ray = new Ray(FirePoint.position, FirePoint.forward);
        RaycastHit hit;

        Debug.DrawRay(FirePoint.position, FirePoint.forward * Range, Color.red, 2.0f);

        // checks ray hit
        if (Physics.Raycast(ray, out hit, Range))
        {
            Debug.Log("Hit: " + hit.collider.name);

            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 forceDirection = hit.point - FirePoint.position;
                forceDirection.Normalize();

                rb.AddForceAtPosition(forceDirection * FireForce, hit.point, ForceMode.Impulse);
            }
        }

        // apply recoil
        
        _tankRigidbody.AddForce(-transform.forward * RecoilForce, ForceMode.Impulse);

        // cooldown
        StartCoroutine(ShotCooldown());
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
