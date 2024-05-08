using Assets.Scripts;
using Assets.Scripts.Menu.Leaderboard;
using Assets.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Toggle IsHostToggle;
    public TMP_InputField playerName;
    public TMP_InputField passwordInput;
    public Button startGameBtn;
    public Button exitBtn;
    public TMP_InputField hostIpInput;
    public Canvas mainCanvas;
    public Canvas leaderBoardCanvas;
    public Button settingsButton;
    public Button leaderBoardButton;
    public GameObject settingsPanel;
    public TextMeshProUGUI errorText;

    private PlayerInfo playerInfo;
    private ConnectionCoroutine<object> authCheck;
    private void Start()
    {
        startGameBtn.onClick.AddListener(InstantiatStartGame);
        playerName.onValueChanged.AddListener(PlayerNameChanged);
        leaderBoardButton.onClick.AddListener(SwitchCanvas);
        exitBtn.onClick.AddListener(() => { Application.Quit(); });

        playerInfo = new PlayerInfo();
        playerName.text = playerInfo.PlayerName.ToString();

        mainCanvas.gameObject.SetActive(true);
        leaderBoardCanvas.gameObject.SetActive(false);
        
        settingsPanel.SetActive(false);
        settingsButton.onClick.AddListener(() => { settingsPanel.SetActive(!settingsPanel.activeSelf); });
        settingsPanel.GetComponent<Settings>().RestoreSettings();
    }

    private void Update()
    {
        if (IsHostToggle.isOn)
        {
            hostIpInput.interactable = false;
        }
        else
        {
            hostIpInput.interactable = true;
        }

        HostAvailabilityCheck();
        PasswordValidityCheck();
    }

    void HostAvailabilityCheck()
    {
        if (connectingTime < 5 && connecting)
        {
            connectingTime += Time.deltaTime;
            errorText.text = "";
        }
        else if (connecting)
        {
            NetworkManager.Singleton.Shutdown();
            connecting = false;
            errorText.text = "Can't connect to a host!";
        }
    }

    void PasswordValidityCheck()
    {
        if(authCheck == null) return;
        var state = authCheck.state;
        if(state == LoadingState.DataAvailable && !gameStarted)
        {
            var _ = authCheck.Result;
            StopAllCoroutines();
            SecureStore.SavePassword(playerName.text, passwordInput.text); // Only save the password if the server authentication was successful
            StartNewGame();
        }
        else if(state == LoadingState.Error)
        {
            StopAllCoroutines();
            errorText.text = "Authentication error!";
        }
    }

    void InstantiatStartGame()
    {
        if (string.IsNullOrEmpty(playerName.text))
        {
            errorText.text = "Player name can't be empty!";
            return;
        };
        if (string.IsNullOrEmpty(passwordInput.text))
        {
            errorText.text = "Password can't be empty!";
            return;
        }

        if (!SecureStore.SecureCheck(playerName.text, passwordInput.text))
        {
            errorText.text = "Wrong password!";
            return;
        }

        if (authCheck?.Coroutine() != null) StopCoroutine(authCheck.Coroutine());
        authCheck = DatabaseHandler.CheckAuth(
            playerName.text,
            SecureStore.CreateHashWithConstSalt(passwordInput.text)
        );
        StartCoroutine(authCheck.Coroutine());
        print("authCheck started!");
    }
    private bool gameStarted = false;
    void StartNewGame()
    {
        if (StartNetworkManager(IsHostToggle.isOn))
        {
            gameStarted = true;
            if (IsHostToggle.isOn)
            {
                SceneLoader.LoadScene(SceneLoader.Scene.LobbyScene, NetworkManager.Singleton);
            }
        }
    }

    private void PlayerNameChanged(string newName)
    {
        if (newName.Length > 32) return;
        playerInfo.PlayerName = newName;
        playerInfo.StorePlayerInfo();
    }
    private float connectingTime = 0;
    private bool connecting = false;
    bool StartNetworkManager(bool isHost)
    {
        NetworkManager.Singleton.Shutdown();

        if (isHost)
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                "0.0.0.0",
                7777
            );
            return NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                string.IsNullOrEmpty(hostIpInput.text) ? "127.0.0.1" : hostIpInput.text,
                7777
            );
            connectingTime = 0;
            connecting = true;
            return NetworkManager.Singleton.StartClient();
        }
    }

    public void SwitchCanvas()
    {
        mainCanvas.gameObject.SetActive(!mainCanvas.gameObject.activeSelf);
        leaderBoardCanvas.gameObject.SetActive(!leaderBoardCanvas.gameObject.activeSelf);
    }
}
