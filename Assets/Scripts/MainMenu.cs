using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
    public Toggle IsHostToggle;
    public TMP_InputField playerName;
    public Button startGameBtn;
    public Button exitBtn;

    private PlayerInfo playerInfo;
    private void Start()
    {
        startGameBtn.onClick.AddListener(StartNewGame);
        exitBtn.onClick.AddListener(QuitGame);
        playerName.onValueChanged.AddListener(PlayerNameChenged);

        playerInfo = new PlayerInfo();
        playerName.text = playerInfo.PlayerName;
    }
    void StartNewGame()
    {
        if (string.IsNullOrEmpty(playerName.text)) return;

        if(IsHostToggle.isOn)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
        SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    private void PlayerNameChenged(string newName)
    {
        playerInfo.PlayerName = newName;
        playerInfo.StorePlayerInfo();
    }
}
