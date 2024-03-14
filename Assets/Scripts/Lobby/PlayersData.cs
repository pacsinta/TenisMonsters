using Unity.Collections;
using Unity.Netcode;

struct PlayersData : INetworkSerializable
{
    public FixedString32Bytes hostPlayerName;
    public bool HostOnLeftSide;
    public FixedString32Bytes clientPlayerName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref hostPlayerName);
        serializer.SerializeValue(ref clientPlayerName);
        serializer.SerializeValue(ref HostOnLeftSide);
    }
}