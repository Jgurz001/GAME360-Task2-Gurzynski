using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    // player speed
    public float moveSpeed = 3f;
    // upward force for jump
    public float jumpForce = 7f;
    // Current camera relative direction
    private Vector3 moveDirection;
    // Camera used to calculate movement
    [SerializeField] private Transform cameraTransform;

    //For rotating the character while moving
    public float rotationSpeed = 1440f;

    [Header("Attack Settings")]
    // Damage that is dealt to an enemy
    [SerializeField] private int attackDamage = 25;
    // Range of the attack area
    [SerializeField] private float attackRange = 1.5f;
    //Controls the game UI
    [SerializeField] private GameUI gameUI;

    //Center of the attack area
    [SerializeField] Transform attackPoint;
    //Layers that the attack can hit
    [SerializeField] LayerMask enemyLayers;

    [Header("Health Settings")]
    // Current and max health
    [SerializeField] float health, maxHealth = 3f;
    // player's health bar
    [SerializeField] FloatingHealthBar healthBar;


    [Header("Components")]
    private Rigidbody rb;
    // Tracks for if the player can jump
    private bool isGrounded = false;

    //Reference to the Animator Component
    private Animator animator;

    // Called when script instance is being loaded
    void Awake()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();

        // Verify component was found
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on Player!");
        }
        else
        {
            Debug.Log("PlayerController Awake - Rigidbody initialized");
        }
    }

    // Called before the first frame update
    void Start()
    {
        // Set players health back to max
        health = maxHealth;
       
        // Find and update the health bar
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, maxHealth);


        // Get reference to the Animator components
        //This was all learned from Faktory Studios on YouTube
        animator = GetComponent<Animator>();
        
        // Find main camera if there was not one assigned
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        
        }

    }

    // Called once per frame
    void Update()
    {
        HandleMovement();
        HandleJumping();
        Attack();
    }



    void HandleMovement()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down arrows

        // IF the camera is not found
        if (cameraTransform == null) 
        {
            Debug.Log("Camera NOT found.");
            return;   
        }
        //Get the camera's forward and right directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        //Remove vertical tilt this way the player stays on the ground
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Keep both directions at equal length
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Create movement relative to the camera
        moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
        // Check if the player is moving
        bool isMoving = moveDirection.sqrMagnitude > 0.01f;


        // Update the running animatiopn
        animator.SetBool("isRunning", isMoving);

        if (isMoving) 
        {

            // Find the rotation that faces the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            //Gradually rotate the player towards the target
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
          
        }

        // Keep the Rigidbody's current vertical speed for jump
        Vector3 velocity = rb.linearVelocity;
        //Apply camera-relative horizontal movement
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, velocity.y, moveDirection.z * moveSpeed);


    }

    void HandleJumping()
    {
        // Check for jump input, only jump if player is on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // prevent another jump in the air
            isGrounded = false;
            // Apply instant upward force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //Play jump animation
            animator.SetTrigger("Jump");
            Debug.Log("Player jumped!");
        }
    }

    // Called when this collider/rigidbody has begun touching another
    void OnCollisionEnter(Collision collision)
    {
        // Allow jumping after touching the ground
        if (collision.gameObject.CompareTag("Ground"))
        {

            isGrounded = true;

            Debug.Log("Player landed on ground");
        }
    }

    // Called when this collider/rigidbody has stopped touching another
    void OnCollisionExit(Collision collision)
    {
        // Stop ground status after leaving
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Player left ground");
        }
    }

    // Method for attacking
    void Attack()
    {
         // Continue attacking after left click is pressed (Left it this way for spamming)
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) {
            
            return;
        }
        
       
        // stop if attack point is missing
        if (attackPoint == null) {
            // For debugging
            Debug.Log("Attack Point not assigned");
            return;
        }

        // This will find enemy colliders inside of the attack sphere
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers, QueryTriggerInteraction.Collide);
        // Check every single collider that was found
        foreach (Collider enemyCollider in hitEnemies) {

            // Call on the enemy script and get the Enemy health
            Enemy enemyHealth = enemyCollider.GetComponent<Enemy>();

            if (enemyHealth != null) {
                Debug.Log("Attack pressed");
                // Damage the enemy
                enemyHealth.TakeDamage(attackDamage);
            }

        }


    }
    // Shows the range of the player/enemy in the scene, good for adjusting
    private void OnDrawGizmosSelected()
    {
        // Stop if attack point is missing
        if (attackPoint == null) return;
        // Make the color of the sphere
        Gizmos.color = Color.red;
        //Draws the wire sphere around the character
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    //Method for player/enemy taking damage
    public void TakeDamage(float damageAmount) {

        // Take player damage
        health -= damageAmount;
        //Update the healthbar
        healthBar.UpdateHealthBar(health, maxHealth);
        //Kill player/ end game at zero
        if (health <= 0) 
        {
            Die();        
        }

    }

    private void Die()
    {
        // Display gameover screen
        if (gameUI != null)
        {
            //Call game over panel
            gameUI.postGameOver();
        }   
    }
}