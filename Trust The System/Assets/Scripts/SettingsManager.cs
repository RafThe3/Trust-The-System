using UnityEngine;

public class SettingsManager : UnityEngine.MonoBehaviour
{
    public static SettingsManager instance;

    private void Awake()
    {
        if (instance)
        {
            Debug.LogError("More than one instance of SettingsManager found!");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
