using Assets.Scripts.Menu.Leaderboard;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public static class DatabaseHandler
    {
        private const string url = "http://localhost:6000/";
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
            var www = new UnityWebRequest(url + "score/" + playerName, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(points.ToString() + ";" + password);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "text/plain");
            return new ConnectionCoroutine<object>(www, false);
        }

        public static ConnectionCoroutine<object> CheckAuth(string playerName, string password)
        {
            var www = new UnityWebRequest(url + "auth/" + playerName, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(password);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "text/plain");
            return new ConnectionCoroutine<object>(www, false);
        }

        public static ConnectionCoroutine<object> ChangePassword(string playerName, string oldPassword, string newPassword)
        {
            var www = new UnityWebRequest(url + "auth/change/" + playerName, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(oldPassword + ";" + newPassword);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "text/plain");
            return new ConnectionCoroutine<object>(www, false);
        }
    }
}
