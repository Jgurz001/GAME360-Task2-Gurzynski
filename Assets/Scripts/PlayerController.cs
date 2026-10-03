using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    //For rotating the character while moving, 720 is degrees per second
    public float rotationSpeed = 1440f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 25;
    [SerializeField] private float attackRange = 1.5f;

    [SerializeField] private GameOver gameOver;

    //Create empty child object
    [SerializeField] Transform attackPoint;

    [SerializeField] LayerMask enemyLayers;
   // This allows time between jumps so player is not rapidly jumping
    [SerializeField] private float jumpRepeatTime = 1f;

    [SerializeField] float health, maxHealth = 3f;

    [SerializeField] FloatingHealthBar healthBar;



    [Header("Mouse Controls")]




    [Header("Components")]
    private Rigidbody rb;
    private bool isGrounded = false;
    //private Vector3 input;

    //Reference to the Animator Component
    private Animator animator;
    //private CharacterController controller;


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


        // Get reference to the Animator and CharacterController components
        //This was all learned from Faktory Studios on YouTube
        animator = GetComponent<Animator>();
        //controller = GetComponent<CharacterController>();
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

        // Create movement vector
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        // This will determine IF the player is moving, editing from previous if statement
        bool isMoving = movement != Vector3.zero;

        // Call on the animation
        animator.SetBool("isRunning", isMoving);

        //With the help of google, this allows the player to rotate
        if (isMoving) {


            Quaternion playerRotation = Quaternion.LookRotation(movement);

            //Quaternion is how Unity represents a 3d objects rotation
            transform.rotation = Quaternion.Slerp(transform.rotation,playerRotation,rotationSpeed = Time.fixedDeltaTime);
        }

        movement = movement.normalized * moveSpeed * Time.deltaTime;

        // Apply movement
        transform.Translate(movement, Space.World);


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

        //Notify the GameManager Singleton that an enemy died, will work on when we get to that point
        if (gameOver != null)
        {
            gameOver.postGameOver();
        }
        
    }


    // Try a switch case with GetAxisRaw: BUT FOCUS MORE ON THE OTHER CONCEPTS BEFORE WORKING ON THE ANIMATION

    /*void testMovement() {

        // Get input from keyboard, maybe switch to mouse later on? 
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

        // Create movement vector
        Vector3 movement = new Vector3(horizontal, 0f, vertical);
        // This will determine IF the player is moving, editing from previous if statement
        bool isMoving = movement != Vector3.zero;

        input = Vector3.zero;

        Keyboard kb = Keyboard.current;

        // move forward
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
        {

            input.y = +1;
            animator.SetBool("isRunning", isMoving);
        }


    }*/


}