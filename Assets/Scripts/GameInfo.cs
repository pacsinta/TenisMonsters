using System;
using Unity.Netcode;
using UnityEngine;

public enum GameMode
{
    QuickGame = 0,
    LongGame = 1,
    FirstToWin = 2
}

public struct EnabledPowerBalls
{
    public bool GravityPowerBall;
    public bool SpeedPowerBall;
    public bool RotationPowerBall;
}

public class GameInfo : INetworkSerializable
{
    private GameMode gameMode;
    public GameMode GameMode => gameMode;
    private bool gravityPowerballEnabled = true;
    public bool GravityPowerballEnabled => gravityPowerballEnabled;
    private bool rotationKickPowerballEnabled = true;
    public bool RotationKickPowerballEnabled => rotationKickPowerballEnabled;
    private bool speedPowerballEnabled = true;
    public bool SpeedPowerballEnabled => speedPowerballEnabled;
    private int powerBallSpawnTime = 10;
    public int PowerBallSpawnTime => powerBallSpawnTime;
    private int powerBallLiveTime = 5;
    public int PowerBallLiveTime => powerBallLiveTime;
    private bool multiplePowerBalls = true;
    public bool MultiplePowerBalls => multiplePowerBalls;
    private bool wallsEnabled = true;
    public bool WallsEnabled => wallsEnabled;
    private SkyType skyType = SkyType.Sunny;
    public SkyType SkyType => skyType;
    private float timeSpeed = 1;
    public float TimeSpeed => timeSpeed;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref gameMode);
        serializer.SerializeValue(ref gravityPowerballEnabled);
        serializer.SerializeValue(ref rotationKickPowerballEnabled);
        serializer.SerializeValue(ref speedPowerballEnabled);
        serializer.SerializeValue(ref powerBallSpawnTime);
        serializer.SerializeValue(ref powerBallLiveTime);
        serializer.SerializeValue(ref multiplePowerBalls);
        serializer.SerializeValue(ref wallsEnabled);
        serializer.SerializeValue(ref skyType);
        serializer.SerializeValue(ref timeSpeed);
    }
    public EnabledPowerBalls GetAllPowerballEnabled()
    {
        return new EnabledPowerBalls
        {
            GravityPowerBall = gravityPowerballEnabled,
            SpeedPowerBall = speedPowerballEnabled,
            RotationPowerBall = rotationKickPowerballEnabled
        };
    }

    public void SetGameMode(int mode)
    {
        gameMode = (GameMode)mode;
        PlayerPrefs.SetInt("GameMode", mode);
    }
    public void SetGravityPowerballEnabled(bool enabled)
    {
        gravityPowerballEnabled = enabled;
        PlayerPrefs.SetInt("GravityPowerballEnabled", enabled ? 1 : 0);
    }
    public void SetRotationKickPowerballEnabled(bool enabled)
    {
        rotationKickPowerballEnabled = enabled;
        PlayerPrefs.SetInt("RotationKickPowerballEnabled", enabled ? 1 : 0);
    }
    public void SetSpeedPowerballEnabled(bool enabled)
    {
        speedPowerballEnabled = enabled;
        PlayerPrefs.SetInt("SpeedPowerballEnabled", enabled ? 1 : 0);
    }
    public void SetPowerBallSpawnTime(float time)
    {
        int seconds = (int)time * 10;
        powerBallSpawnTime = seconds;
        PlayerPrefs.SetInt("PowerBallSpawnTime", seconds);
    }
    public void SetPowerBallLiveTime(float time)
    {
        int seconds = (int)time * 5;
        powerBallLiveTime = seconds;
        PlayerPrefs.SetInt("PowerBallLiveTime", seconds);
    }
    public void SetMultiplePowerBalls(bool enabled)
    {
        multiplePowerBalls = enabled;
        PlayerPrefs.SetInt("MultiplePowerBalls", enabled ? 1 : 0);
    }
    public void SetWallsEnabled(bool enabled)
    {
        wallsEnabled = enabled;
        PlayerPrefs.SetInt("WallsEnabled", enabled ? 1 : 0);
    }
    public void SetSkyType(int type)
    {
        skyType = (SkyType)type;
        PlayerPrefs.SetInt("skyType", type);
    }

    public void SetTimeSpeed(float speed)
    {
        timeSpeed = speed;
        PlayerPrefs.SetFloat("TimeSpeed", speed);
    }
    public GameInfo()
    {
        gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");
        gravityPowerballEnabled = PlayerPrefs.GetInt("GravityPowerballEnabled") == 1;
        rotationKickPowerballEnabled = PlayerPrefs.GetInt("RotationKickPowerballEnabled") == 1;
        speedPowerballEnabled = PlayerPrefs.GetInt("SpeedPowerballEnabled") == 1;
        powerBallSpawnTime = PlayerPrefs.GetInt("PowerBallSpawnTime");
        powerBallLiveTime = PlayerPrefs.GetInt("PowerBallLiveTime", 5);
        multiplePowerBalls = PlayerPrefs.GetInt("MultiplePowerBalls") == 1;
        wallsEnabled = PlayerPrefs.GetInt("WallsEnabled", 1) == 1;
        skyType = (SkyType)PlayerPrefs.GetInt("skyType", 0);
        timeSpeed = PlayerPrefs.GetFloat("TimeSpeed", 1);

        powerBallSpawnTime = powerBallSpawnTime == 0 ? 10 : powerBallSpawnTime;
    }

    public uint GetMaxTime
    {
        get
        {
            return gameMode switch
            {
                GameMode.QuickGame => 60 * 5,
                GameMode.LongGame => 60 * 10,
                GameMode.FirstToWin => 0,
                _ => 0,
            };
        }
    }

    public uint GetMaxScore
    {
        get
        {
            return gameMode switch
            {
                GameMode.QuickGame => 0,
                GameMode.LongGame => 0,
                GameMode.FirstToWin => 10,
                _ => 0,
            };
        }
    }
}
