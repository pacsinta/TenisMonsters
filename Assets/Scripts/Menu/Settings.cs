using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown refreshRateDropdown;
    public Slider volumeSlider;
    public GameObject audioObject;

    void Start()
    {
        windowModeDropdown.value = PlayerPrefs.GetInt("windowmode", 0);
        windowModeDropdown.onValueChanged.AddListener(SetWindowMode);

        var resolutions = Screen.resolutions.Select(res => res.width + "x" + res.height).ToList();
        resolutions.Reverse();
        resolutionDropdown.AddOptions(resolutions);
        resolutionDropdown.value = (Screen.resolutions.Length - 1) 
                                        - Screen.resolutions.ToList()
                                            .FindIndex(res => res.width == Screen.width && res.height == Screen.height);

        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        refreshRateDropdown.onValueChanged.AddListener(SetWindowRefreshRate);
        refreshRateDropdown.value = PlayerPrefs.GetInt("refreshrate", 3);

        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1);
    }

    void SetWindowMode(int mode)
    {
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        PlayerPrefs.SetInt("windowmode", mode);
    }

    void SetVolume(float volume)
    {
        audioObject.GetComponent<Audio>().SetVolume(volume);
    }

    void SetResolution(int resolution)
    {
        var selectedResoltuion = Screen.resolutions[Screen.resolutions.Length - resolution - 1];
        Screen.SetResolution(selectedResoltuion.width, selectedResoltuion.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("resolution", resolution);
    }

    void SetWindowRefreshRate(int rate)
    {
        switch (rate)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Application.targetFrameRate = 120;
                break;
            case 3:
                Application.targetFrameRate = -1;
                break;
        }
        PlayerPrefs.SetInt("refreshrate", rate);
    }

    public void RestoreSettings()
    {
        int defaultResolution = Screen.resolutions.Length - 2 >= 0 ? Screen.resolutions.Length - 2 : 0; // The default resolution is the second biggest resolution
        var resolution = PlayerPrefs.GetInt("resolution", defaultResolution);
        SetResolution(resolution);

        var windowMode = PlayerPrefs.GetInt("windowmode", 0);
        SetWindowMode(windowMode);

        var refreshRate = PlayerPrefs.GetInt("refreshrate", 3);
        SetWindowRefreshRate(refreshRate);
    }
}
