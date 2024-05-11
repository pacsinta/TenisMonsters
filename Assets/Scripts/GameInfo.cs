using Assets.Scripts.Game;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    

    public struct EnabledPowerBalls : INetworkSerializeByMemcpy
    {
        public bool GravityPowerBall;
        public bool SpeedPowerBall;
        public bool RotationPowerBall;
    }

    public class GameInfo : INetworkSerializable
    {
        private EGameMode gameMode;
        private PowerBallInfo powerBallInfo = new()
        { multiplePowerBalls = true, powerBallLiveTime = 5, powerBallSpawnTime = 10 };
        private EnabledPowerBalls enabledPowerBalls = new()
        { GravityPowerBall = true, SpeedPowerBall = true, RotationPowerBall = true };
        private bool wallsEnabled = true;
        private SkyType skyType = SkyType.Sunny;
        private float timeSpeed = 1;

        // Get properties
        public EGameMode GameMode => gameMode;
        public bool GravityPowerballEnabled => enabledPowerBalls.GravityPowerBall;
        public bool RotationKickPowerballEnabled => enabledPowerBalls.RotationPowerBall;
        public bool SpeedPowerballEnabled => enabledPowerBalls.SpeedPowerBall;
        public int PowerBallSpawnTime => powerBallInfo.powerBallSpawnTime;
        public int PowerBallLiveTime => powerBallInfo.powerBallLiveTime;
        public bool MultiplePowerBalls => powerBallInfo.multiplePowerBalls;
        public bool WallsEnabled => wallsEnabled;
        public SkyType SkyType => skyType;
        public float TimeSpeed => timeSpeed;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref gameMode);
            serializer.SerializeValue(ref enabledPowerBalls);
            serializer.SerializeValue(ref powerBallInfo);
            serializer.SerializeValue(ref wallsEnabled);
            serializer.SerializeValue(ref skyType);
            serializer.SerializeValue(ref timeSpeed);
        }

        public enum EGameMode
        {
            QuickGame = 0,
            LongGame = 1,
            FirstToWin = 2
        }
        private struct PowerBallInfo : INetworkSerializeByMemcpy
        {
            public bool multiplePowerBalls;
            public int powerBallLiveTime;
            public int powerBallSpawnTime;
        }


        // Setters
        public void SetGameMode(int mode)
        {
            gameMode = (EGameMode)mode;
            PlayerPrefs.SetInt("GameMode", mode);
        }
        public void SetGravityPowerballEnabled(bool enabled)
        {
            enabledPowerBalls.GravityPowerBall = enabled;
            PlayerPrefs.SetInt("GravityPowerballEnabled", enabled ? 1 : 0);
        }
        public void SetRotationKickPowerballEnabled(bool enabled)
        {
            enabledPowerBalls.RotationPowerBall = enabled;
            PlayerPrefs.SetInt("RotationKickPowerballEnabled", enabled ? 1 : 0);
        }
        public void SetSpeedPowerballEnabled(bool enabled)
        {
            enabledPowerBalls.SpeedPowerBall = enabled;
            PlayerPrefs.SetInt("SpeedPowerballEnabled", enabled ? 1 : 0);
        }
        public void SetPowerBallSpawnTime(float time)
        {
            int seconds = (int)time * 10;
            powerBallInfo.powerBallSpawnTime = seconds;
            PlayerPrefs.SetInt("PowerBallSpawnTime", seconds);
        }
        public void SetPowerBallLiveTime(float time)
        {
            int seconds = (int)time * 5;
            powerBallInfo.powerBallLiveTime = seconds;
            PlayerPrefs.SetInt("PowerBallLiveTime", seconds);
        }
        public void SetMultiplePowerBalls(bool enabled)
        {
            powerBallInfo.multiplePowerBalls = enabled;
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
            gameMode = (EGameMode)PlayerPrefs.GetInt("GameMode");
            enabledPowerBalls.GravityPowerBall = PlayerPrefs.GetInt("GravityPowerballEnabled") == 1;
            enabledPowerBalls.RotationPowerBall = PlayerPrefs.GetInt("RotationKickPowerballEnabled") == 1;
            enabledPowerBalls.SpeedPowerBall = PlayerPrefs.GetInt("SpeedPowerballEnabled") == 1;
            powerBallInfo.powerBallSpawnTime = PlayerPrefs.GetInt("PowerBallSpawnTime");
            powerBallInfo.powerBallLiveTime = PlayerPrefs.GetInt("PowerBallLiveTime", 5);
            powerBallInfo.multiplePowerBalls = PlayerPrefs.GetInt("MultiplePowerBalls") == 1;
            wallsEnabled = PlayerPrefs.GetInt("WallsEnabled", 1) == 1;
            skyType = (SkyType)PlayerPrefs.GetInt("skyType", 0);
            timeSpeed = PlayerPrefs.GetFloat("TimeSpeed", 1);

            powerBallInfo.powerBallSpawnTime = powerBallInfo.powerBallSpawnTime == 0 ? 10 : powerBallInfo.powerBallSpawnTime;
        }

        public EnabledPowerBalls GetAllPowerballEnabled()
        {
            return enabledPowerBalls;
        }

        public uint GetMaxTime
        {
            get
            {
                return gameMode switch
                {
                    EGameMode.QuickGame => 60 * 5,
                    EGameMode.LongGame => 60 * 10,
                    EGameMode.FirstToWin => 0,
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
                    EGameMode.QuickGame => 0,
                    EGameMode.LongGame => 0,
                    EGameMode.FirstToWin => 10,
                    _ => 0,
                };
            }
        }
    }
}