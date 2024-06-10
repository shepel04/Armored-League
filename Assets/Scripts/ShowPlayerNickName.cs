using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.Animations;

public class ShowPlayerNickName : MonoBehaviourPunCallbacks
{
    private TMP_Text _nick;
    private GameObject _localTank;

    void Start()
    {
        _nick = GetComponent<TMP_Text>();
        _nick.text = photonView.Owner.NickName;

        UpdateNickColor();

        if (photonView.IsMine)
        {
            _localTank = gameObject;
            StartCoroutine(UpdateNicknamesLookAt());
        }
    }

    private void UpdateNickColor()
    {
        var player = GetComponentInParent<PhotonView>().Owner;
        
        _nick.color = (string)player.CustomProperties["team"] == "blue" ? Color.blue : new Color(1f, 0.5f, 0f);
        
        
        /*var playerSetup = GetComponentInParent<PlayerSetup>();
        if (playerSetup != null)
        {
            _nick.color = playerSetup.PlayerTeam == "blue" ? Color.blue : new Color(1f, 0.5f, 0f);
        }*/
    }

    private IEnumerator UpdateNicknamesLookAt()
    {
        while (true)
        {
            //need look at local player camera
            GameObject[] nickObjects = GameObject.FindGameObjectsWithTag("PlayerNick");

            foreach (GameObject nickObject in nickObjects)
            {
                if (nickObject != null && nickObject != _nick.gameObject)
                {
                    nickObject.transform.LookAt(_localTank.transform);
                    nickObject.transform.Rotate(0, 180, 0);
                }
            }

            yield return new WaitForSeconds(0.1f); // Update every 0.1 seconds
        }
    }
}