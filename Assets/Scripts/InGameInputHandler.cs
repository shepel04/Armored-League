using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InGameInputHandler : MonoBehaviour
{
    public GameObject MenuUI;
    public GameObject HUD;

    private bool _isMenuActive = false;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            OnOpenMenu();
        }
    }

    private void OnOpenMenu()
    {
        MenuUI.SetActive(!MenuUI.activeSelf);
        HUD.SetActive(!HUD.activeSelf);
    }
}