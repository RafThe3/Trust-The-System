using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Canvas pauseMenu;

    public bool AutoLockCursor { get; set; } = true;

    private void Start()
    {
        pauseMenu.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EnablePauseMenu();
        }

        if (AutoLockCursor)
        {
            Cursor.lockState = pauseMenu.enabled ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void EnablePauseMenu()
    {
        pauseMenu.enabled = !pauseMenu.enabled;
        Time.timeScale = Time.timeScale == 1 ? 0 : 1;
        Cursor.lockState = Time.timeScale == 1 ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void Resume()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
