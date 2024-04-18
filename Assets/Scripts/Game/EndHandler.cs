using Assets.Scripts;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EndHandler : NetworkBehaviour
{
    public TextMeshProUGUI endText;
    public TextMeshProUGUI errorText;
    public Button exitBtn;
    public Button tryAgainBtn;
    public GameObject audioObject;

    private ConnectionCoroutine<object> uploadScoreCoroutine;
    private bool gameEnded = false;
    private PlayerSide? winnerPlayer;

    void Start()
    {
        errorText.text = "Loading...";
        endText.text = "";
        SetButtonVisibility(tryAgainBtn, ButtonVisibility.Hide);
        SetButtonVisibility(exitBtn, ButtonVisibility.Disabled);
        exitBtn.onClick.AddListener(() => SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true));
        tryAgainBtn.onClick.AddListener(() =>
        {
            time = 0;
            errorText.text = "Loading...";
            if (uploadScoreCoroutine.coroutine() != null) StopCoroutine(uploadScoreCoroutine.coroutine());
            StartCoroutine(uploadScoreCoroutine.coroutine());
        });
    }

    float time = 0;
    void Update()
    {
        if (!gameEnded) return;
        time += Time.deltaTime;

        if (winnerPlayer == null)
        {
            endText.text = "Draw!";
        }
        else
        {
            bool isCurrentPlayerWinner = (winnerPlayer == PlayerSide.Host && IsHost) ||
                                            (winnerPlayer == PlayerSide.Client && !IsHost);

            endText.text = isCurrentPlayerWinner ? "You won!" : "You lost!";

        }

        if (uploadScoreCoroutine.state == LoadingState.DataAvailable)
        {
            errorText.text = "Score uploaded!";
            SetButtonVisibility(tryAgainBtn, ButtonVisibility.Hide);
            SetButtonVisibility(exitBtn, ButtonVisibility.ShowAndEnable);
        }
        else if (uploadScoreCoroutine.state == LoadingState.NotLoaded && time < 10)
        {
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

    public void instantiateGameEnd(PlayerSide? winnerPlayer, string clientName, string hostName)
    {
        this.winnerPlayer = winnerPlayer;
        gameEnded = true;
        int hostScore = winnerPlayer == PlayerSide.Host ? 2 : -1;
        int clientScore = winnerPlayer == PlayerSide.Client ? 2 : -1;

        if (IsHost)
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(hostName, hostScore);
        }
        else
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(clientName, clientScore);
        }
        StartCoroutine(uploadScoreCoroutine.coroutine());
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
