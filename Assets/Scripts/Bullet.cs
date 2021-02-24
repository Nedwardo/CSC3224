using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private LayerMask targetLayer;
    private LayerMask groundLayer;

    public void setValues(int damage, LayerMask targetLayer, LayerMask groundLayer){
        this.damage = damage;
        this.targetLayer = targetLayer;
        this.groundLayer = groundLayer;

    }
    private void OnTriggerEnter2D (Collider2D other){
        // Debug.Log("Layer =" + other.gameObject.layer);
        // Debug.Log("Target Layer =" + (targetLayer.value));
        // Debug.Log("Ground Layer =" + groundLayer.value);
        // Debug.Log("13 ^ 2  = " + (1 << 13));
        if (1 << other.gameObject.layer == targetLayer.value){;
            other.gameObject.GetComponent<Health>().takeDamage(damage);
            Destroy(gameObject);
        }
        else if(1 << other.gameObject.layer == groundLayer.value){
            Destroy(gameObject);
        }
        
    }
}
