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
            if (Time.timeScale != 0)
            {
                EnablePauseMenu();
            }
            else if (Time.timeScale == 0 && pauseMenu.enabled)
            {
                Resume();
            }
        }

        if (AutoLockCursor)
        {
            Cursor.lockState = Time.timeScale == 0 ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    public void EnablePauseMenu()
    {
        pauseMenu.enabled = true;
        Time.timeScale = 0;
        //Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        pauseMenu.enabled = false;
        Time.timeScale = 1;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
