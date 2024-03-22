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
    public Button openLeaderboardBtn;
    public TMP_InputField hostIpInput;
    public TMP_Dropdown WindowModeDropdown;
    public Canvas mainCanvas;
    public Canvas leaderBoardCanvas;
    public TextMeshProUGUI myPontsText;

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
        WindowModeDropdown.onValueChanged.AddListener(SetWindowMode);

        openLeaderboardBtn.onClick.AddListener(() => { 
            mainCanvas.gameObject.SetActive(false);
            leaderBoardCanvas.gameObject.SetActive(true);
        });
        exitBtn.onClick.AddListener(() => { Application.Quit(); });

        playerInfo = new PlayerInfo();
        playerName.text = playerInfo.PlayerName.ToSafeString();

        mainCanvas.gameObject.SetActive(true);
        leaderBoardCanvas.gameObject.SetActive(false);

        myPointCoroutine = DatabaseHandler.GetMyPoints(playerInfo.PlayerName.ToSafeString());
        StartCoroutine(myPointCoroutine.coroutine());
    }

    private float time = 0;
    private void Update()
    {
        time += Time.deltaTime;
        if(myPointCoroutine.state == LoadingState.DataAvailable)
        {
            Debug.Log("MyPoints loaded: " + myPointCoroutine.Result.ToString());
            myPontsText.text = "MyScore: " + myPointCoroutine.Result.Score;
        }
        else if(myPointCoroutine.state == LoadingState.Error || myPointCoroutine.state == LoadingState.NotLoaded)
        {
            myPontsText.text = "MyScore: 0";
            if(time >= 10)
            {
                if(myPointCoroutine.coroutine() != null)
                {
                    StopCoroutine(myPointCoroutine.coroutine());
                }
                myPointCoroutine = DatabaseHandler.GetMyPoints(playerInfo.PlayerName.ToSafeString());
                StartCoroutine(myPointCoroutine.coroutine());
                time = 0;
            }
        }

        RotateMonster();
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
        if(rotateRight)
        {
            monster.transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
            if(monster.transform.rotation.eulerAngles.y - 180 >= rotationAngleLimit)
            {
                rotateRight = false;
            }
        }
        else
        {
            monster.transform.Rotate(Vector3.down * Time.deltaTime * rotationSpeed);
            if(monster.transform.rotation.eulerAngles.y - 180 < -rotationAngleLimit)
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
