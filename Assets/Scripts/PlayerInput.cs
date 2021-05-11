using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private bool airControl = true;
    [SerializeField] private LayerMask groundLayer;                          // A mask determining what is ground to the character
    [SerializeField] private float distanceToGround = 0.1f;
    [SerializeField] private float SlopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;
    [SerializeField] private GameObject statsTextObject;

    [SerializeField] private GameObject spriteRenderObject;
    [SerializeField] private Gun gun;
    [SerializeField] private float stepDuration;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private PauseMenu pauseMenu;
    private Vector3 playerVelocity = Vector3.zero;
    private float horizontalMove = 0f;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private bool grounded;            // Whether or not the player is grounded.
    private bool jump = false;
    private Vector2 slopeNormalPerpendicular;
    private float slopeDownAngleOld;
    private float slopeSideAngle;
    private bool onSlope = false;
    private bool cheats = false;
    private float timeToStep = 0f;


    private Rigidbody2D playerBody;
    private PolygonCollider2D playerCollider;
    private PlayerHealth health;
    private Camera mainCamera;

    void Start(){
        health = GetComponent<PlayerHealth>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        playerBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<PolygonCollider2D>();
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)){
            if (pauseMenu.GameIsPaused){
                pauseMenu.Resume();
            }
            else{
                pauseMenu.Pause();
            }
        }
        if (!pauseMenu.GameIsPaused){
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            if (Input.GetKeyDown("k"))
                statsTextObject.SetActive(!statsTextObject.activeSelf);

            if (Input.GetButtonDown("Jump"))
                jump = true;

            if (Input.GetMouseButtonDown(0))
                gun.fire(mainCamera.ScreenToWorldPoint(Input.mousePosition));

            if (Input.GetKeyDown("c")){
                Debug.Log("Cheats Active");
                cheats = true;
                GameObject.Find("Score").GetComponent<Score>().scoringOff();
            }
            if (Input.GetKeyDown("z") && cheats){
                Debug.Log("God mode toggled");
                health.godMode = !health.godMode;
            }
            if (Input.GetKeyDown("x") && cheats){
                Debug.Log("Infinite fire rate");
                gun.durationToFireS = 0.0f;
            }
            if (Input.GetKeyDown("\\") && cheats){
                Debug.Log("Double speed");
                runSpeed  *= 2;
            }
        }

    }
    private bool checkGrounded(){
        bool grounded = (Physics2D.BoxCast(
            new Vector2((playerCollider.bounds.min.x + playerCollider.bounds.max.x)/2.0f, playerCollider.bounds.min.y), 
            new Vector2(Math.Abs(playerCollider.bounds.min.x - playerCollider.bounds.max.x), distanceToGround), 
            0f,
            Vector2.down,
            Mathf.Infinity,
            groundLayer) 
            .collider != null);
        
        Debug.DrawRay(playerCollider.bounds.min, new Vector2 (0, -distanceToGround), grounded ? Color.green : Color.red);
        Debug.DrawRay(new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.min.y), new Vector3 (0, -distanceToGround),  grounded ? Color.green : Color.red);
        Debug.DrawRay(new Vector2((playerCollider.bounds.min.x + playerCollider.bounds.max.x)/2.0f, (playerCollider.bounds.min.y + playerCollider.bounds.max.y)/2.0f), new Vector3 (0, -distanceToGround),  grounded ? Color.green : Color.red);
        return grounded;
    }
    private void FixedUpdate()
    {
        SlopeCheck();
        move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }

    public void move(float move, bool jump)
    {
        
        playerBody.sharedMaterial = noFriction;

        //only control the player if grounded or airControl is turned on
        if (!grounded || move == 0 )
            timeToStep = 0;
        if (grounded || airControl)
        {
            Vector3 targetVelocity;
            if (!onSlope){ 
                targetVelocity = new Vector2(move * 10f, playerBody.velocity.y);
            }
            else{
                if (move == 0.0f){
                    playerBody.sharedMaterial = fullFriction;
                }
                targetVelocity = new Vector2(move * 10f, -move * 10f * slopeNormalPerpendicular.y);
            }
            playerBody.velocity = Vector3.SmoothDamp(playerBody.velocity, targetVelocity, ref playerVelocity, movementSmoothing);
            if (playerBody.velocity.sqrMagnitude > 0.1){
                timeToStep += Time.deltaTime;
                if (timeToStep > stepDuration){
                    timeToStep -= stepDuration;
                    footstep.Play();
                }
            }
            spriteRenderObject.transform.Rotate(new Vector3(0, 0, -playerBody.velocity.x*0.08f));
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (grounded && jump)
        {
            // Add a vertical force to the player.
            grounded = false;
            playerBody.AddForce(new Vector2(0f, jumpForce));
        }
    }


    private void SlopeCheck(){
        Vector2[] groundedCheckPoints = new Vector2[]{
            new Vector2(playerCollider.bounds.min.x, playerCollider.bounds.min.y + 0.1f),
            new Vector2((playerCollider.bounds.max.x + playerCollider.bounds.min.x)/2, playerCollider.bounds.min.y + 0.1f),
            new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.min.y + 0.1f)
        };
        bool[] slopeHits = new bool[3];
        for (int i = 0; i < 3; i ++){
            slopeHits[i] = Physics2D.Raycast(groundedCheckPoints[i], Vector2.down, SlopeCheckDistance, groundLayer).collider != null;
            }
        
        
        onSlope = false;
        grounded = false;
        if (slopeHits[0] || slopeHits[1] || slopeHits[2] ){

            grounded = true;
            if ((slopeHits[0] && slopeHits[1] && slopeHits[1] && slopeHits[2]) == false){
                onSlope = true;
            }

        }

        // RaycastHit2D slopeHitFront = Physics2D.Raycast(playerCollider.bounds.min, transform.right, SlopeCheckDistance, groundLayer);
        // RaycastHit2D slopeHitMid = Physics2D.Raycast(checkPostition, -transform.right, SlopeCheckDistance, groundLayer);
        // RaycastHit2D slopeHitBack = Physics2D.Raycast(playerCollider.bounds.min, -transform.right, SlopeCheckDistance, groundLayer);
        // //Broken for both player and enemy
        // Vector2 checkPostition = new Vector2((playerCollider.bounds.min.x + playerCollider.bounds.max.x)/2.0f, playerCollider.bounds.min.y);
        // Debug.DrawRay(checkPostition, 2*Vector2.down, Color.green);
        // SlopeCheckHorizontal(checkPostition);
        // SlopeCheckVertical(checkPostition);
    }

    private void SlopeCheckHorizontal(Vector2 checkPostition){
        Debug.DrawRay(checkPostition, SlopeCheckDistance*transform.right);
        Debug.DrawRay(checkPostition, -SlopeCheckDistance*transform.right);
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
        Debug.DrawRay(playerCollider.bounds.min, SlopeCheckDistance*Vector2.down);
        RaycastHit2D hit = Physics2D.Raycast(checkPostition, Vector2.down, SlopeCheckDistance, groundLayer);
        float slopeDownAngle = 90;
        if(hit){
            slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeDownAngle != slopeDownAngleOld){
                onSlope = true;
            }
            slopeDownAngleOld = slopeDownAngle;
        }
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            onSlope = false;
        }
    }

    private void Flip()
        {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        spriteRenderObject.GetComponent<SpriteRenderer>().flipX = !spriteRenderObject.GetComponent<SpriteRenderer>().flipX;
        transform.localScale = theScale;
    }
}
