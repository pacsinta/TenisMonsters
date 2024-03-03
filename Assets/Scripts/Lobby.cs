using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    private int maxPlayerCount = 2;
    private void Start()
    {
        
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnect;
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(NetworkManager.Singleton.ConnectedClients.Count > maxPlayerCount - 1)
        {
            response.Approved = false;
            response.Reason = "Server is full";
        }
        else
        {
            response.Approved = true;
        }
    }
    
    void HandleClientConnect(ulong clientId)
    {
    }
}
