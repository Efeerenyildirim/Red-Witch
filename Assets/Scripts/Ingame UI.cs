using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class IngameUI : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    bool isPaused = false;
    [SerializeField] bool mainMenu = false;
    public int isPlayerClearedTutorial = 0;

    void Start()
    {
        isPlayerClearedTutorial = PlayerPrefs.GetInt("isPlayerClearedTutorial");
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)) 
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if(!mainMenu)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);

            if (isPaused)
                Time.timeScale = 0f;
            else
                Time.timeScale = 1f;

        }
    }

    public void restartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void returnMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void exitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
    public void resetSave()
    {

    }

    public void loadTutorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }
    public void startGame()
    {
        Time.timeScale = 1f;
        if(isPlayerClearedTutorial == 0)
        SceneManager.LoadScene("Tutorial");
        else
        SceneManager.LoadScene("Main Room");
    }
    public void ResetPlayerProgress()
    {
        Debug.Log("reset");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
