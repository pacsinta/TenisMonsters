using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public TextMeshProUGUI debugText;
    public TextMeshProUGUI scoreText;
    public NetworkObject playerObject;

    public uint MaxGameTime = 60 * 5;
    public uint MaxScore = 5;
    public Vector3 PlayerStartPosition;
    public float ZOffsetOfNet;

    private PlayerInfo hostPlayerInfo;
    private PlayerInfo clientPlayerInfo;
    private uint time = 0;
    public override void OnNetworkSpawn()
    {
        hostPlayerInfo = new PlayerInfo();

        if (!IsServer) return;
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
            clientPlayerInfo = new PlayerInfo(client.ClientId.ToString());
        }
    }

    private void Start()
    {
        if(!IsServer && !IsClient && !IsHost)
        {
            Debug.LogError("Netcode is not initialized");
            SceneLoader.LoadScene(SceneLoader.Scene.MenuScene);
        }
    }

    private void Update()
    {
        time += (uint)Time.deltaTime;
        debugText.text = time.ToString();
        /*if (time >= MaxGameTime || hostPlayerInfo.Score >= MaxScore || clientPlayerInfo.Score >= MaxScore)
        {
            EndGame();
        }*/
    }

    public void EndTurn(bool hostWon)
    {
        if (hostWon)
        {
            hostPlayerInfo.Score++;
        }
        else
        {
            clientPlayerInfo.Score++;
        }
        scoreText.text = "Host: " + hostPlayerInfo.Score + " Client: " + clientPlayerInfo.Score;
    }

    private void EndGame()
    {
        // TODO: Implement
        Debug.Log("Game Over");
    }
}
