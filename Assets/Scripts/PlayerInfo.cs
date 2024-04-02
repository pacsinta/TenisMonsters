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
    /*
     * Enum to represent the side of the player
     * The host side position is in the negative z axis
     * The client side position is in the positive z axis 
     * 
     * The players are differenciated by their Network id
     */
    public enum PlayerSide
    {
        Host,
        Client
    }
    public class PlayerInfo : INetworkSerializable
    {
        public FixedString32Bytes PlayerName;
        public int Score;
        public PlayerSide Side;

        public PlayerInfo(string id)
        {
            LoadPlayerInfo(id);
        }

        public PlayerInfo()
        {
            LoadPlayerInfo();
        }

        public void StorePlayerInfo(string id = "")
        {
            PlayerPrefs.SetString("PlayerName"+id, PlayerName.ToString());
            PlayerPrefs.SetInt("Score"+id, Score);
        }

        public void LoadPlayerInfo(string id = "")
        {
            PlayerName = PlayerPrefs.GetString("PlayerName"+id);
            Score = PlayerPrefs.GetInt("Score"+id);
            Side = PlayerSide.Host;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Score);
            serializer.SerializeValue(ref Side);
        }
    }
}
