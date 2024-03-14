using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class DatabaseHandler
    {
        static HttpClient client = new HttpClient();
        private static string url = "http://localhost:5000/";
        public static async void SetPoint(string name, int score)
        {
            HttpResponseMessage response = await client.PostAsync(url + "setPoint?name=" + name + "&score=" + score, null);
            response.EnsureSuccessStatusCode();
        }
        public static async void GetPoints(string name)
        { }
    }
}
