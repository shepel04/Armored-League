using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDataHandler : MonoBehaviour
{
    public TMP_Text NicknameText; 

    void Start()
    {
        GetPlayerNickname();
    }

    void GetPlayerNickname()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
    }

    void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        string nickname = result.AccountInfo.TitleInfo.DisplayName;
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = result.AccountInfo.Username;
        }

        NicknameText.text = nickname;
    }

    void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("Information error: " + error.GenerateErrorReport());
    }
}