using Assets.Scripts.Menu.Leaderboard;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public static class DatabaseHandler
    {
        public const string url = "http://localhost:6000/";
        public static ConnectionCoroutine<List<LeaderBoardElement>> GetLeaderBoard(int maxPlayerCount = -1)
        {
            var www = UnityWebRequest.Get(url + "leaderboard");
            if (maxPlayerCount != -1)
            {
                www.url += "?limit=" + maxPlayerCount;
            }
            return new ConnectionCoroutine<List<LeaderBoardElement>>(www);
        }

        public static ConnectionCoroutine<LeaderBoardElement> GetMyPoints(string playerName)
        {
            var www = UnityWebRequest.Get(url + "score/" + playerName);
            return new ConnectionCoroutine<LeaderBoardElement>(www);
        }

        public static ConnectionCoroutine<object> SetMyPoints(string playerName, int points, string password)
        {
            string fullUrl = url + "score/" + playerName;
            var type = ERequestType.POST;
            string body = points.ToString() + ";" + password;
            return new ConnectionCoroutine<object>(fullUrl, type, body, false);
        }

        public static ConnectionCoroutine<object> CheckAuth(string playerName, string password)
        {
            string fullUrl = url + "auth/" + playerName;
            var type = ERequestType.POST;
            string body = password;
            return new ConnectionCoroutine<object>(fullUrl, type, body, false);
        }

        public static ConnectionCoroutine<object> ChangePassword(string playerName, string oldPassword, string newPassword)
        {
            string fullUrl = url + "auth/change/" + playerName;
            var type = ERequestType.POST;
            string body = oldPassword + ";" + newPassword;
            return new ConnectionCoroutine<object>(fullUrl, type, body, false);
        }
    }
}
