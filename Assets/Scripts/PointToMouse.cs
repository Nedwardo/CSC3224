using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PointToMouse : MonoBehaviour
{
    void Update()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Quaternion lookRotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        transform.rotation = lookRotation;
    }
}
