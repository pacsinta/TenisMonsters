using Assets.Scripts;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyController : NetworkBehaviour
{
    public TextMeshProUGUI clientsText;
    public Button startGameButton;
    public TextMeshProUGUI titleText;
    public Button exitButton;

    //Environment panel
    public TMP_Dropdown skyDropdown;
    public TextMeshProUGUI timeText;
    public Slider timeSlider;

    // Game mode panel
    public TMP_Dropdown gameModeDropdown;
    public Toggle gravityPowerBallToggle;
    public Toggle speedPowerBallToggle;
    public Toggle rotationKickPowerBallToggle;
    public Slider powerBallSpawnTimeSlider;
    public TextMeshProUGUI powerBallSpawnTimeText;
    public Toggle multiplePowerBallToggle;
    public TextMeshProUGUI powerBallLiveTimeText;
    public Slider powerballLiveTimeSlider;
    public Toggle wallsEnabledToggle;

    private readonly int maxPlayerCount = 2;
    private PlayerInfo playerInfo;

    public override void OnNetworkSpawn()
    {
        playerInfo = new PlayerInfo();

        clientsText.text = "No clients connected";
        if (IsHost)
        {
            _gameInfo.Value = new GameInfo();
            _hostPlayerInfo.Value = playerInfo;

            SetGameSettingListeners();
        }
        else
        {
            DisableGameSettings();
            playerInfo.Side = PlayerSide.Client;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        startGameButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true));
    }

    private void DisableGameSettings()
    {
        gameModeDropdown.interactable = false;
        gravityPowerBallToggle.interactable = false;
        speedPowerBallToggle.interactable = false;
        rotationKickPowerBallToggle.interactable = false;
        powerBallSpawnTimeSlider.interactable = false;
        multiplePowerBallToggle.interactable = false;
        powerballLiveTimeSlider.interactable = false;
        wallsEnabledToggle.interactable = false;
        skyDropdown.interactable = false;
        timeSlider.interactable = false;
    }

    private void SetGameSettingListeners()
    {
        gameModeDropdown.onValueChanged.AddListener(GameSettingListeners<int>(_gameInfo.Value.SetGameMode));
        gravityPowerBallToggle.onValueChanged.AddListener(GameSettingListeners<bool>(_gameInfo.Value.SetGravityPowerballEnabled));
        speedPowerBallToggle.onValueChanged.AddListener(GameSettingListeners<bool>(_gameInfo.Value.SetSpeedPowerballEnabled));
        rotationKickPowerBallToggle.onValueChanged.AddListener(GameSettingListeners<bool>(_gameInfo.Value.SetRotationKickPowerballEnabled));
        powerBallSpawnTimeSlider.onValueChanged.AddListener(GameSettingListeners<float>(_gameInfo.Value.SetPowerBallSpawnTime));
        powerballLiveTimeSlider.onValueChanged.AddListener(GameSettingListeners<float>(_gameInfo.Value.SetPowerBallLiveTime));
        multiplePowerBallToggle.onValueChanged.AddListener((isOn) =>
        {
            GameSettingListeners<bool>(_gameInfo.Value.SetMultiplePowerBalls)(isOn);
            powerballLiveTimeSlider.interactable = isOn;
        });
        wallsEnabledToggle.onValueChanged.AddListener(GameSettingListeners<bool>(_gameInfo.Value.SetWallsEnabled));
        skyDropdown.onValueChanged.AddListener(GameSettingListeners<int>(_gameInfo.Value.SetSkyType));
        timeSlider.onValueChanged.AddListener(GameSettingListeners<float>(_gameInfo.Value.SetTimeSpeed));
    }

    private UnityAction<T> GameSettingListeners<T>(Action<T> func)
    {
        return (enabled) => { func(enabled); _gameInfo.IsDirty(); };
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(_clientPlayerInfo.Value?.PlayerName.ToString()))
        {
            clientsText.text = "No clients connected";
        }
        else
        {
            var ownerName = IsHost ? _hostPlayerInfo.Value.PlayerName : _clientPlayerInfo.Value.PlayerName;
            var opponentName = IsHost ? _clientPlayerInfo.Value.PlayerName : _hostPlayerInfo.Value.PlayerName;
            clientsText.text = "You: " + ownerName + "\nOpponent: " + opponentName;
        }

        if(_gameInfo.Value == null)
        {
            return;
        }
        multiplePowerBallToggle.isOn = _gameInfo.Value.multiplePowerBalls;
        gameModeDropdown.value = _gameInfo.Value.gameMode;
        gravityPowerBallToggle.isOn = _gameInfo.Value.gravityPowerballEnabled;
        speedPowerBallToggle.isOn = _gameInfo.Value.speedPowerballEnabled;
        rotationKickPowerBallToggle.isOn = _gameInfo.Value.rotationKickPowerballEnabled;
        powerBallSpawnTimeSlider.value = _gameInfo.Value.powerBallSpawnTime / 10;
        powerBallSpawnTimeText.text = "Powerball spawn time: " + _gameInfo.Value.powerBallSpawnTime + "s";
        powerballLiveTimeSlider.value = _gameInfo.Value.powerBallLiveTime / 5;
        powerBallLiveTimeText.text = "Powerball live time: " + (_gameInfo.Value.multiplePowerBalls ? _gameInfo.Value.powerBallLiveTime : "0") + "s";
        wallsEnabledToggle.isOn = _gameInfo.Value.wallsEnabled;
        skyDropdown.value = (int)_gameInfo.Value.skyType;
        timeSlider.value = _gameInfo.Value.timeSpeed;
        timeText.text = "Time speed: " + _gameInfo.Value.timeSpeed.ToString("F1");
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > maxPlayerCount - 1)
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
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            RegisterPlayerOnServerRpc(playerInfo);
        }
        titleText.text = "Ready for game";
    }

    void HandleClientDisconnect(ulong _)
    {
        if (!IsHost) return;

        titleText.text = "Wait for oponents";
        clientsText.text = "No clients connected";
        _clientPlayerInfo.Value = null;
    }

    void StartGame()
    {
        if (IsServer)
        {
            SceneLoader.LoadScene(SceneLoader.Scene.GameScene, NetworkManager.Singleton);
        }
        else
        {
            StartGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RegisterPlayerOnServerRpc(PlayerInfo clientPlayerInfo, ServerRpcParams serverRpcParams = default)
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

    readonly NetworkVariable<PlayerInfo> _hostPlayerInfo = new();
    readonly NetworkVariable<PlayerInfo> _clientPlayerInfo = new();
    readonly NetworkVariable<GameInfo> _gameInfo = new();
}
