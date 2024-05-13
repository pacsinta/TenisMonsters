using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public enum EPlayerSide
    {
        Host = 0,
        Client = 1
    }
    public class PlayerInfo : INetworkSerializable
    {
        public FixedString32Bytes PlayerName;
        public int Score = 0;

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
            PlayerPrefs.SetString("PlayerName" + id, PlayerName.ToString());
        }

        private void LoadPlayerInfo(string id = "")
        {
            PlayerName = PlayerPrefs.GetString("PlayerName" + id);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Score);
        }
    }
}
