using System;

namespace Assets.Scripts
{
    public class Utils
    {
        public static T Swap<T>(T value) where T : Enum
        {
            int converted = Convert.ToInt32(value);
            converted = converted == 0 ? 1 : 0;
            return (T)Enum.ToObject(typeof(T), converted);
        }

        public static void Swap<T>(ref T value) where T : Enum
        {
            int converted = Convert.ToInt32(value);
            converted = converted == 0 ? 1 : 0;
            value = (T)Enum.ToObject(typeof(T), converted);
        }

        public static bool IsMyPlayer(PlayerSide playerSide, bool IsHost)
        {
            return (IsHost && playerSide == PlayerSide.Host) || (!IsHost && playerSide == PlayerSide.Client);
        }
    }
}