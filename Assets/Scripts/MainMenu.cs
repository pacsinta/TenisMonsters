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
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
