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
    [SerializeField] private GameObject bulletPrefab;
    private float timeAtLastFire;
    private float currentTime;

    public void setValues(int damage,float durationToFireS, LayerMask targetLayer, LayerMask groundLayer, GameObject bulletPrefab, float bulletSpeed){
        this.damage = damage;
        this.durationToFireS = durationToFireS;
        this.targetLayer = targetLayer;
        this.groundLayer = groundLayer;
        this.bulletPrefab = bulletPrefab;
        this.bulletSpeed = bulletSpeed;
    }
    public void fire(Vector2 target){
        currentTime = Time.time; 
        if(currentTime - timeAtLastFire > durationToFireS){
            GameObject bulletInstance = Instantiate(bulletPrefab, GetComponent<Transform>().position, Quaternion.identity) as GameObject;
            bulletInstance.GetComponent<Bullet>().setValues(damage, targetLayer, groundLayer);
            bulletInstance.GetComponent<Rigidbody2D>().velocity = bulletSpeed * (Vector2) Vector3.Normalize(target-(Vector2) GetComponent<Transform>().position);  
            timeAtLastFire = currentTime;
        }
    }
}

