using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Create a LoaderScene while the game is loading
public class SceneLoader
{
    public enum Scene
    {
        MenuScene = 0,
        GameScene = 1,
        LobbyScene = 2
    }

    public static void LoadScene(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}
