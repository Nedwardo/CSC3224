using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float jumpForce = 400f;
    [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private bool airControl = true;
    [SerializeField] private LayerMask groundLayer;                          // A mask determining what is ground to the character
    [SerializeField] private LayerMask platformLayer;                          // A mask determining what is ground to the character
    [SerializeField] private float distanceToGround = 0.1f;
    [SerializeField] private float SlopeCheckDistance;
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D fullFriction;

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


    private Rigidbody2D playerBody;
    private BoxCollider2D playerCollider;
    private Gun gun;
    private PlayerHealth health;
    private Camera mainCamera;

    void Start(){
        gun = GetComponent<Gun>();
        health = GetComponent<PlayerHealth>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        playerBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            gun.fire(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
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

    private void FixedUpdate()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast((Vector2) playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, distanceToGround, groundLayer | platformLayer);
        // SHouldn't have to check if on slope, but going down slopes is being really buggy
        grounded = (raycastHit.collider != null) || onSlope;
        Color color = Color.red;
        if (grounded){
            color = Color.green;
        }
        Debug.DrawRay((Vector2) playerCollider.bounds.center + new Vector2(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + distanceToGround), color);
        Debug.DrawRay((Vector2) playerCollider.bounds.center - new Vector2(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + distanceToGround), color);
        Debug.DrawRay((Vector2) playerCollider.bounds.center - new Vector2(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + distanceToGround), Vector2.right * (playerCollider.bounds.extents.x*2), color);
        SlopeCheck();
        move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }

        public void move(float move, bool jump)
    {
         playerBody.sharedMaterial = noFriction;

        //only control the player if grounded or airControl is turned on
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
        //Broken for both player and enemy
        Vector2 checkPostition = (Vector2) transform.position- new Vector2(0.0f,  playerCollider.size.y*transform.localScale.y*0.5f);
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
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
