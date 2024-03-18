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
    public TMP_Dropdown gameModeDropdown;

    private int maxPlayerCount = 2;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        playerInfo = new PlayerInfo();

        clientsText.text = "No clients connected";
        if(IsHost)
        {
            IsHostText.text = "Host";
            _hostPlayerInfo.Value = playerInfo;
            gameModeDropdown.onValueChanged.AddListener(ChangeGameMode);
        }
        else
        {
            IsHostText.text = "Client";
            gameModeDropdown.interactable = false;
            playerInfo.Side = PlayerSide.Client;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        startGameBtn.onClick.AddListener(StartGame);
    }

    private void Update()
    {
        if(string.IsNullOrEmpty(_clientPlayerInfo.Value?.PlayerName.ToString()))
        {
            clientsText.text = "No clients connected";
        }
        else
        {
            clientsText.text = "Host: " + _hostPlayerInfo.Value.PlayerName + "\nClient: " + _clientPlayerInfo.Value.PlayerName;
        }

        gameModeDropdown.value = _gameInfo.Value == null ? 0 : _gameInfo.Value.gameMode;
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
            _clientPlayerInfo.Value = clientPlayerInfo;
            clientPlayerInfo.StorePlayerInfo(clientId.ToString());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene, NetworkManager.Singleton);
    }

    NetworkVariable<PlayerInfo> _hostPlayerInfo = new NetworkVariable<PlayerInfo>();
    NetworkVariable<PlayerInfo> _clientPlayerInfo = new NetworkVariable<PlayerInfo>();
    NetworkVariable<GameInfo> _gameInfo = new NetworkVariable<GameInfo>();

    void ChangeGameMode(int mode)
    {
        _gameInfo.Value = new GameInfo();
        _gameInfo.Value.gameMode = mode;
        _gameInfo.Value.SaveGameInfo();
    }
}
