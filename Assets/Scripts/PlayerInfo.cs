using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public enum PlayerSide
    {
        Left,
        Right
    }
    public class PlayerInfo : INetworkSerializable
    {
        public FixedString32Bytes PlayerName;
        public int Score;
        public PlayerSide Side;

        public PlayerInfo()
        {
            LoadPlayerInfo();
        }

        public void StorePlayerInfo(string id = "")
        {
            PlayerPrefs.SetString("PlayerName"+id, PlayerName);
            PlayerPrefs.SetInt("Score"+id, Score);
        }

        public void LoadPlayerInfo()
        {
            PlayerName = PlayerPrefs.GetString("PlayerName");
            Score = PlayerPrefs.GetInt("Score");
            Side = PlayerSide.Left;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Score);
            serializer.SerializeValue(ref Side);
        }
    }
}
