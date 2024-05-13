using Assets.Scripts.Networking;
using System;
using System.Collections.Generic;
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
    public TMP_InputField changePasswordInput;
    public Button changePasswordButton;

    public TMP_InputField playerName;
    public TextMeshProUGUI errorText;
    public TMP_InputField originalPasswordInput;

    public Button resetGameButton;

    private ConnectionCoroutine<object> changePasswordCoroutine;
    void Start()
    {
        windowModeDropdown.value = PlayerPrefs.GetInt("windowmode", 0);
        windowModeDropdown.onValueChanged.AddListener(SetWindowMode);

        InitResolutions();
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution", GetDefaultResolution());

        refreshRateDropdown.onValueChanged.AddListener(SetWindowRefreshRate);
        refreshRateDropdown.value = PlayerPrefs.GetInt("refreshrate", 3);

        volumeSlider.onValueChanged.AddListener(SetVolume);
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1);

        changePasswordButton.onClick.AddListener(ChangePassword);
        resetGameButton.onClick.AddListener(ResetGame);
    }

    List<string> resolutionOptions = new();
    void InitResolutions()
    {
        var resolutions = Screen.resolutions.Select(res => res.width + "x" + res.height).ToHashSet();
        resolutionOptions = resolutions.ToList();
        resolutionOptions.Reverse();
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
        var options = resolutionDropdown.options;
        var selectedResoltuion = options[resolution].text.Split('x');

        Screen.SetResolution(Convert.ToInt32(selectedResoltuion[0]), Convert.ToInt32(selectedResoltuion[1]), Screen.fullScreenMode);
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

    int GetDefaultResolution()
    {
        return resolutionOptions.Count > 1 ? 1 : 0;
    }
    public void RestoreSettings()
    {
        InitResolutions();

        int defaultResolution = GetDefaultResolution();
        var resolution = PlayerPrefs.GetInt("resolution", defaultResolution);
        
        SetResolution(resolution);
        resolutionDropdown.value = resolution;

        var windowMode = PlayerPrefs.GetInt("windowmode", 0);
        SetWindowMode(windowMode);
        windowModeDropdown.value = windowMode;

        var refreshRate = PlayerPrefs.GetInt("refreshrate", 3);
        SetWindowRefreshRate(refreshRate);
        refreshRateDropdown.value = refreshRate;
    }

    void ChangePassword()
    {
        if (string.IsNullOrEmpty(changePasswordInput.text))
        {
            errorText.text = "The new password can't be empty!";
            return;
        }
        if (!SecureStore.SecureCheck(playerName.text, originalPasswordInput.text))
        {
            errorText.text = "Wrong password!";
            return;
        }
        errorText.text = "";

        if (changePasswordCoroutine?.Coroutine() != null) StopCoroutine(changePasswordCoroutine.Coroutine());
        changePasswordCoroutine = DatabaseHandler.ChangePassword(
            playerName.text, 
            SecureStore.GetHashedPassword(playerName.text), 
            SecureStore.CreateHashWithConstSalt(changePasswordInput.text));
        StartCoroutine(changePasswordCoroutine.Coroutine());
    }

    void CheckUpdatedPassword()
    {
        if (changePasswordCoroutine == null) return;
        switch(changePasswordCoroutine.state)
        {
            case ELoadingState.Error:
                errorText.text = "Error changing password!";
                _ = changePasswordCoroutine.Result;
                break;
            case ELoadingState.DataAvailable:
                _ = changePasswordCoroutine.Result;
                errorText.text = "Password changed!";
                SecureStore.SavePassword(playerName.text, changePasswordInput.text);
                changePasswordInput.text = "";
                originalPasswordInput.text = "";
                break;
        }
    }

    private void Update()
    {
        CheckUpdatedPassword();
    }

    void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        RestoreSettings();
    }
}
