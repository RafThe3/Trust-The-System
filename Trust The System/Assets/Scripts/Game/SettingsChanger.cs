using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsChanger : UnityEngine.MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float defaultValue = 1f;
    [SerializeField] private bool addPercentSign = true;
    [SerializeField] private string settingType = string.Empty;
    [Min(0), Range(0, 3), SerializeField] private int moveDecimalPlaces = 1;

    private float decimalPlaces = 1;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(settingType, defaultValue);
        decimalPlaces = Mathf.Pow(10, moveDecimalPlaces);
        Debug.Log($"Value: {PlayerPrefs.GetFloat(settingType, defaultValue)}");
    }

    private void Update()
    {
        AdjustSettings();
    }

    private void AdjustSettings()
    {
        if (settingType == string.Empty)
        {
            throw new ArgumentException("Setting type requires a name.");
        }

        if (!slider.wholeNumbers)
        {
            slider.value = Mathf.Round(slider.value * decimalPlaces) / decimalPlaces;
        }

        PlayerPrefs.SetFloat(settingType, slider.value);
        string value = $"{PlayerPrefs.GetFloat(settingType) * decimalPlaces}";
        valueText.text = !addPercentSign ? value : $"{value}%";
    }

    public void AddSliderValue() => slider.value++;

    public void DecreaseSliderValue() => slider.value--;
}
