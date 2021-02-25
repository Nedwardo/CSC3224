using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
    public void PlayGame (){
        SceneManager.LoadScene("game");
    }

    public void BackToMainMenu(){
        SceneManager.LoadScene("mainMenu");
    }
}
