using Assets.Scripts;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : NetworkBehaviour
{
    public Toggle IsHostToggle;
    public TMP_InputField playerName;
    public Button startGameBtn;
    public Button exitBtn;
    public TMP_InputField hostIpInput;
    public Canvas mainCanvas;
    public Canvas leaderBoardCanvas;
    public TextMeshProUGUI myPontsText;
    public Button settingsButton;
    public GameObject settingsPanel;

    // monster show variables
    public GameObject monster;
    public float rotationSpeed = 10.0f;
    public float rotationAngleLimit = 15.0f;

    private PlayerInfo playerInfo;
    private ConnectionCoroutine<LeaderBoardElement> myPointCoroutine;
    private void Start()
    {
        startGameBtn.onClick.AddListener(StartNewGame);
        playerName.onValueChanged.AddListener(PlayerNameChanged);

        exitBtn.onClick.AddListener(() => { Application.Quit(); });

        playerInfo = new PlayerInfo();
        playerName.text = playerInfo.PlayerName.ToString();

        mainCanvas.gameObject.SetActive(true);
        leaderBoardCanvas.gameObject.SetActive(false);

        myPointCoroutine = DatabaseHandler.GetMyPoints(playerInfo.PlayerName.ToString());
        StartCoroutine(myPointCoroutine.coroutine());

        var resolution = Screen.resolutions[Screen.resolutions.Length - 2 >= 0 ? Screen.resolutions.Length - 2 : 0];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed);

        settingsPanel.SetActive(false);
        settingsButton.onClick.AddListener(() => { settingsPanel.SetActive(!settingsPanel.activeSelf); });
    }

    private float time = 0;
    private void Update()
    {
        time += Time.deltaTime;
        if (myPointCoroutine.state == LoadingState.DataAvailable)
        {
            Debug.Log("MyPoints loaded: " + myPointCoroutine.Result.ToString());
            myPontsText.text = "MyScore: " + myPointCoroutine.Result.Score;
        }
        else if (myPointCoroutine.state == LoadingState.Error || myPointCoroutine.state == LoadingState.NotLoaded)
        {
            myPontsText.text = "MyScore: -";
            if (time >= 10)
            {
                if (myPointCoroutine.coroutine() != null)
                {
                    StopCoroutine(myPointCoroutine.coroutine());
                }
                myPointCoroutine = DatabaseHandler.GetMyPoints(playerInfo.PlayerName.ToString());
                StartCoroutine(myPointCoroutine.coroutine());
                time = 0;
            }
        }

        RotateMonster();

        if (IsHostToggle.isOn)
        {
            hostIpInput.interactable = false;
        }
        else
        {
            hostIpInput.interactable = true;
        }
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

    private bool rotateRight = true;
    void RotateMonster()
    {
        if (rotateRight)
        {
            monster.transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
            if (monster.transform.rotation.eulerAngles.y - 180 >= rotationAngleLimit)
            {
                rotateRight = false;
            }
        }
        else
        {
            monster.transform.Rotate(Vector3.down * Time.deltaTime * rotationSpeed);
            if (monster.transform.rotation.eulerAngles.y - 180 < -rotationAngleLimit)
            {
                rotateRight = true;
            }
        }
    }

    private void PlayerNameChanged(string newName)
    {
        if (newName.Length > 32) return;
        playerInfo.PlayerName = newName;
        playerInfo.StorePlayerInfo();
    }
    void startNetworkManager(bool isHost)
    {
        NetworkManager.Singleton?.Shutdown();

        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                string.IsNullOrEmpty(hostIpInput.text) ? "127.0.0.1" : hostIpInput.text,
                7777
            );
            NetworkManager.Singleton.StartClient();
        }
    }

    public void SwitchCanvas()
    {
        mainCanvas.gameObject.SetActive(!mainCanvas.gameObject.activeSelf);
        leaderBoardCanvas.gameObject.SetActive(!leaderBoardCanvas.gameObject.activeSelf);
    }
}
