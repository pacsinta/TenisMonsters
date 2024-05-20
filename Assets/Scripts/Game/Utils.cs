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

        public static bool IsMyPlayer(EPlayerSide playerSide, bool IsHost)
        {
            return (IsHost && playerSide == EPlayerSide.Host) || (!IsHost && playerSide == EPlayerSide.Client);
        }

        // RunOnHost is a helper function that allows you to run a function on the target by passing the function and the RPC function
        public static void RunOnTarget<T>(Action<T> action, Action<T> rpcAction, T parameter, bool targetCondition)
        {
            if(targetCondition)
            {
                action(parameter);
            }
            else
            {
                rpcAction(parameter);
            }
        }
    }
}