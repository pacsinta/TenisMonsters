using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public NetworkObject playerPrefab;
    public NetworkObject powerBallPrefab;
    public GameObject ballObject;
    public GameObject ground;

    public Vector3 PlayerStartPosition;
    public float ZOffsetOfNet;
    public uint PowerBallSpawnTime = 10u;

    private GameObject hostPlayerObject;
    private GameObject clientPlayerObject;

    NetworkVariable<PlayerInfo> _hostPlayerInfo = new NetworkVariable<PlayerInfo>();
    NetworkVariable<PlayerInfo> _clientPlayerInfo = new NetworkVariable<PlayerInfo>();
    GameInfo gameInfo;
    private float time = 0;
    private float remainingTimeToSpawnPowerBall = 0;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        _hostPlayerInfo.Value = new PlayerInfo();
        gameInfo = new GameInfo();

        var clients = NetworkManager.Singleton.ConnectedClientsList;

        foreach (var client in clients)
        {
            instantiatePlayerObject(client);
            _clientPlayerInfo.Value = new PlayerInfo(client.ClientId.ToString());
        }
    }

    private void instantiatePlayerObject(NetworkClient client)
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
        var playerObject = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(playerPrefab, client.ClientId, false, true, true, spawnPosition, spawnRotation);
        if (NetworkManager.Singleton.LocalClientId == client.ClientId)
        {
            hostPlayerObject = playerObject.gameObject;
        }
        else
        {
            clientPlayerObject = playerObject.gameObject;
        }
    }

    private void Update()
    {
        HandleNotConnected();

        time += Time.deltaTime;
        remainingTimeToSpawnPowerBall += Time.deltaTime;

        timeText.text = gameInfo.GetMaxTime != 0 ? ConvertSecondsToTimeString(gameInfo.GetMaxTime - ((uint)time)) : "";
        scoreText.text = _hostPlayerInfo.Value?.Score + " - " + _clientPlayerInfo.Value?.Score;

        if (!IsServer) return;
        if (TimeEnded() || ScoreReached())
        {
            EndGame();
        }

        if(remainingTimeToSpawnPowerBall >= gameInfo.GetPowerBallSpawnTime())
        {
            SpawnPowerBall(PlayerSide.Host);
            SpawnPowerBall(PlayerSide.Client);
            remainingTimeToSpawnPowerBall = 0;
        }
    }

    private string ConvertSecondsToTimeString(uint seconds)
    {
        int minutes = (int)seconds / 60;
        int remainingSeconds = (int)seconds % 60;
        return minutes + ":" + remainingSeconds;
    }

    private void HandleNotConnected()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Netcode is not initialized");
            ExitGame();
        }
        else if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogError("Player is not connected to the server!");
            ExitGame();
        }
        else if (IsHost && NetworkManager.Singleton.ConnectedClients.Count != 2)
        {
            Debug.LogError("No opponent is connected");
            ExitGame();
        }
    }

    private void ExitGame()
    {
        NetworkManager.Singleton?.Shutdown();
        SceneLoader.LoadScene(SceneLoader.Scene.MenuScene);
    }

    private bool TimeEnded()
    {
        uint maxTime = gameInfo.GetMaxTime;
        return maxTime != 0 && time >= maxTime;
    }
    private bool ScoreReached()
    {
        uint maxScore = gameInfo.GetMaxScore;
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
        var position = PlayerStartPosition;
        clientPlayerObject.GetComponent<PlayerController>().ResetObject(position);
        position.z *= -1;
        hostPlayerObject.GetComponent<PlayerController>().ResetObject(position);
    }

    private ConnectionCoroutine<LeaderBoardElement> uploadScoreCoroutine;
    private void EndGame()
    {
        // TODO: Implement
        Debug.Log("Game Over");
        bool hostWon = true;

        int hostScore = hostWon ? 2 : -1;
        int clientScore = hostWon ? -1 : 2;

        if(IsHost)
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(_hostPlayerInfo.Value.PlayerName.ToSafeString(), hostScore);
        }
        else
        {
            uploadScoreCoroutine = DatabaseHandler.SetMyPoints(_clientPlayerInfo.Value.PlayerName.ToSafeString(), clientScore);
        }
    }

    private void SpawnPowerBall(PlayerSide side)
    {
        // create random position
        var groundSize = ground.transform.localScale;
        Vector3 spawnPosition = new Vector3(Random.Range(0, groundSize.x - 1), 0.5f, Random.Range(0, groundSize.z - 1));
        if(side == PlayerSide.Host)
        {
            spawnPosition.z *= -1;
        }

        NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(powerBallPrefab, 0, true, false, false, spawnPosition);
    }
}
