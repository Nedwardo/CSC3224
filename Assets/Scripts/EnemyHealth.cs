using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHealth : Health
{
    [SerializeField] private GameObject healthBar;
    private float initialHealthBarScale;

    private GameObject dropPrefab;

    public override void Awake(){
        currentHealth = maxHealth;
        initialHealthBarScale = healthBar.transform.localScale.x;
        healthBar.GetComponent<SpriteRenderer>().enabled = false;
    }
    public override void takeDamage(int damage){
        currentHealth -= damage;
        healthDisplayUpdate();
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

    public void setDrop(GameObject dropPrefab){
        this.dropPrefab = dropPrefab;
    }
    public override void healthDisplayUpdate(){
        if (currentHealth != 0){
            healthBar.GetComponent<Renderer>().enabled = true;
            healthBar.transform.localScale = (Vector3) new Vector2(initialHealthBarScale * ((float) currentHealth) / maxHealth, healthBar.transform.localScale.y);
        }
        else{
            Instantiate(dropPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}