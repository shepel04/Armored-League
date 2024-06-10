using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ball
{
    public class GoalTrigger : MonoBehaviour
    {
        public event Action<Vector3, Vector3> GoalIntoTeamOne;  
        public event Action<Vector3, Vector3> GoalIntoTeamTwo;
        public event Action<int, Vector3, Vector3> GoalIntoTeam; // team index, ball position, goal normal
        public event Action<int> ScoreTeam; // team index


        public TMP_Text ScoredPlayerText;

        private PhotonManagerStadium _photonManagerObject;
        private GameObject _ballProjection;
        private PhotonView _mainPhotonView;
        private Player _lastBlueBallOwner;
        private Player _lastOrangeBallOwner;
        private PhotonView _ballPhotonView;
        public MatchManager _matchManagerInstance;

        void Start()
        {
            _photonManagerObject = GameObject.FindWithTag("PhotonManager").GetComponent<PhotonManagerStadium>();
            _ballProjection = GameObject.FindWithTag("BallProjection");
            _mainPhotonView = GameObject.FindWithTag("PhotonManager").GetComponent<PhotonView>();
            _ballPhotonView = GameObject.FindWithTag("Ball").GetComponent<PhotonView>();
            _matchManagerInstance = GameObject.FindWithTag("PhotonManager").GetComponent<MatchManager>();
        }

        private void FixedUpdate()
        {
            PhotonView.Get(this).RPC("WriteLastBallOwner", RpcTarget.All);
        }

        [PunRPC]
        void WriteLastBallOwner()
        {
            if ((string)_ballPhotonView.Owner.CustomProperties["team"] == "blue")
            {
                _lastBlueBallOwner = _ballPhotonView.Owner;
            }
            else if ((string)_ballPhotonView.Owner.CustomProperties["team"] == "orange")
            {
                _lastOrangeBallOwner = _ballPhotonView.Owner;
            }
            else
            {
                Debug.Log("Team is not assigned");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_photonManagerObject.IsMatchStarted)
            {
                if (other.transform.CompareTag("TeamBlueGoal"))
                {
                    GoalIntoTeamOne?.Invoke(transform.position, other.transform.forward);
                    
                    ScoredPlayerText.text = _lastOrangeBallOwner.NickName + " scored";
                    PhotonView.Get(this).RPC("ShowScoredPlayerForThreeSeconds", RpcTarget.All);
                    
                    //_mainPhotonView.RPC("PauseMatchTimer", RpcTarget.All);
                    TeamScored(true);
                    //_mainPhotonView.RPC("OnGoalScored", RpcTarget.All);
                    
                    _matchManagerInstance.OnGoalScored();
                    
                    GoalIntoTeam?.Invoke(
                        0,
                        transform.position,
                        other.transform.forward);
                    ScoreTeam?.Invoke(1);
                
                    DisableBall();
                }
                else if (other.transform.CompareTag("TeamOrangeGoal"))
                {
                    
                    GoalIntoTeamTwo?.Invoke(transform.position, other.transform.forward);
                    
                    ScoredPlayerText.text = _lastBlueBallOwner.NickName + " scored";
                    PhotonView.Get(this).RPC("ShowScoredPlayerForThreeSeconds", RpcTarget.All);
                    
                    
                    //_mainPhotonView.RPC("PauseMatchTimer", RpcTarget.All);
                    TeamScored(false);
                    
                    _matchManagerInstance.OnGoalScored();
                    
                    GoalIntoTeam?.Invoke(
                        1,
                        transform.position,
                        other.transform.forward);
                    ScoreTeam?.Invoke(0);

                    //_mainPhotonView.RPC("OnGoalScored", RpcTarget.All);
                    DisableBall();
                }
                
                /*if (PhotonNetwork.IsMasterClient)
                {
                    _matchManagerInstance.RespawnManager.RespawnPlayersAndBall(); 
                    _matchManagerInstance.RespawnManager.EnablePlayerControllers(false);
                    _mainPhotonView.RPC("StartSmallMatchCountdown", RpcTarget.All);
                    
                }*/
            }
        }
        
        [PunRPC]
        public void ShowScoredPlayerForThreeSeconds()
        {
            StartCoroutine(ShowTextCoroutine());
        }
        
        private IEnumerator ShowTextCoroutine()
        {
            ScoredPlayerText.gameObject.SetActive(true);

            yield return new WaitForSeconds(3);
            
            ScoredPlayerText.text = String.Empty;

            ScoredPlayerText.gameObject.SetActive(false);
        }

        //[PunRPC]
        private void TeamScored(bool isTeamOne)
        {
            if (isTeamOne)
            {
                ScoreManager.Instance.TeamOneScored();
            }
            else
            {
                ScoreManager.Instance.TeamTwoScored();
            }
        }

        private void DisableBall()
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //gameObject.SetActive(false);
            //_ballProjection.SetActive(false);
        }
    }
}
