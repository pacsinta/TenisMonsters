using Assets.Scripts;
using Assets.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EndHandler : MonoBehaviour
{
    public TextMeshProUGUI endText;
    public TextMeshProUGUI end2Text;
    public TextMeshProUGUI errorText;
    public Button exitBtn;
    public Button tryAgainBtn;

    private ConnectionCoroutine<object> uploadScoreCoroutine;
    private bool gameEnded = false;
    private PlayerSide? winnerPlayer;
    private string oponentPlayerName;
    private GameObject audioObject;
    private bool IsHost = false;

    void Start()
    {
        errorText.text = "Loading...";
        endText.text = "";
        end2Text.text = "";
        SetButtonVisibility(tryAgainBtn, ButtonVisibility.Hide);
        SetButtonVisibility(exitBtn, ButtonVisibility.Disabled);
        exitBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true));
        tryAgainBtn.onClick.AddListener(() =>
        {
            timeOutTime = 0;
            errorText.text = "Loading...";
            if (uploadScoreCoroutine.Coroutine() != null) StopCoroutine(uploadScoreCoroutine.Coroutine());
            StartCoroutine(uploadScoreCoroutine.Coroutine());
        });

        audioObject = GameObject.Find("Audio");
    }

    float timeOutTime = 0;
    float audioTime = 0;
    void Update()
    {
        if (!gameEnded) return;
        timeOutTime += Time.deltaTime;
        audioTime += Time.deltaTime;

        bool playWinnerAudio = true;
        if (winnerPlayer == null)
        {
            endText.text = "Draw!";
            end2Text.text = "";
        }
        else
        {
            bool isCurrentPlayerWinner = (winnerPlayer == PlayerSide.Host && IsHost) ||
                                            (winnerPlayer == PlayerSide.Client && !IsHost);

            endText.text = isCurrentPlayerWinner ? "You won!" : "You lost!";
            end2Text.text = isCurrentPlayerWinner ? oponentPlayerName + " lost!" : oponentPlayerName + " won!";
            playWinnerAudio = isCurrentPlayerWinner;
        }

        if (audioTime % 60 == 0 && audioObject != null)
        {
            audioObject.GetComponent<Audio>().PlayEndingSong(playWinnerAudio);
        }

        if (uploadScoreCoroutine.state == LoadingState.DataAvailable)
        {
            errorText.text = "Score uploaded!";
            SetButtonVisibility(tryAgainBtn, ButtonVisibility.Hide);
            SetButtonVisibility(exitBtn, ButtonVisibility.ShowAndEnable);
        }
        else if (uploadScoreCoroutine.state == LoadingState.NotLoaded && timeOutTime < 10)
        {
            errorText.text = "Loading...";
            SetButtonVisibility(tryAgainBtn, ButtonVisibility.Hide);
            SetButtonVisibility(exitBtn, ButtonVisibility.Disabled);
        }
        else // error or timeout
        {
            errorText.text = "Can't upload the score";
            SetButtonVisibility(tryAgainBtn, ButtonVisibility.ShowAndEnable);
            SetButtonVisibility(exitBtn, ButtonVisibility.ShowAndEnable);
        }
    }

    public void InstantiateGameEnd(PlayerSide? winnerPlayer, string clientName, string hostName, bool IsHost)
    {
        this.winnerPlayer = winnerPlayer;
        this.IsHost = IsHost;
        oponentPlayerName = IsHost ? clientName : hostName;
        gameEnded = true;

        int hostScore = 0;
        int clientScore = 0;
        switch (winnerPlayer)
        {
            case PlayerSide.Host:
                hostScore = 2; clientScore = -1;
                break;
            case PlayerSide.Client:
                hostScore = -1; clientScore = 2;
                break;
            case null:
                hostScore = 1; clientScore = 1;
                break;
        }

        if (IsHost)
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(hostName, hostScore, SecureStore.GetHashedPassword(hostName));
        }
        else
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(clientName, clientScore, SecureStore.GetHashedPassword(clientName));
        }
        StartCoroutine(uploadScoreCoroutine.Coroutine());
    }

    private enum ButtonVisibility
    {
        Hide,
        Disabled,
        ShowAndEnable
    }

    private void SetButtonVisibility(Button button, ButtonVisibility visibility)
    {
        switch (visibility)
        {
            case ButtonVisibility.Hide:
                button.gameObject.SetActive(false);
                button.enabled = false;
                break;
            case ButtonVisibility.Disabled:
                button.gameObject.SetActive(true);
                button.enabled = false;
                break;
            case ButtonVisibility.ShowAndEnable:
                button.gameObject.SetActive(true);
                button.enabled = true;
                break;
        }
    }
}
