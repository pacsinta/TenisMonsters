using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public List<AudioSource> idleSources;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private float volume;
    private void Start()
    {
        volume = PlayerPrefs.GetFloat("volume", 1);
        SetVolume(volume);
    }
    public void SetVolume(float volume)
    {
        foreach (var audioSource in idleSources)
        {
            audioSource.volume = volume;
        }
        PlayerPrefs.SetFloat("volume", volume);
    }
}
