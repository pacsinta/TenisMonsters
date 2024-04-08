using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
    public int gameMode;
    public bool gravityPowerballEnabled = true;
    public bool rotationKickPowerballEnabled = true;
    public bool speedPowerballEnabled = true;
    public int powerBallSpawnTime = 10;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref gameMode);
        serializer.SerializeValue(ref gravityPowerballEnabled);
        serializer.SerializeValue(ref rotationKickPowerballEnabled);
        serializer.SerializeValue(ref speedPowerballEnabled);
        serializer.SerializeValue(ref powerBallSpawnTime);
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
        gameMode = mode;
        PlayerPrefs.SetInt("GameMode", mode);
    }
    public int GetGameMode()
    {
        return gameMode;
    }
    public void SetGravityPowerballEnabled(bool enabled)
    {
        gravityPowerballEnabled = enabled;
        PlayerPrefs.SetInt("GravityPowerballEnabled", enabled ? 1 : 0);
    }
    public bool GetGravityPowerballEnabled()
    {
        return gravityPowerballEnabled;
    }
    public void SetRotationKickPowerballEnabled(bool enabled)
    {
        rotationKickPowerballEnabled = enabled;
        PlayerPrefs.SetInt("RotationKickPowerballEnabled", enabled ? 1 : 0);
    }
    public bool GetRotationKickPowerballEnabled()
    {
        return rotationKickPowerballEnabled;
    }
    public void SetSpeedPowerballEnabled(bool enabled)
    {
        speedPowerballEnabled = enabled;
        PlayerPrefs.SetInt("SpeedPowerballEnabled", enabled ? 1 : 0);
    }
    public bool GetSpeedPowerballEnabled()
    {
        return speedPowerballEnabled;
    }
    public void SetPowerBallSpawnTime(float time)
    {
        int seconds = (int)time * 10;
        powerBallSpawnTime = seconds;
        PlayerPrefs.SetInt("PowerBallSpawnTime", seconds);
    }
    public int GetPowerBallSpawnTime()
    {
        return powerBallSpawnTime;
    }
    public GameInfo()
    {
        gameMode = PlayerPrefs.GetInt("GameMode");
        gravityPowerballEnabled = PlayerPrefs.GetInt("GravityPowerballEnabled") == 1;
        rotationKickPowerballEnabled = PlayerPrefs.GetInt("RotationKickPowerballEnabled") == 1;
        speedPowerballEnabled = PlayerPrefs.GetInt("SpeedPowerballEnabled") == 1;
        powerBallSpawnTime = PlayerPrefs.GetInt("PowerBallSpawnTime");

        powerBallSpawnTime = powerBallSpawnTime == 0 ? 10 : powerBallSpawnTime;
    }

    public uint GetMaxTime
    {
        get 
        { 
            switch((GameMode)gameMode)
            {
                case GameMode.QuickGame:
                    return 60 * 5;
                case GameMode.LongGame:
                    return 60 * 10;
                case GameMode.FirstToWin:
                    return 0;
                default:
                    return 0;
            }
        }
    }

    public uint GetMaxScore
    {
        get
        {
            switch((GameMode)gameMode)
            {
                case GameMode.QuickGame:
                    return 0;
                case GameMode.LongGame:
                    return 0;
                case GameMode.FirstToWin:
                    return 10;
                default:
                    return 0;
            }
        }
    }
}
