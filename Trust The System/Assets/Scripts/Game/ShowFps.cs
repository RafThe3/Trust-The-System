using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowFps : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [Tooltip("How often it counts the frames."), Min(0), SerializeField] private float pollingTime = 1;
    public bool showFps = true;

    private Toggle toggle;

    private float time;
    private int frameCount;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        showFps = PlayerPrefs.GetInt("Show FPS") == 1;
        toggle.isOn = showFps;
        fpsText.enabled = showFps;
    }

    private void Update()
    {
        fpsText.enabled = showFps;

        ShowFPS();
    }

    private void ShowFPS()
    {
        if (!showFps)
        {
            return;
        }

        time += Time.deltaTime;

        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fpsText.text = $"{frameRate} FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }

    public void ShowFPS(bool showFps)
    {
        this.showFps = showFps;
        PlayerPrefs.SetInt("Show FPS", showFps ? 1 : 0);
    }
}
