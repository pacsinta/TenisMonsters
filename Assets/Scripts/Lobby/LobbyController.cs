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
    public TextMeshProUGUI titleText;

    // Game mode panel
    public TMP_Dropdown gameModeDropdown;
    public Toggle gravityPowerBallToggle;
    public Toggle speedPowerBallToggle;
    public Toggle rotationKickPowerBallToggle;
    public Slider powerBallSpawnTimeSlider;
    public TextMeshProUGUI powerBallSpawnTimeText;

    private int maxPlayerCount = 2;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        playerInfo = new PlayerInfo();

        clientsText.text = "No clients connected";
        if(IsHost)
        {
            _gameInfo.Value = new GameInfoSync();

            IsHostText.text = "Host";
            _hostPlayerInfo.Value = playerInfo;

            gameModeDropdown.onValueChanged.AddListener( (mode) => { _gameInfo.Value.SetGameMode(mode); });
            gameModeDropdown.value = _gameInfo.Value.GetGameMode();

            gravityPowerBallToggle.onValueChanged.AddListener( (enabled) => { _gameInfo.Value.SetGravityPowerballEnabled(enabled); });
            gravityPowerBallToggle.isOn = _gameInfo.Value.GetGravityPowerballEnabled();

            speedPowerBallToggle.onValueChanged.AddListener( (enabled) => { _gameInfo.Value.SetSpeedPowerballEnabled(enabled); });
            speedPowerBallToggle.isOn = _gameInfo.Value.GetSpeedPowerballEnabled();

            rotationKickPowerBallToggle.onValueChanged.AddListener( (enabled) => { _gameInfo.Value.SetRotationKickPowerballEnabled(enabled); });
            rotationKickPowerBallToggle.isOn = _gameInfo.Value.GetRotationKickPowerballEnabled();

            powerBallSpawnTimeSlider.onValueChanged.AddListener( (time) => {
                _gameInfo.Value.powerBallSpawnTime = (int)time;
                powerBallSpawnTimeText.text = "Powerball spawn time: " + (int)time * 10 + "s";
            });
            powerBallSpawnTimeSlider.value = _gameInfo.Value.powerBallSpawnTime;
            
        }
        else
        {
            IsHostText.text = "Client";
            gameModeDropdown.interactable = false;
            gravityPowerBallToggle.interactable = false;
            speedPowerBallToggle.interactable = false;
            rotationKickPowerBallToggle.interactable = false;
            powerBallSpawnTimeSlider.interactable = false;
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

        gameModeDropdown.value = _gameInfo.Value.GetGameMode();
        gravityPowerBallToggle.isOn = _gameInfo.Value.GetGravityPowerballEnabled();
        speedPowerBallToggle.isOn = _gameInfo.Value.GetSpeedPowerballEnabled();
        rotationKickPowerBallToggle.isOn = _gameInfo.Value.GetRotationKickPowerballEnabled();
        powerBallSpawnTimeSlider.value = _gameInfo.Value.powerBallSpawnTime;
        powerBallSpawnTimeText.text = "Powerball spawn time: " + _gameInfo.Value.powerBallSpawnTime * 10 + "s";

        
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
        titleText.text = "Ready for game";
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
    NetworkVariable<GameInfoSync> _gameInfo = new NetworkVariable<GameInfoSync>();
}
