using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    [SerializeField] private Text healthUI;
    public bool godMode;

    public override void Awake(){
        currentHealth = maxHealth;
        healthDisplayUpdate();
    }
    public override void takeDamage(int damage){
        if (!godMode){
            currentHealth -= damage;
            if (currentHealth > 0){
                healthDisplayUpdate();
            }
            else{
                 SceneManager.LoadScene("gameOver");
            }
        }
    }
    public override void restoreHealth(int restoredHealth){
        currentHealth += restoredHealth;
        if (currentHealth > maxHealth){
            currentHealth = maxHealth;
        }
        healthDisplayUpdate();
    }
    public override void restoreHealthToFull(){
        currentHealth = maxHealth;
        healthDisplayUpdate();
    }

    public override void healthDisplayUpdate(){
        healthUI.text = "Health = " + currentHealth;
    }
}