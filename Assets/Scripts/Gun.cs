using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int damage;
    public float bulletSpeed;
    public float durationToFireS;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask boundingBoxLayer;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private AudioSource shotSound;
    private float timeAtLastFire;
    private float currentTime;

    void Awake(){
        timeAtLastFire = -durationToFireS;
    }
    public void fire(Vector2 target){
        currentTime = Time.time; 
        if(currentTime - timeAtLastFire > durationToFireS){
            Vector2 direction = (Vector2) Vector3.Normalize(target-(Vector2) GetComponent<Transform>().position);
            Quaternion lookRotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

            GameObject bulletInstance = Instantiate(bulletPrefab, GetComponent<Transform>().position, lookRotation) as GameObject;
            bulletInstance.GetComponent<Bullet>().setValues(damage, targetLayer, groundLayer, boundingBoxLayer);
            bulletInstance.GetComponent<Rigidbody2D>().velocity = bulletSpeed * (Vector2) Vector3.Normalize(target-(Vector2) GetComponent<Transform>().position);  
            timeAtLastFire = currentTime;
            if (shotSound != null)
                shotSound.Play();
        }
    }

    public void setGroundLayer(LayerMask groundLayer){
        this.groundLayer = groundLayer;
    }
}

