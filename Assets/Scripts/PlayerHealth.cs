using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : Health
{
    [SerializeField] private Text healthUI;
    public bool godMode;

    public override void Start(){
        currentHealth = maxHealth;
        healthDisplayUpdate();
    }
    public override void takeDamage(int damage){
        if (!godMode){
            currentHealth -= damage;
            healthDisplayUpdate();
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