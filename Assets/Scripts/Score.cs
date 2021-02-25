using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{    
    private bool cheats;
    public static int score;
    void Awake(){
        score = 0;
        cheats = false;
    }
    public void addScore(int value){
        Debug.Log(cheats);
        if (!cheats){
            score += value;
            scoreDisplayUpdate();
        }
    }

    public void scoringOff(){
        cheats = true;
    }

    public int getScore(){
        return score;
    }

    public void scoreDisplayUpdate(){
        GameObject.Find("ScoreUI").GetComponent<Text>().text = "Score: " + score;
    }
}