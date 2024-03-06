using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : NetworkBehaviour
{
    public TextMeshProUGUI clientsText;
    public Button startGameBtn;

    private int maxPlayerCount = 2;
    private PlayerInfo playerInfo;
    private void Start()
    {
        clientsText.text = "No clients connected";

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        startGameBtn.onClick.AddListener(StartGame);

        playerInfo = new PlayerInfo();
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(NetworkManager.Singleton.ConnectedClients.Count > maxPlayerCount - 1)
        {
            response.Approved = false;
        }
        else
        {
            response.Approved = true;
        }

        Debug.Log("ApprovalCheck: " + response.Approved);
    }

    void HandleClientConnect(ulong clientId)
    {
        Debug.Log("Handle Connection: " + clientId);
        
        if(IsOwner)
        {
            RegisterPlayerOnServerRpc(playerInfo.PlayerName);
        }
    }

    void ApprovalCheckDecline(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = false;
        response.Reason = "Game already in progress";
    }

    void StartGame()
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    [ServerRpc]
    void RegisterPlayerOnServerRpc(string PlayerName)
    {
        Debug.Log("PlayerName: " + PlayerName);
        clientsText.text = "Client connected: " + PlayerName;
    }
}
