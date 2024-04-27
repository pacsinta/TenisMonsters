using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public class LeaderBoard : MonoBehaviour
    {
        public GameObject scrollViewContent;
        public GameObject scrollViewContentPrefab;
        private ConnectionCoroutine<List<LeaderBoardElement>> fetchLeaderboardCoroutine;

        void Start()
        {
            var loadingText = new GameObject("LoadingText");
            loadingText.AddComponent<TextMeshProUGUI>().text = "Loading...";
            loadingText.transform.SetParent(scrollViewContent.transform);

            fetchLeaderboardCoroutine = DatabaseHandler.GetLeaderBoard();
            StartCoroutine(fetchLeaderboardCoroutine.Coroutine());
        }

        private float time = 0;
        private void Update()
        {
            time += Time.deltaTime;
            if (fetchLeaderboardCoroutine.state == LoadingState.DataAvailable)
            {
                removeAllChildren(scrollViewContent);

                var leaderboard = fetchLeaderboardCoroutine.Result;
                if (leaderboard.Count != 0)
                {
                    for (var i = 0; i < leaderboard.Count; i++)
                    {
                        var text = new GameObject("Player" + i);
                        text.AddComponent<TextMeshProUGUI>().text = leaderboard[i].ToString();
                        text.transform.SetParent(scrollViewContent.transform);
                    }
                }
                else
                {
                    var text = new GameObject("Empty leaderboard");
                    text.AddComponent<TextMeshProUGUI>().text = "Leaderboard is empty";
                    text.GetComponent<TextMeshProUGUI>().fontSize = 20;
                    text.transform.SetParent(scrollViewContent.transform);
                }
            }
            else if (fetchLeaderboardCoroutine.state == LoadingState.Error)
            {
                removeAllChildren(scrollViewContent);

                var errorText = new GameObject("ErrorText");
                errorText.AddComponent<TextMeshProUGUI>().text = "Can't fetch the leaderboard";
                errorText.GetComponent<TextMeshProUGUI>().color = Color.red;
                errorText.GetComponent<TextMeshProUGUI>().fontSize = 20;
                errorText.transform.SetParent(scrollViewContent.transform);
            }


            if (time >= 10 &&
                (fetchLeaderboardCoroutine.state == LoadingState.NotLoaded ||
                fetchLeaderboardCoroutine.state == LoadingState.Error))
            {
                Refresh();
            }
            else if(time >= 120)
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            if (fetchLeaderboardCoroutine.Coroutine() != null)
            {
                StopCoroutine(fetchLeaderboardCoroutine.Coroutine());
            }
            fetchLeaderboardCoroutine = DatabaseHandler.GetLeaderBoard();
            StartCoroutine(fetchLeaderboardCoroutine.Coroutine());
            time = 0;
        }

        private void removeAllChildren(GameObject content)
        {
            var parent = content.transform.parent;
            var scrollRect = content.GetComponentInParent<ScrollRect>();
            Destroy(content);
            scrollViewContent = Instantiate(scrollViewContentPrefab, parent);
            scrollRect.content = scrollViewContent.GetComponent<RectTransform>();
        }
    }
}