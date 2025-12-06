using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionMode : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private Resolution[] resolutions;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        SetResolution();
    }

    private void SetResolution()
    {
        resolutions = Screen.resolutions;

        dropdown.ClearOptions();

        List<string> options = new();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution resolution = resolutions[i], currentResolution = Screen.currentResolution;
            string option = $"{resolution.width} x {resolution.height} @ {(int) resolution.refreshRateRatio.value}Hz";
            options.Add(option);

            if (resolution.width == currentResolution.width && resolution.height == currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        dropdown.AddOptions(options);
        dropdown.value = PlayerPrefs.GetInt("Resolution", currentResolutionIndex);
        dropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
        Resolution resolution = resolutions[PlayerPrefs.GetInt("Resolution", resolutionIndex)];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Application.targetFrameRate = (int) resolution.refreshRateRatio.value;

        Debug.Log(resolution.ToString());
    }

    
}
