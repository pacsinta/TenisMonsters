using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
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
    public TMP_InputField hostIpInput;
    public TMP_Dropdown WindowModeDropdown;

    private PlayerInfo playerInfo;
    private void Start()
    {
        startGameBtn.onClick.AddListener(StartNewGame);
        exitBtn.onClick.AddListener(QuitGame);
        playerName.onValueChanged.AddListener(PlayerNameChanged);
        WindowModeDropdown.onValueChanged.AddListener(SetWindowMode);

        playerInfo = new PlayerInfo();
        playerName.text = playerInfo.PlayerName.ToSafeString();
    }
    void StartNewGame()
    {
        if (string.IsNullOrEmpty(playerName.text)) return;

        startNetworkManager(IsHostToggle.isOn);

        if (IsHost)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene, NetworkManager.Singleton);
        }
        
    }
    void QuitGame()
    {
        Application.Quit();
    }
    private void PlayerNameChanged(string newName)
    {
        if (newName.Length > 32) return;
        playerInfo.PlayerName = newName;
        playerInfo.StorePlayerInfo();
    }
    void startNetworkManager(bool isHost)
    {
        /*NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
            hostIpInput.text,
            (ushort)7777,
            "0.0.0.0"
        );*/
        
        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    void SetWindowMode(int mode)
    {
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }
}
