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

        private void OnEnable()
        {
            RemoveAllChildren(scrollViewContent);
            CreateText(scrollViewContent.transform, "Loading...", "LoadingText");
            Refresh();
        }

        private float time = 0;
        private void Update()
        {
            time += Time.deltaTime;
            if (fetchLeaderboardCoroutine.state == LoadingState.DataAvailable)
            {
                RemoveAllChildren(scrollViewContent);

                var leaderboard = fetchLeaderboardCoroutine.Result;
                if (leaderboard.Count != 0)
                {
                    for (var i = 0; i < leaderboard.Count; i++)
                    {
                        CreateText(scrollViewContent.transform, leaderboard[i].ToString(), "Player" + i);
                    }
                }
                else
                {
                    CreateText(scrollViewContent.transform, "Leaderboard is empty", "EmptyLeaderboardText");
                }
            }
            else if (fetchLeaderboardCoroutine.state == LoadingState.Error)
            {
                RemoveAllChildren(scrollViewContent);
                CreateText(scrollViewContent.transform, "Can't fetch the leaderboard", "ErrorText", Color.red);
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

        private void CreateText(Transform parent, string text, string name, Color? color = null)
        {
            var newText = new GameObject(name);
            newText.AddComponent<TextMeshProUGUI>().text = text;
            if(color != null) newText.GetComponent<TextMeshProUGUI>().color = (Color)color;
            newText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            var parentWidth = parent.GetComponent<RectTransform>().rect.width;
            newText.GetComponent<RectTransform>().sizeDelta = new Vector2(parentWidth, 50);
            newText.transform.SetParent(parent);
        }

        private void Refresh()
        {
            print("Refreshing the leaderboard");
            if (fetchLeaderboardCoroutine?.Coroutine() != null)
            {
                StopCoroutine(fetchLeaderboardCoroutine.Coroutine());
            }
            fetchLeaderboardCoroutine = DatabaseHandler.GetLeaderBoard();
            StartCoroutine(fetchLeaderboardCoroutine.Coroutine());
            time = 0;
        }

        private void RemoveAllChildren(GameObject content)
        {
            var parent = content.transform.parent;
            var scrollRect = content.GetComponentInParent<ScrollRect>();
            Destroy(content);
            scrollViewContent = Instantiate(scrollViewContentPrefab, parent);
            scrollRect.content = scrollViewContent.GetComponent<RectTransform>();
        }
    }
}