using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int value;
    [SerializeField] private int durationInS;
    void Awake(){
        StartCoroutine(duration());
    }

    void OnCollisionEnter2D (Collision2D other){
        if (other.gameObject.CompareTag("Player")){
            GameObject.Find("Score").GetComponent<Score>().addScore(value);
            Destroy(gameObject);
        }
    }

    IEnumerator duration(){
        yield return new WaitForSeconds(durationInS);
        Destroy(gameObject);
    }
}