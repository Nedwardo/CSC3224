using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private Transform stairsUpStart;
    [SerializeField] private Transform stairsDownStart;
    private float height;
    private float stairsUpX;
    private float stairsDownX;

    void Start(){
        height = -500;
        if (stairsUpStart != null){
            stairsUpX = stairsUpStart.position.y;
            height = stairsUpStart.position.y;
        }else{
            stairsUpX = -500;
        }
        if (stairsDownStart != null){
            stairsDownX = stairsDownStart.position.y;
            if (height > 0){
                height += stairsDownStart.position.y;
                height /= 2;
            }
            else{
                height = stairsDownStart.position.y;
            }
        }else{
            stairsDownX = -500;
        }
    }
    public void setValues(float height, float stairsUpX, float stairsDownX){
        this.height = height;
        this.stairsUpX = stairsDownX;
        this.stairsDownX = stairsDownX;
        stairsUpStart = null;
        stairsDownStart = null;
    }
    public Transform getStairsUpStart(){
        return stairsUpStart;
    }

    public Transform getStairsDownStart(){
        return stairsDownStart;
    }
    public float getHeight(){
        return height;
    }
    public float getStairsUpX(){
        return stairsUpX;
    }

    public float getStairsDownX(){
        return stairsDownX;
    }
}