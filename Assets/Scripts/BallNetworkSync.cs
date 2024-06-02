using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody))]
public class BallNetworkSync : MonoBehaviour, IPunObservable
{
    private Rigidbody _rb;
    private PhotonView _photonView;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rb.position);
            stream.SendNext(_rb.velocity);
        }
        else
        {
            _rb.position = (Vector3)stream.ReceiveNext();
            _rb.velocity = (Vector3)stream.ReceiveNext();
        }
    }
}