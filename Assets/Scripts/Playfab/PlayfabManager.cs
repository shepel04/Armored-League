using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;

public class PlayfabManager : MonoBehaviour
{
    public TMP_InputField PlayerNick;
    public TMP_InputField PlayerPassword;
    public TMP_Text FeedbackText;
    
    // public void RegisterPlayer()
    // {
    //     if (string.IsNullOrEmpty(PlayerNick.text) || string.IsNullOrEmpty(PlayerPassword.text))
    //     {
    //         FeedbackText.text = "Please enter both username and password.";
    //         return;
    //     }
    //
    //     var request = new RegisterPlayFabUserRequest
    //     {
    //         Username = username,
    //         Password = password,
    //         RequireBothUsernameAndEmail = false
    //     };
    //
    //     PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    // }

    
}
