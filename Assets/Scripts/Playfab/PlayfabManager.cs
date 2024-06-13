using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Photon.Pun;
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
    
    public static PlayfabManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
        
        SceneManager.LoadScene(1); // load main menu
    }
    
    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string nickname = result.AccountInfo.TitleInfo.DisplayName;
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = result.AccountInfo.Username;
        }

       PhotonNetwork.NickName = nickname;
       
       Debug.Log("Player nickname: " + PhotonNetwork.NickName);
    }
    
    void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("Information error: " + error.GenerateErrorReport());
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
    
    public void UpdatePlayerScore(int score)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new System.Collections.Generic.Dictionary<string, string>
            {
                {"PlayerScore", score.ToString()}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successfully updated player score.");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error while updating player score: " + error.GenerateErrorReport());
    }
    
    public void GetPlayerScore(System.Action<int> onSuccess)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result =>
        {
            if (result.Data != null && result.Data.ContainsKey("PlayerScore"))
            {
                int currentScore = int.Parse(result.Data["PlayerScore"].Value);
                Debug.Log("Current player score: " + currentScore);
                onSuccess(currentScore);
            }
            else
            {
                onSuccess(0);
            }
        }, OnError);
    }
}
