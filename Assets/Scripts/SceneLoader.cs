using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    public enum Scene
    {
        MenuScene = 0,
        GameScene = 1,
        LobbyScene = 2
    }

    public static void LoadScene(Scene scene, NetworkManager networkManager)
    {
        networkManager.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }
    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public static void LoadScene(Scene scene, NetworkManager networkManager, bool exit)
    {
        if(exit && networkManager != null)
        {
            networkManager.Shutdown();
            LoadScene(scene);
        }
        else
        {
            LoadScene(scene, networkManager);
        }
    }
}
