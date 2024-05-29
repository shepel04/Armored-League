using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    [Header("RegisterCanvas")]
    public TMP_InputField PlayerNickReg;
    public TMP_InputField PlayerPasswordReg;
    public TMP_Text FeedbackTextReg;
    
    [Header("LoginCanvas")]
    public TMP_InputField PlayerNickLogin;
    public TMP_InputField PlayerPasswordLogin;
    public TMP_Text FeedbackTextLogin;

    [Header("Canvases")] 
    public GameObject RegisterCanv;
    public GameObject LoginCanv;


    private bool _isPasswordVisible;
    public void RegisterPlayer()
    {
        if (string.IsNullOrEmpty(PlayerNickReg.text) || string.IsNullOrEmpty(PlayerPasswordReg.text))
        {
            FeedbackTextReg.text = "Please enter both username and password.";
            return;
        }
    
        var request = new RegisterPlayFabUserRequest
        {
            Username = PlayerNickReg.text,
            Password = PlayerPasswordReg.text,
            RequireBothUsernameAndEmail = false
        };
    
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }
    
    public void LoginPlayer()
    {
        if (string.IsNullOrEmpty(PlayerNickLogin.text) || string.IsNullOrEmpty(PlayerPasswordLogin.text))
        {
            FeedbackTextLogin.text = "Please enter both username and password.";
            return;
        }

        var request = new LoginWithPlayFabRequest
        {
            Username = PlayerNickLogin.text,
            Password = PlayerPasswordLogin.text
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        SwitchLogCanvas();
        //load menu or login page
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        FeedbackTextReg.text = "Registration failed: " + error.ErrorMessage;
    }
    
    private void OnLoginSuccess(LoginResult result)
    {
        SceneManager.LoadScene(1); // load main menu
    }

    private void OnLoginFailure(PlayFabError error)
    {
        FeedbackTextLogin.text = "Login failed: " + error.ErrorMessage;
    }

    public void SwitchLogCanvas()
    {
        LoginCanv.SetActive(!LoginCanv.activeSelf);
        RegisterCanv.SetActive(!RegisterCanv.activeSelf);
    }

    public void PasswordVisibility(TMP_InputField field)
    {
        if (!_isPasswordVisible)
        {
            field.contentType = TMP_InputField.ContentType.Standard;
            _isPasswordVisible = true;
        }
        else
        {
            field.contentType = TMP_InputField.ContentType.Password;
            _isPasswordVisible = false;
        }
    }
}
