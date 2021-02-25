using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] private int damageOnCollision;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private EnemyHealth health;

    [SerializeField] private GameObject dropOnDeathPrefab;
    private Gun gun;
    void Awake(){
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        health.setDrop(dropOnDeathPrefab);
        gun = GetComponent<Gun>(); 
        gun.setGroundLayer(groundLayer);
    }
    void Update()
    {
        // if(onScreen){
        //     Debug.Log("On screen with Boxes");
        // }
        // else{
        //     Debug.Log("Off screen with Boxes");
        // }
        if(Physics2D.Linecast(GetComponent<Transform>().position, playerTransform.position, groundLayer).collider == null){// Need to check top and bottom
            gun.fire((Vector2) playerTransform.position);
        }
              
    }

    void OnCollisionEnter2D (Collision2D other){
        if (other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<Health>().takeDamage(damageOnCollision);
        }
    }
}
