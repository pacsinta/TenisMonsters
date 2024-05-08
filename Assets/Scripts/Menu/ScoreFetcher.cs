using Assets.Scripts.Menu.Leaderboard;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TMP_InputField playerName;
    public TextMeshProUGUI myPontsText;
    private ConnectionCoroutine<LeaderBoardElement> myPointCoroutine;

    void Start()
    {
        playerName.onValueChanged.AddListener(PlayerNameChange);
        RefreshMyPoints();
    }

    private float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (myPointCoroutine?.state == LoadingState.DataAvailable)
        {
            Debug.Log("MyPoints loaded: " + myPointCoroutine.Result.ToString());
            myPontsText.text = "MyScore: " + myPointCoroutine.Result.Score;
        }
        else if (myPointCoroutine?.state == LoadingState.Error || myPointCoroutine?.state == LoadingState.NotLoaded)
        {
            if(myPointCoroutine?.state == LoadingState.Error) myPontsText.text = "MyScore: -";
            if (time >= 10)
            {
                RefreshMyPoints();
            }
        }
        else if (time >= 60)
        {
            RefreshMyPoints();
        }
    }

    void PlayerNameChange(string name)
    {
        time = 57; // set the time so that if the name is stopped being changed, the points will be refreshed
    }

    void RefreshMyPoints()
    {
        string name = playerName.text;
        if (name == null || string.IsNullOrEmpty(name)) return;

        if (myPointCoroutine?.Coroutine() != null)
        {
            StopCoroutine(myPointCoroutine.Coroutine());
        }
        myPointCoroutine = DatabaseHandler.GetMyPoints(name);
        StartCoroutine(myPointCoroutine.Coroutine());
        time = 0;
    }
}
