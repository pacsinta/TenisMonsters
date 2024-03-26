using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public enum LoadingState
{
    NotLoaded,
    DataAvailable,
    Loaded,
    Error
}

public class ConnectionCoroutine<T>
{
    private UnityWebRequest www;
    private T result;
    public T Result
    {
        get
        {
            state = LoadingState.Loaded;
            return result;
        }
    }
    public LoadingState state = LoadingState.NotLoaded;
    public IEnumerator coroutine()
    {
        state = LoadingState.NotLoaded;
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("BackendError: " + www.error);
            state = LoadingState.Error;
        }
        else
        {
            var resultText = www.downloadHandler.text;
            try
            {
                result = JsonConvert.DeserializeObject<T>(resultText);
                if(result == null)
                {
                    Debug.LogError("Can't parse the result");
                }

                state = LoadingState.DataAvailable;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                state = LoadingState.Error;
            }
        }

    }

    public ConnectionCoroutine(UnityWebRequest www)
    {
        this.www = www;
    }
}