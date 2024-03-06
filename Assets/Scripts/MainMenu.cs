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
        playerName.onValueChanged.AddListener(PlayerNameChanged);

        playerInfo = new PlayerInfo();
        playerName.text = playerInfo.PlayerName;
    }
    void StartNewGame()
    {
        if (string.IsNullOrEmpty(playerName.text)) return;

        startNetworkManager(IsHostToggle.isOn);
        SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene);
    }
    void QuitGame()
    {
        Application.Quit();
    }
    private void PlayerNameChanged(string newName)
    {
        playerInfo.PlayerName = newName;
        playerInfo.StorePlayerInfo();
    }
    void startNetworkManager(bool isHost)
    {
        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }
}
