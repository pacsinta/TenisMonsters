using System.Collections.Generic;
using UnityEngine;
using static SceneLoader;

public class Audio : MonoBehaviour
{
    public AudioSource idleSource;
    public AudioSource gameSource;
    public AudioSource winSource;
    public AudioSource loseSource;

    private static Audio instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private float volume;
    private void Start()
    {
        volume = PlayerPrefs.GetFloat("volume", 1);
        SetVolume(volume, false);
    }
    public void SetVolume(float volume, bool save = true)
    {
        idleSource.volume = volume;
        gameSource.volume = volume;
        winSource.volume = volume;
        loseSource.volume = volume;

        if(save)
            PlayerPrefs.SetFloat("volume", volume);
    }

    private void Update()
    {
        if (winSource.isPlaying || loseSource.isPlaying)
        {
            idleSource.Stop();
            gameSource.Stop();
        }
        // check if currentScene is 1
        else if (GetCurrentScene() == Scene.GameScene)
        {
            idleSource.Stop();
            if(!gameSource.isPlaying) gameSource.Play();
        }
        else
        {
            gameSource.Stop();
            if(!idleSource.isPlaying) idleSource.Play();
        }
    }

    public void PlayEndingSong(bool winner)
    {
        if (winner)
        {
            winSource.Play();
        }
        else
        {
            loseSource.Play();
        }
    }
}
