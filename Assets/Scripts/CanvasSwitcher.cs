using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public void SwitchCanvas(GameObject canvas)
    {
        canvas.SetActive(!canvas.activeSelf);
    }
}
