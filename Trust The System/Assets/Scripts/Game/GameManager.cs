using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : UnityEngine.MonoBehaviour
{
    public static GameManager instance;

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


    [SerializeField] private bool fixLockCursor = false;

    private void Start()
    {
        Time.timeScale = 1;

        Cursor.lockState = fixLockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void LoadAScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadAScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void EnableCanvas(Canvas canvas)
    {
        canvas.enabled = true;
    }

    public void DisableCanvas(Canvas canvas)
    {
        canvas.enabled = false;
    }

    public void PlaySFX(AudioClip audioClip)
    {
        Camera.main.GetComponent<AudioSource>().PlayOneShot(audioClip);
    }

    public void ChangeBrightness(Slider brightness)
    {
        Screen.brightness = brightness.value;
    }

    public void NewGame()
    {
        SaveSystem.Delete();
        LoadNextScene();
    }

    public void LoadGame()
    {
        PlayerData playerData = SaveSystem.Load();

        LoadAScene(playerData.sceneIndex);
    }

    /*
    public void SavePlayerProgress()
    {
        GameObject.FindWithTag("Player").GetComponent<SaveSystem>().Save();
    }

    public void LoadPlayerProgress()
    {
        GameObject.FindWithTag("Player").GetComponent<SaveSystem>().Load();
    }

    public void DeletePlayerProgress()
    {
        GameObject.FindWithTag("Player").GetComponent<SaveSystem>().Delete();
    }
    */
}
