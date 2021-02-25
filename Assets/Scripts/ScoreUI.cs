using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    void Awake(){
        gameObject.GetComponent<Text>().text = "Score: " + Score.score;
    }
}
