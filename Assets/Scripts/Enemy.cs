using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Health health;
    private Gun gun;

    // Update is called once per frame
    void Start(){
        gun = GetComponent<Gun>(); 
    }
    void Update()
    {
        // if(onScreen){
        //     Debug.Log("On screen with Boxes");
        // }
        // else{
        //     Debug.Log("Off screen with Boxes");
        // }
        if(Physics2D.Linecast(GetComponent<Transform>().position, player.position, groundLayer).collider == null){// Need to check top and bottom
            gun.fire((Vector2) player.position);
        }
              
    }
}
