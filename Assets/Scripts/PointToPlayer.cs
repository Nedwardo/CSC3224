using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToPlayer : MonoBehaviour
{
    private Transform playerTransform;
    void Start(){
        GameObject player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
    }

    void Update()
    {
        Vector2 direction = transform.position - playerTransform.position;
        Quaternion lookRotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        transform.rotation = lookRotation;
    }
}
