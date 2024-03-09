using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameRules : NetworkBehaviour
{
    public TextMeshProUGUI debugText;
    public NetworkObject playerObject;

    public Vector3 PlayerStartPosition;
    public float ZOffsetOfNet;

    private PlayerInfo playerInfo;
    public override void OnNetworkSpawn()
    {
        playerInfo = new PlayerInfo();
        Debug.Log("PlayerInfo loaded");
        Debug.Log("PlayerName: " + playerInfo.PlayerName);

        if (!IsHost) return;
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var client in clients)
        {
            Vector3 spawnPosition = PlayerStartPosition;
            if(NetworkManager.Singleton.LocalClientId == client.ClientId)
            {
                spawnPosition = new Vector3(PlayerStartPosition.x, PlayerStartPosition.y, PlayerStartPosition.z * -1);
            }
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerObject, client.ClientId, false, true, false, spawnPosition);
        }
    }
}
