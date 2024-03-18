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
    public NetworkObject playerPrefab;
    public NetworkObject powerBallPrefab;
    public GameObject ballObject;

    public Vector3 PlayerStartPosition;
    public float ZOffsetOfNet;

    NetworkVariable<PlayerInfo> _hostPlayerInfo = new NetworkVariable<PlayerInfo>();
    NetworkVariable<PlayerInfo> _clientPlayerInfo = new NetworkVariable<PlayerInfo>();
    GameInfo GameInfo;
    private float time = 0;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _hostPlayerInfo.Value = new PlayerInfo();
        GameInfo = new GameInfo();
        Debug.Log("Game mode: " + (GameMode)GameInfo.gameMode);
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
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, client.ClientId, true, true, true, spawnPosition, spawnRotation);
            _clientPlayerInfo.Value = new PlayerInfo(client.ClientId.ToString());
        }
    }

    private void Update()
    {
        //HandleNotConnected();

        time += Time.deltaTime;
        debugText.text = ((uint)time).ToString();
        scoreText.text = "Host: " + _hostPlayerInfo.Value?.Score + " Client: " + _clientPlayerInfo.Value?.Score;

        if (!IsServer) return;
        if (TimeEnded() || ScoreReached())
        {
            EndGame();
        }

        if(time % 60 == 0)
        {
            SpawnPowerBall(PlayerSide.Host);
            SpawnPowerBall(PlayerSide.Client);
        }
    }

    private void HandleNotConnected()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Netcode is not initialized");
            SceneLoader.LoadScene(SceneLoader.Scene.MenuScene);
        }
        else if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogError("Player is not connected to the server!");
            SceneLoader.LoadScene(SceneLoader.Scene.MenuScene);
        }
    }

    private bool TimeEnded()
    {
        uint maxTime = GameInfo.GetMaxTime;
        return maxTime != 0 && time >= maxTime;
    }
    private bool ScoreReached()
    {
        uint maxScore = GameInfo.GetMaxScore;
        return maxScore != 0 && (_hostPlayerInfo.Value.Score >= maxScore || _clientPlayerInfo.Value.Score >= maxScore);
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
        
        ResetEnvironment();
    }

    private void ResetEnvironment()
    {
        var ballController = ballObject.GetComponent<BallController>();
        ballController.ResetObject();
    }

    private void EndGame()
    {
        // TODO: Implement
        Debug.Log("Game Over");
    }

    private void SpawnPowerBall(PlayerSide side)
    {
        // create random position
        Vector3 spawnPosition = new Vector3(Random.Range(0, 10), 0.5f, Random.Range(0, 10));
        if(side == PlayerSide.Host)
        {
            spawnPosition.z *= -1;
        }

        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(powerBallPrefab, 0, true, false, true, spawnPosition);
    }
}
