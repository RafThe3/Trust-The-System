using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : UnityEngine.MonoBehaviour
{
    public bool fullscreen = true;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        toggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;
    }

    private void Update()
    {
        Debug.Log($"Is fullscreen: {fullscreen}");
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
    }
}
