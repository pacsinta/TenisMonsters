using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    void Awake()
    {
        //Debug.Log("MultiplayerMode: " + (PlayerInfo.isHost ? "Host" : "Client"));
        /*if(PlayerInfo.isHost)
        {
            NetworkManager.Singleton.StartHost(); // Start the server
        }
        else
        {
            NetworkManager.Singleton.StartClient(); // Start the client
        }*/
    }

    void Start()
    {
        //debugText.text = "MultiplayerMode: " + (PlayerInfo.isHost ? "Host" : "Client");
    }
}
