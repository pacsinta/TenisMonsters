using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
    public TextMeshProUGUI clientsText;
    public Button startGameBtn;
    public TextMeshProUGUI IsHostText;

    private int maxPlayerCount = 2;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        clientsText.text = "No clients connected";
        if(IsServer)
        {
            IsHostText.text = "Host";
        }
        else
        {
            IsHostText.text = "Client";
        }

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        startGameBtn.onClick.AddListener(StartGame);

        playerInfo = new PlayerInfo();
        if(IsClient)
        {
            playerInfo.Side = PlayerSide.Right;
        }
    }

    private void Update()
    {
        if(string.IsNullOrEmpty(playerNames.Value.clientPlayerName.ToString()))
        {
            clientsText.text = "No clients connected";
        }
        else
        {
            clientsText.text = "Host: " + playerNames.Value.hostPlayerName + "\nClient: " + playerNames.Value.clientPlayerName;
        }
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
    }

    void HandleClientConnect(ulong clientId)
    {
        if(NetworkManager.Singleton.LocalClientId == clientId)
        {
            RegisterPlayerOnServerRpc(playerInfo);
        }
    }

    void StartGame()
    {
        if(IsServer)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene, NetworkManager.Singleton);
        }
        else
        {
            StartGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RegisterPlayerOnServerRpc(PlayerInfo clientPlayerInfo,ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var clientPlayerName = clientPlayerInfo.PlayerName;
            playerNames.Value = new PlayersData
            {
                hostPlayerName = playerInfo.PlayerName,
                clientPlayerName = clientPlayerName
            };
            clientPlayerInfo.StorePlayerInfo(clientId.ToString());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene, NetworkManager.Singleton);
    }

    NetworkVariable<PlayersData> playerNames = new NetworkVariable<PlayersData>();
}
