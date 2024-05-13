using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public enum ELoadingState
    {
        NotLoaded,
        DataAvailable,
        Loaded,
        Error
    }

    public enum ERequestType
    {
        GET,
        POST
    }

    public class ConnectionCoroutine<T>
    {
        private readonly UnityWebRequest www;
        private readonly bool parseResponse = true;
        private T result;
        public T Result
        {
            get
            {
                state = ELoadingState.Loaded;
                return result;
            }
        }
        public ELoadingState state = ELoadingState.NotLoaded;
        public IEnumerator Coroutine()
        {
            state = ELoadingState.NotLoaded;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("BackendError: " + www.error);
                state = ELoadingState.Error;
            }
            else
            {
                var resultText = www.downloadHandler.text;
                try
                {
                    if (parseResponse)
                    {
                        result = JsonConvert.DeserializeObject<T>(resultText);
                        if (result == null)
                        {
                            Debug.LogError("Can't parse the result");
                        }
                    }

                    state = ELoadingState.DataAvailable;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    state = ELoadingState.Error;
                }
            }
        }

        public ConnectionCoroutine(UnityWebRequest www, bool parseResponse = true)
        {
            this.www = www;
            this.parseResponse = parseResponse;
        }
        public ConnectionCoroutine(string fullUrl, ERequestType type, string body = "", bool parseResponse = true)
        {
            string typeString = type == ERequestType.GET ? "GET" : "POST";
            var www = new UnityWebRequest(fullUrl, typeString);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "text/plain");
            
            this.www = www;
            this.parseResponse = parseResponse;
        }
    }
}