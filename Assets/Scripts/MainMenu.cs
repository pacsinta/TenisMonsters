using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneManager.LoadSceneAsync(1); // Load the game scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
