using Assets.Scripts;
using Unity.Netcode;

struct KickData
{
    private PlayerSide player;
    public PlayerSide Player
    {
        get => player;
        set
        {
            player = value;
            bounced = false;
        }
    }
    public bool bounced; // true if the ball has already bounced on the current turn
    public bool firstKickSuccess; // true after the first kick was successful
}

struct Kick : INetworkSerializable
{
    public float XdirectionForce;
    public float force;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref XdirectionForce);
        serializer.SerializeValue(ref force);
    }
}

struct RotationKick
{
    public bool rotationKick;
    public float rotationKickTime;
    public int rotationKickDirection;
}