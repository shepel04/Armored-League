using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerLeague : MonoBehaviour
{
    public string CurrentPlayerLeague;
    public TMP_Text PlayerLeagueText;
    public TMP_Text PlayerLeagueText2;
    public TMP_Text PlayerLeagueText3;
    
    private int _playerPoints;


    private void Start()
    {
        PlayfabManager.Instance.GetPlayerScore(score =>
        {
            Debug.Log("Player's current score: " + score);
            _playerPoints = score;

            AssignLeague(_playerPoints);

            PlayerLeagueText.text = CurrentPlayerLeague;

            if (PlayerLeagueText2 != null && PlayerLeagueText3 != null)
            {
                PlayerLeagueText2.text = CurrentPlayerLeague;
                PlayerLeagueText3.text = CurrentPlayerLeague;
            }
        });
    }

    void AssignLeague(int points)
    {
        if (points >= 0 && points < 10)
        {
            CurrentPlayerLeague = "Rookie League";
        }
        else if (points >= 10 && points < 30)
        {
            CurrentPlayerLeague = "Recon League";
        }
        else if (points >= 30 && points < 50)
        {
            CurrentPlayerLeague = "Light Armor League";
        }
        else if (points >= 50 && points < 70)
        {
            CurrentPlayerLeague = "Panzer League";
        }
        else if (points >= 70 && points < 90)
        {
            CurrentPlayerLeague = "Steel League";
        }
        else if (points >= 90 && points < 110)
        {
            CurrentPlayerLeague = "Iron League";
        }
        else if (points >= 110 && points < 130)
        {
            CurrentPlayerLeague = "Heavy Armor League";
        }
        else if (points >= 130 && points < 150)
        {
            CurrentPlayerLeague = "Warrior League";
        }
        else if (points >= 150 && points < 170)
        {
            CurrentPlayerLeague = "Battle Master League";
        }
        else if (points >= 170)
        {
            CurrentPlayerLeague = "Titan League";
        }
        else
        {
            CurrentPlayerLeague = "Unranked";
        }

        Debug.Log("Player League: " + CurrentPlayerLeague);
    }
}