using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float runSpeed = 40f;
    public float jumpForce = 400f;
    public float movementSmoothing = .05f;
    public bool airControl = true;
    float horizontalMove = 0f;
    [SerializeField] private LayerMask groundLayer;                          // A mask determining what is ground to the character
    public Rigidbody2D playerBody;
    public BoxCollider2D playerCollider;
    public float distanceToGround = 0.1f;
    public float tolaranceOnGroundEdge = 0.05f;
    private bool grounded;            // Whether or not the player is grounded.
    bool jump = false;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 playerVelocity = Vector3.zero;
    private Gun gun;
    private PlayerHealth health;
    private Camera mainCamera;
    private bool cheats;

    void Start(){
        cheats = false;
        gun = GetComponent<Gun>();
        health = GetComponent<PlayerHealth>();
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
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
            gameObject.GetComponent<Score>().scoringOff();
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
        bool wasGrounded = grounded;
        grounded = false;
        Collider2D colliderDebug = Physics2D.BoxCast(playerCollider.bounds.center,playerCollider.bounds.size - new Vector3(tolaranceOnGroundEdge,0,0), 0f,Vector2.down, distanceToGround, groundLayer).collider;
        // Color rayColor;
        // if (colliderDebug != null){
        //     rayColor = Color.green;
        // }
        // else{
        //     rayColor = Color.red;
        // }
        // Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + distanceToGround), rayColor);
        // Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + distanceToGround), rayColor);
        // Debug.DrawRay(playerCollider.bounds.center - new Vector3(0, playerCollider.bounds.extents.y), Vector2.right * (playerCollider.bounds.extents.x), rayColor);

        if (colliderDebug != null)
        {
            grounded = true;
        }
        move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }

        public void move(float move, bool jump)
    {

        //only control the player if grounded or airControl is turned on
        if (grounded || airControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, playerBody.velocity.y);
            // And then smoothing it out and applying it to the character
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
