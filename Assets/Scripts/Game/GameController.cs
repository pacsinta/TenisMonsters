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

    NetworkVariable<PlayerInfo> _hostPlayerInfo = new NetworkVariable<PlayerInfo>();
    NetworkVariable<PlayerInfo> _clientPlayerInfo = new NetworkVariable<PlayerInfo>();
    private uint time = 0;
    public override void OnNetworkSpawn()
    {
        _hostPlayerInfo.Value = new PlayerInfo();

        if (!IsServer) return;
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var client in clients)
        {
            Vector3 spawnPosition = PlayerStartPosition;
            Quaternion spawnRotation = Quaternion.identity;
            if (NetworkManager.Singleton.LocalClientId == client.ClientId) // The local client is the host here
            {
                spawnPosition = new Vector3(PlayerStartPosition.x, PlayerStartPosition.y, PlayerStartPosition.z * -1);
            }
            else
            {
                spawnRotation = Quaternion.Euler(0, -180, 0);
            }
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerObject, client.ClientId, false, true, false, spawnPosition, spawnRotation);
            _clientPlayerInfo.Value = new PlayerInfo(client.ClientId.ToString());
        }
    }

    private void Start()
    {
        if(NetworkManager.Singleton == null)
        {
            Debug.LogError("Netcode is not initialized");
            SceneLoader.LoadScene(SceneLoader.Scene.MenuScene);
        }
    }

    private void Update()
    {
        time += (uint)Time.deltaTime;
        debugText.text = time.ToString();

        if(!IsServer) return;
        if (time >= MaxGameTime || _hostPlayerInfo.Value.Score >= MaxScore || _clientPlayerInfo.Value.Score >= MaxScore)
        {
            EndGame();
        }
    }

    public void EndTurn(bool hostWon)
    {
        if (hostWon)
        {
            _hostPlayerInfo.Value.Score++;
        }
        else
        {
            _clientPlayerInfo.Value.Score++;
        }
        scoreText.text = "Host: " + _hostPlayerInfo.Value.Score + " Client: " + _clientPlayerInfo.Value.Score;
    }

    private void EndGame()
    {
        // TODO: Implement
        Debug.Log("Game Over");
    }
}
