﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject scorePrefab;
    public void PlayGame (){
        SceneManager.LoadScene("game");
    }

    public void Leaderboard(){
        
    }
}
