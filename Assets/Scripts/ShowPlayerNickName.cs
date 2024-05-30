using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class ShowPlayerNickName : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GetComponent<TMP_Text>().text = photonView.Owner.NickName;
    }

}
