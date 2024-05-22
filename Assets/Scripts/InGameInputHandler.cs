using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameInputHandler : MonoBehaviour
{
    public GameObject MenuUI;

    private bool _isMenuActive = false;

    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["InGameMenuCall"].performed += OnOpenMenu;
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["InGameMenuCall"].performed -= OnOpenMenu;
    }

    private void OnOpenMenu(InputAction.CallbackContext context)
    {
        MenuUI.SetActive(!MenuUI.activeSelf);
    }
}
