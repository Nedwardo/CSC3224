using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int damageOnCollision;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private EnemyHealth health;
    [SerializeField] private GameObject dropOnDeathPrefab;
    [SerializeField] private int floorCount;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float movementSmoothing;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private float SlopeCheckDistance;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

    
    private Transform playerTransform; 
    private BoxCollider2D selfCollider;
    private bool facingRight = true;
    private bool onSlope = false;
    private float slopeSideAngle;
    private float slopeDownAngleOld;
    private Vector2 slopeNormalPerpendicular;
    private Vector3 bodyVelocity = Vector3.zero;
    private Rigidbody2D body;
    private Gun gun;
    public Floor[] floors;
    private PathingNode playerPathingNode;

    void Awake(){
        GameObject player = GameObject.Find("Player");
        playerTransform = player.GetComponent<Transform>();
        health.setDrop(dropOnDeathPrefab);
        gun = GetComponent<Gun>(); 
        body = GetComponent<Rigidbody2D>();
        selfCollider = GetComponent<BoxCollider2D>();
    }

    void floors_init(){
        floors = new Floor[floorCount];
        for (int i = 0; i < floorCount; i++){
            floors[i] = GameObject.Find("Floor " + i).GetComponent<Floor>();
        }
    }
    void FixedUpdate()
    {
        bool clear = (Physics2D.Linecast(transform.position, playerTransform.position, groundLayer).collider == null) &&
        (Physics2D.Linecast(new Vector3(transform.position.x - selfCollider.size.x*50, transform.position.y - selfCollider.size.y*50, 0.0f), playerTransform.position, groundLayer).collider == null) &&
        (Physics2D.Linecast(new Vector3(transform.position.x - selfCollider.size.x*50, transform.position.y + selfCollider.size.y*50, 0.0f), playerTransform.position, groundLayer).collider == null) &&
        (Physics2D.Linecast(new Vector3(transform.position.x - selfCollider.size.x*50, transform.position.y + selfCollider.size.y*50, 0.0f), playerTransform.position, groundLayer).collider == null) &&
        (Physics2D.Linecast(new Vector3(transform.position.x - selfCollider.size.x*50, transform.position.y + selfCollider.size.y*50, 0.0f), playerTransform.position, groundLayer).collider == null);
        Color color = Color.red;
        if (clear){
            color = Color.green;
        }
        Debug.DrawLine(new Vector3(transform.position.x - selfCollider.size.x*50, transform.position.y - selfCollider.size.y*50, 0.0f) ,playerTransform.position, color);
        Debug.DrawLine(new Vector3(transform.position.x - selfCollider.size.x*50, transform.position.y + selfCollider.size.y*50, 0.0f) ,playerTransform.position, color);
        Debug.DrawLine(new Vector3(transform.position.x + selfCollider.size.x*50, transform.position.y - selfCollider.size.y*50, 0.0f) ,playerTransform.position, color);
        Debug.DrawLine(new Vector3(transform.position.x + selfCollider.size.x*50, transform.position.y + selfCollider.size.y*50, 0.0f) ,playerTransform.position, color);
        if(clear){// Need to check top and bottom
            body.sharedMaterial = fullFriction;
            gun.fire((Vector2) playerTransform.position);
        }
        else{
            if(playerPathingNode == null){
                updatePathing();
            }
            body.sharedMaterial = noFriction;
            SlopeCheck();
            if (playerPathingNode.pastDest((Vector2) gameObject.transform.position) && !playerPathingNode.isFinal()){
                playerPathingNode = playerPathingNode.getNextNode();
            }
            moveTowardsPlayer(playerPathingNode.getDirection());        
        }
              
    }

    private void moveTowardsPlayer(int moveDirection)
    {
        body.sharedMaterial = noFriction;
        Vector3 targetVelocity;
        if (!onSlope){
            targetVelocity = new Vector2(moveDirection * moveSpeed * 10f, body.velocity.y);
        }
        else{
            targetVelocity = new Vector2(moveDirection * moveSpeed * 10f, -moveDirection * moveSpeed * 10f * slopeNormalPerpendicular.y);
        }
        body.velocity = Vector3.SmoothDamp(body.velocity, targetVelocity, ref bodyVelocity, movementSmoothing);

        if (moveDirection > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveDirection < 0 && facingRight)
        {
            Flip();
        }

    }        
    private void Flip(){
        facingRight = !facingRight;
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    public void updatePathing(){
        if(floors[0] == null){
            floors_init();
        }
        playerPathingNode = PathingNode.generatePathTo(playerTransform, gameObject.transform, floors);
    }

    void OnCollisionEnter2D (Collision2D other){
        if (other.gameObject.CompareTag("Player")){
            other.gameObject.GetComponent<Health>().takeDamage(damageOnCollision);
        }
    }
    private void SlopeCheck(){
        Vector2 checkPostition = (Vector2) transform.position- new Vector2(0.0f, gameObject.GetComponent<BoxCollider2D>().size.y*transform.localScale.y*0.5f);
        SlopeCheckHorizontal(checkPostition);
        SlopeCheckVertical(checkPostition);
    }

    private void SlopeCheckHorizontal(Vector2 checkPostition){
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPostition, transform.right, SlopeCheckDistance, groundLayer);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPostition, -transform.right, SlopeCheckDistance, groundLayer);
        
        if(slopeHitFront){
            onSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack){
            onSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else{
            onSlope = false;
            slopeSideAngle = 0.0f;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPostition){
        RaycastHit2D hit = Physics2D.Raycast(checkPostition, Vector2.down, SlopeCheckDistance, groundLayer);
        float slopeDownAngle = 90;
        if(hit){
            slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            float slopeDownAngel = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeDownAngel != slopeDownAngleOld){
                onSlope = true;
            }
            slopeDownAngleOld = slopeDownAngel;
        }
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            onSlope = false;
        }
    }
}