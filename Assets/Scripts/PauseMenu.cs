using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;

    public void Pause(){
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        GameIsPaused = true;
    }
    public void Resume(){
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        GameIsPaused = false;
    }
    public void Quit(){
        SceneManager.LoadScene("mainMenu");
    }
}
