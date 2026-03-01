using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunManager : UnityEngine.MonoBehaviour
{
    #region Singleton

    public static GunManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.Log($"More than one instance of {this} found in {name}");
        }
        Instance = this;
    }

    #endregion

    public TextMeshProUGUI ammoText;
    public Slider reloadBar;
    public GameObject tracerPrefab;
    public float tracerSpeed = 500f;
    public float tracerLifetime = 1f;

    private Image[] reloadBarImages;

    private void Start()
    {
        reloadBarImages = reloadBar.GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        DontDestroyOnLoad(gameObject);

        bool isGamePaused = Time.timeScale == 0;
        ammoText.enabled = !isGamePaused;
        ToggleSliderImages(!isGamePaused);
    }

    // Toggles the slider entirely though its image components
    private void ToggleSliderImages(bool toggle)
    {
        foreach (Image image in reloadBarImages)
        {
            image.enabled = toggle;
        }
    }
}
