using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    public void LoadSceneByNumber(int sceneNumber)
    {
        Debug.Log("Scene loading");
        SceneManager.LoadScene(sceneNumber);
    }
}
