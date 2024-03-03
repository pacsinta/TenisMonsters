using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("MultiplayerMode: " + (Settings.isHost ? "Host" : "Client"));
        if(Settings.isHost)
        {
            NetworkManager.Singleton.StartHost(); // Start the server
        }
        else
        {
            NetworkManager.Singleton.StartClient(); // Start the client
        }
    }
}
