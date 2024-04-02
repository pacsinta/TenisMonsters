using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
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

    private ConnectionCoroutine<LeaderBoardElement> uploadScoreCoroutine;
    private bool gameEnded = false;
    private PlayerSide? winnerPlayer;

    void Start()
    {
        errorText.text = "Loading...";
        endText.text = "";
        tryAgainBtn.enabled = false;
        exitBtn.enabled = false;
        exitBtn.onClick.AddListener(()=>SceneLoader.LoadScene(SceneLoader.Scene.MenuScene, NetworkManager.Singleton, true));
    }

    float time = 0;
    void Update()
    {
        if(!gameEnded) return;
        
        if(winnerPlayer == null)
        {
            endText.text = "Draw!";
        }
        else
        {
            endText.text = (winnerPlayer == PlayerSide.Host && IsHost) || 
                           (winnerPlayer == PlayerSide.Client && !IsHost) ? "You won!" : "You lost!";
        }

        if(uploadScoreCoroutine.state == LoadingState.DataAvailable)
        {
            exitBtn.enabled = true;
        }
        else if(uploadScoreCoroutine.state == LoadingState.NotLoaded)
        {
        }
        else if(uploadScoreCoroutine.state == LoadingState.Error)
        {
            tryAgainBtn.enabled = true; 
        }
    }

    public void instantiateGameEnd(PlayerSide? winnerPlayer, string clientName, string hostName)
    {
        this.winnerPlayer = winnerPlayer;
        gameEnded = true;
        int hostScore = winnerPlayer == PlayerSide.Host ? 2 : -1;
        int clientScore = winnerPlayer == PlayerSide.Client ? 2 : -1;
        
        if(IsHost)
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(hostName, hostScore);
        }
        else
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(clientName, clientScore);
        }
        
    }
}
