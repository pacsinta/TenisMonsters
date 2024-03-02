using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Toggle IsHostToggle;

    public void IsHostToggled()
    {
        Settings.isHost = IsHostToggle.isOn;
    }
    public void StartNewGame()
    {
        SceneManager.LoadSceneAsync(1); // Load the game scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void Update()
    {
        Debug.Log(Settings.isHost);
    }
}
