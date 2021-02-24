using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public virtual void Start(){
        currentHealth = maxHealth;
    }
    public abstract void takeDamage(int damage);
    public abstract void restoreHealth(int health);
    public abstract void restoreHealthToFull();
    public abstract void healthDisplayUpdate();
}