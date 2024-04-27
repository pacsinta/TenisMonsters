using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public NetworkObject playerPrefab;
    public GameObject ballObject;
    public GameObject ground;
    public Canvas endCanvas;
    public GameObject walls;

    // Powerball prefabs
    public NetworkObject gravityPowerBallPrefab;
    public NetworkObject speedPowerBallPrefab;
    public NetworkObject rotationKickPowerBallPrefab;

    public Vector3 PlayerStartPosition;
    public float ZOffsetOfNet;
    public uint PowerBallSpawnTime = 10u;

    private GameObject hostPlayerObject;
    private GameObject clientPlayerObject;

    readonly NetworkVariable<PlayerInfo> _hostPlayerInfo = new();
    readonly NetworkVariable<PlayerInfo> _clientPlayerInfo = new();
    readonly NetworkVariable<GameInfo> _gameInfo = new();
    readonly NetworkVariable<float> time = new();
    readonly NetworkVariable<bool> gameEnd = new(false);
    private bool timeCounting = false;
    private float remainingTimeToSpawnPowerBall = 0;

    private void Start()
    {
        endCanvas.gameObject.SetActive(false);
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _hostPlayerInfo.Value = new PlayerInfo();
            _gameInfo.Value = new GameInfo();

            var clients = NetworkManager.Singleton.ConnectedClientsList;

            foreach (var client in clients)
            {
                instantiatePlayerObject(client);
                _clientPlayerInfo.Value = new PlayerInfo(client.ClientId.ToString());
            }
        }
        
        walls.SetActive(_gameInfo.Value.wallsEnabled);
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
        playerObject.GetComponent<PlayerController>().Environment = gameObject;
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
        
        checkEndGame();
        updateTexts();

        // Pass the environment to the player objects
        // Because the client object has a separate version on the client side, we need to pass the environment to it as well
        if(clientPlayerObject == null)
        {
            clientPlayerObject = GameObject.Find("ClientPlayer");
            if(clientPlayerObject != null)
                clientPlayerObject.GetComponent<PlayerController>().Environment = gameObject;
        }

        if (!IsServer || _gameInfo.Value == null) return;

        if (timeCounting)
        {
            time.Value += Time.deltaTime;
            remainingTimeToSpawnPowerBall += Time.deltaTime;
        }

        if (remainingTimeToSpawnPowerBall >= _gameInfo.Value.powerBallSpawnTime)
        {
            SpawnPowerBall(PlayerSide.Host, _gameInfo.Value.GetAllPowerballEnabled());
            SpawnPowerBall(PlayerSide.Client, _gameInfo.Value.GetAllPowerballEnabled());
            remainingTimeToSpawnPowerBall = 0;
        }
    }

    void checkEndGame()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if(IsHost)
            {
                EndGameClientRPC(PlayerSide.Client);
            }
            else
            {
                EndGameServerRPC(PlayerSide.Host);
                EndGame(PlayerSide.Host);
            }
        }

        if (gameEnd.Value)
        {
            EndGame();
            return;
        }

        if(IsHost)
        {
            if (TimeEnded() || ScoreReached())
            {
                EndGame();
            }
        }
    }

    void updateTexts()
    {
        timeText.text = _gameInfo.Value.GetMaxTime != 0 ? ConvertSecondsToTimeString(_gameInfo.Value.GetMaxTime - ((uint)time.Value)) : "";
        scoreText.text = _hostPlayerInfo.Value?.Score + " - " + _clientPlayerInfo.Value?.Score;
    }

    private string ConvertSecondsToTimeString(uint seconds)
    {
        int minutes = (int)seconds / 60;
        int remainingSeconds = (int)seconds % 60;
        return minutes + ":" + remainingSeconds;
    }

    private bool TimeEnded()
    {
        uint maxTime = _gameInfo.Value.GetMaxTime;
        return maxTime != 0 && time.Value >= maxTime;
    }
    private bool ScoreReached()
    {
        uint maxScore = _gameInfo.Value.GetMaxScore;
        return maxScore != 0 && (_hostPlayerInfo.Value.Score >= maxScore || _clientPlayerInfo.Value.Score >= maxScore);
    }

    public void EndTurn(PlayerSide winner)
    {
        if(!IsServer)
        {
            EndTurnServerRPC(winner);
            return;
        }

        if (winner == PlayerSide.Host)
        {
            _hostPlayerInfo.Value.Score++;
            _hostPlayerInfo.IsDirty();
        }
        else
        {
            _clientPlayerInfo.Value.Score++;
            _clientPlayerInfo.IsDirty();
        }

        ResetEnvironment();
        timeCounting = false;
    }

    [ServerRpc(RequireOwnership = false)]
    void EndTurnServerRPC(PlayerSide winner)
    {
        EndTurn(winner);
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


    /*
     * If you don't pass a winner, it means the winner is calulated by the scores.
     * Otherwise the winner is the one passed as argument.
     */
    private void EndGame(PlayerSide? winner = null)
    {
        if (endCanvas.gameObject.activeSelf) return; // already instantiated gameEnd
        Debug.Log("Game Over");
        if (IsHost)
        {
            Destroy(ballObject);
            gameEnd.Value = true;
        }

        timeCounting = false;

        if(winner == null)
        {
            // If the scores are equal, it's a draw, and the winner remains null
            if (_clientPlayerInfo.Value.Score > _hostPlayerInfo.Value.Score)
            {
                winner = PlayerSide.Client;
            }
            else if (_hostPlayerInfo.Value.Score > _clientPlayerInfo.Value.Score)
            {
                winner = PlayerSide.Host;
            }
        }

        endCanvas.gameObject.SetActive(true);
        endCanvas.GetComponent<EndHandler>().InstantiateGameEnd(winner,
                                                                _clientPlayerInfo.Value.PlayerName.ToString(),
                                                                _hostPlayerInfo.Value.PlayerName.ToString(),
                                                                IsHost);

        hostPlayerObject.SetActive(false);
        clientPlayerObject.SetActive(false);
        gameObject.SetActive(false);
    }
    [ServerRpc(RequireOwnership = false)]
    public void EndGameServerRPC(PlayerSide winner)
    {
        gameEnd.Value = true;
        EndGame(winner);
    }
    [ClientRpc]
    public void EndGameClientRPC(PlayerSide winner)
    {
        EndGame(winner);
    }

    public void StartGame()
    {
        timeCounting = true;
    }

    private void SpawnPowerBall(PlayerSide side, EnabledPowerBalls enabled)
    {
        // Create a shuffled list of rthe enabled power balls
        var enabledList = new List<PowerEffects>();
        if (enabled.GravityPowerBall) enabledList.Add(PowerEffects.Gravitychange);
        if (enabled.RotationPowerBall) enabledList.Add(PowerEffects.BallRotate);
        if (enabled.SpeedPowerBall) enabledList.Add(PowerEffects.SpeedIncrease);
        if (enabledList.Count == 0)
        {
            print("No power balls enabled");
            return;
        }
        enabledList = enabledList.OrderBy(x => Random.Range(0, enabledList.Count)).ToList();

        // Select the correct prefab based on the first element of the shuffled list
        NetworkObject selectedPowerballPrefab = null;
        switch (enabledList[0])
        {
            case PowerEffects.Gravitychange:
                selectedPowerballPrefab = gravityPowerBallPrefab;
                break;
            case PowerEffects.SpeedIncrease:
                selectedPowerballPrefab = speedPowerBallPrefab;
                break;
            case PowerEffects.BallRotate:
                selectedPowerballPrefab = rotationKickPowerBallPrefab;
                break;
            default:
                selectedPowerballPrefab = gravityPowerBallPrefab; // Safe value, but should never be reached
                break;
        }

        // Create random position for the powerball
        var groundSize = ground.transform.localScale * 10 / 2;
        Vector3 spawnPosition = new Vector3(Random.Range(-groundSize.x + 1, groundSize.x - 1),
                                                         0.5f,
                                                         Random.Range(0, groundSize.z - 1));

        if (side == PlayerSide.Host)
        {
            spawnPosition.z *= -1;
        }

        var ball = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(selectedPowerballPrefab, 0, true, false, false, spawnPosition);
        ball.GetComponent<PowerBallController>().powerBallLiveTime = _gameInfo.Value.multiplePowerBalls ? -1 : _gameInfo.Value.powerBallLiveTime;
    }
}
