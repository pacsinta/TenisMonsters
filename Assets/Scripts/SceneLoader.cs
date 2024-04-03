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
    public static Scene GetCurrentScene()
    {
        return (Scene)SceneManager.GetActiveScene().buildIndex;
    }

    public static void LoadScene(Scene scene, NetworkManager networkManager)
    {
        if(networkManager != null)
        {
            networkManager.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
        }
        else
        {
            LoadScene(scene);
        }
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
            networkManager.ConnectionApprovalCallback = null;
            LoadScene(scene);
        }
        else
        {
            LoadScene(scene, networkManager);
        }
    }
}
