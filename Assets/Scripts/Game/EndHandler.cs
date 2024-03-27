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
    public Button exitBtn;

    private ConnectionCoroutine<LeaderBoardElement> uploadScoreCoroutine;
    private bool gameEnded = false;
    private bool hostWon = false;



    // Update is called once per frame
    void Update()
    {
        if(!gameEnded) return;
        
        endText.text = hostWon == IsHost ? "You won!" : "You lost!";

        if(!IsHost) return;
    }

    public void instantiateGameEnd(bool hostWon, string clientName, string hostName)
    {
        this.hostWon = hostWon;
        gameEnded = true;
        int hostScore = hostWon ? 2 : -1;
        int clientScore = hostWon ? -1 : 2;
        
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
