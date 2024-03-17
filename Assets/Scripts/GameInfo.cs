using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum GameMode
{
    QuickGame = 0,
    LongGame = 1,
    FirstToWin = 2
}

public class GameInfo : INetworkSerializable
{
    public int gameMode;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref gameMode);
    }
    public void SaveGameInfo()
    {
        PlayerPrefs.SetInt("GameMode", gameMode);
    }
    public GameInfo()
    {
        gameMode = PlayerPrefs.GetInt("GameMode");
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
