using Assets.Scripts.Networking;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu.Leaderboard
{
    public class LeaderBoard : MonoBehaviour
    {
        public GameObject scrollViewContent;
        public GameObject scrollViewContentPrefab;
        public TMP_InputField maxPlayerCount;
        private ConnectionCoroutine<List<LeaderBoardElement>> fetchLeaderboardCoroutine;

        private void Start()
        {
            maxPlayerCount.onValueChanged.AddListener((string value) => 
            { 
                if (string.IsNullOrEmpty(value) || !value.All(c => char.IsDigit(c)))
                {
                    maxPlayerCount.text = "";
                    return;
                }
                    
                Refresh();
            });
        }

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
            if (fetchLeaderboardCoroutine.state == ELoadingState.DataAvailable)
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
            else if (fetchLeaderboardCoroutine.state == ELoadingState.Error)
            {
                RemoveAllChildren(scrollViewContent);
                CreateText(scrollViewContent.transform, "Can't fetch the leaderboard", "ErrorText", Color.red);
            }


            if (time >= 10 &&
                (fetchLeaderboardCoroutine.state == ELoadingState.NotLoaded ||
                fetchLeaderboardCoroutine.state == ELoadingState.Error))
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
            
            if (fetchLeaderboardCoroutine?.Coroutine() != null)
            {
                StopCoroutine(fetchLeaderboardCoroutine.Coroutine());
            }
            int maxPlayers = -1;
            if(!string.IsNullOrEmpty(maxPlayerCount.text))
            {
                try
                {
                    maxPlayers = int.Parse(maxPlayerCount.text);
                }
                catch
                {
                    maxPlayers = -1;
                }
            }
            print("Refreshing the leaderboard with " + maxPlayers + " maxPlayerCount");
            fetchLeaderboardCoroutine = DatabaseHandler.GetLeaderBoard(maxPlayers);
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