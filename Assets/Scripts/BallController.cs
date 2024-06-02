using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class BallController : MonoBehaviourPun
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            base.photonView.RequestOwnership();
            Debug.Log("Owner is: " + collision.gameObject.name);
        }
    }
}