using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameRules : NetworkBehaviour
{
    public TextMeshProUGUI debugText;
    public TextMeshProUGUI scoreText;
    public NetworkObject playerObject;

    public Vector3 PlayerStartPosition;
    public float ZOffsetOfNet;

    private PlayerInfo hostPlayerInfo;
    private PlayerInfo clientPlayerInfo;
    public override void OnNetworkSpawn()
    {
        hostPlayerInfo = new PlayerInfo();

        if (!IsHost) return;
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var client in clients)
        {
            Vector3 spawnPosition = PlayerStartPosition;
            Quaternion spawnRotation = Quaternion.identity;
            if (NetworkManager.Singleton.LocalClientId == client.ClientId)
            {
                spawnPosition = new Vector3(PlayerStartPosition.x, PlayerStartPosition.y, PlayerStartPosition.z * -1);
                spawnRotation = Quaternion.Euler(0, 180, 0);
            }
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerObject, client.ClientId, false, true, false, spawnPosition, spawnRotation);
        }
    }

    public void EndGame()
    {
        
    }
}
