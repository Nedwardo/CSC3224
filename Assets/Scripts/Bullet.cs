using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private LayerMask targetLayer;
    private LayerMask groundLayer;
    private LayerMask boundingBoxLayer;

    public void setValues(int damage, LayerMask targetLayer, LayerMask groundLayer, LayerMask boundingBoxLayer){
        this.damage = damage;
        this.targetLayer = targetLayer;
        this.groundLayer = groundLayer;
        this.boundingBoxLayer = boundingBoxLayer;

    }
    private void OnTriggerEnter2D (Collider2D other){
        // Debug.Log("Layer =" + other.gameObject.layer);
        // Debug.Log("Target Layer =" + (targetLayer.value));
        // Debug.Log("Ground Layer =" + groundLayer.value);
        // Debug.Log("13 ^ 2  = " + (1 << 13));
        int value = ((1 << other.gameObject.layer) & targetLayer.value);
        if (((1 << other.gameObject.layer) & targetLayer.value) > 0){; // wrong, need to make it work with doors too
            other.gameObject.GetComponent<Health>().takeDamage(damage);
            Destroy(gameObject);
        }
        else if(((1 << other.gameObject.layer) & groundLayer.value) > 0){
            Destroy(gameObject);
        }
        
    }

    private void OnTriggerExit2D (Collider2D other){
        if(((1 << other.gameObject.layer) & boundingBoxLayer.value) > 0){
            Destroy(gameObject);
        }
        
    }
}
