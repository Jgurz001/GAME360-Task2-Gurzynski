using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    
    private Vector3 moveDirection;
    
    [SerializeField] private Transform cameraTransform;

    //For rotating the character while moving, 720 is degrees per second
    public float rotationSpeed = 1440f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackRange = 1.5f;

    [SerializeField] private GameOver gameOver;

    //Create empty child object
    [SerializeField] Transform attackPoint;

    [SerializeField] LayerMask enemyLayers;


    [SerializeField] float health, maxHealth = 3f;

    [SerializeField] FloatingHealthBar healthBar;


    [Header("Components")]
    private Rigidbody rb;
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

        health = maxHealth;
       
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        healthBar.UpdateHealthBar(health, maxHealth);


        // Get reference to the Animator components
        //This was all learned from Faktory Studios on YouTube
        animator = GetComponent<Animator>();
        

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
        // Get input from keyboard, maybe switch to mouse later on? 
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxisRaw("Vertical");     // W/S or Up/Down arrows

        if (cameraTransform == null) 
        {
            return;
        
        
        }


        //Get the camera's forward and right directions
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        //Remove vertical tilt this way the player stays of the ground
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Create movement relative to the camera
        moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        bool isMoving = moveDirection.sqrMagnitude > 0.01f;


        // Play or stop the running animatiopn
        animator.SetBool("isRunning", isMoving);

        if (isMoving) 
        {

            // Find the rotation that faces the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            //Gradually rotate the player
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
          
        }

        // Keep the Rigidbody's current vertical speed for jump
        Vector3 velocity = rb.linearVelocity;
        //Apply camera-relative horizontal movement
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, velocity.y, moveDirection.z * moveSpeed);


    }

    void HandleJumping()
    {
        // Check for jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            //
            isGrounded = false;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            Debug.Log("Player jumped!");
        }
    }

    // Called when this collider/rigidbody has begun touching another
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player landed on ground");
        }
    }

    // Called when this collider/rigidbody has stopped touching another
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Player left ground");
        }
    }

    void Attack()
    {

        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) {
            
            return;
        }
        
        Debug.Log("Attack pressed");

        if (attackPoint == null) {
            Debug.Log("Attack Point not assigned");
            return;
        }

        // This will find enemy colliders inside of the attack sphere
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers, QueryTriggerInteraction.Collide);
        foreach (Collider enemyCollider in hitEnemies) {


            Enemy enemyHealth = enemyCollider.GetComponent<Enemy>();
            if (enemyHealth != null) {

                enemyHealth.TakeDamage(attackDamage);
            }

        }


    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakeDamage(float damageAmount) {

        health -= damageAmount;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0) 
        {
            Die();        
        }

    }

    private void Die()
    {
        if (gameOver != null)
        {
            gameOver.postGameOver();
        }
        
    }





}