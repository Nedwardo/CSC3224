using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verticalPlatform : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform enemyHolder;
    [SerializeField] private float waitTimeInS;
    [SerializeField] private LayerMask activePlatformLayer;
    [SerializeField] private LayerMask inactivePlatformLayer;

    private int activePlatformLayerNumber;
    private int inactivePlatformLayerNumber;
    private float waitedTime;
    void Start(){
        activePlatformLayerNumber = (int) Mathf.Log((float) activePlatformLayer, (float) 2);
        inactivePlatformLayerNumber = (int) Mathf.Log((float) inactivePlatformLayer, (float) 2);
        gameObject.layer = activePlatformLayerNumber;
    }

    void Update(){
        if (Input.GetAxisRaw("Vertical") < 0){
            if (waitTimeInS <= waitedTime){
                if (gameObject.layer == activePlatformLayerNumber){
                    gameObject.layer = inactivePlatformLayerNumber;
                }
            }
            else if (gameObject.layer == activePlatformLayerNumber){
                waitedTime += Time.deltaTime;
            }
        }
        else{
            gameObject.layer = activePlatformLayerNumber;
            waitedTime = 0.0f;
        }
    } 
    
    void OnCollisionExit2D(Collision2D other){
        if ((other.gameObject.layer & playerLayer.value) > 0){
            for (int i = 0; i < enemyHolder.childCount; i++){
                enemyHolder.GetChild(i).GetComponent<Enemy>().updatePathing();
            }
        }
    }
}
