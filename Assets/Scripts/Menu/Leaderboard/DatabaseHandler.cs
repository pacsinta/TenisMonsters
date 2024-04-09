using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public static class DatabaseHandler
    {
        public static string url = "http://localhost:6000/";
        public static ConnectionCoroutine<List<LeaderBoardElement>> GetLeaderBoard()
        {
            var www = UnityWebRequest.Get(url + "leaderboard");
            return new ConnectionCoroutine<List<LeaderBoardElement>>(www);
        }

        public static ConnectionCoroutine<LeaderBoardElement> GetMyPoints(string playerName)
        {
            var www = UnityWebRequest.Get(url + "score/" + playerName);
            return new ConnectionCoroutine<LeaderBoardElement>(www);
        }

        public static ConnectionCoroutine<LeaderBoardElement> SetMyPoints(string playerName, int points)
        {
            var www = new UnityWebRequest(url + "score/" + playerName, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new LeaderBoardElement(playerName, points)));
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            return new ConnectionCoroutine<LeaderBoardElement>(www);
        }
    }
}
