using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreViewer : MonoBehaviour
{
    public TMP_Text TargetText;
    public TMP_Text Scoreboard;
    
    void Start()
    {
        TargetText.text = Scoreboard.text;
    }
}
