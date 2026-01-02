using UnityEngine;
using UnityEngine.UI;

public class Vsync : MonoBehaviour
{
    public bool enableVsync = true;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        enableVsync = PlayerPrefs.GetInt("Vsync") == 1;
        toggle.isOn = enableVsync;
    }

    private void Update()
    {
        QualitySettings.vSyncCount = enableVsync ? 1 : 0;
        Debug.Log($"Is vsync on: {QualitySettings.vSyncCount}");
    }

    public void EnableVsync(bool enableVsync)
    {
        this.enableVsync = enableVsync;
        PlayerPrefs.SetInt("Vsync", enableVsync ? 1 : 0);
    }
}
