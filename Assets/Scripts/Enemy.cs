using UnityEngine;

// Ensure the enemy has a rigidbody
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    // Current and max health
    [SerializeField] float health, maxHealth = 3f;

    //Normal enemy movement speed
    public float moveSpeed = 2f;

    //How quickly the enemy turns toward the player
    public float rotateSpeed = 360f;

    [Header("Attack Settings")]
    // Damage dealt to the player
    [SerializeField] private int attackDamage = 10;
    
    //Enemy needs a cool down because spamming would cause issues
    [SerializeField] private float attackCooldown = 1f;
    // Time for when the enemy can attack again
    private float attackTime;
    // Enemy healthbar
    [SerializeField] FloatingHealthBar healthBar;

    [Header("AI")]

    //Enemy begins chasing when the player enters this range
    public float detectionRange = 5f;

    //Ref to the player's location
    private Transform player;

    //Reference to the enemy's 3D RigidBoy
    private Rigidbody rb;

    //Reference to the Animator Component
    private Animator animator;

    private void Awake()
    {
        //Get the Rgidbody attached to the enemy
        rb = GetComponent<Rigidbody>();

        // Get the animator on the enemy or its child
        animator = GetComponentInChildren<Animator>();

        if (animator == null) 
        {
            Debug.Log("Enemy could not find the Animator");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Begin with full health
        health = maxHealth;
        // Display the full health
        if (healthBar != null) {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
        //Find the gameobject tagged player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
 

        //Store the player's position if one was found
        if (playerObject != null)
        {
            player = playerObject.transform;

        }
        else 
        {
            Debug.LogError("Enemy could not find an object tagged player!");

        }
        
    }

    // FixedUpdate used for rigidbody physics
    void FixedUpdate()
    {
        ChasePlayer();
    }

    private void ChasePlayer() 
    {
        //Stop if player or rigidbody cannot be found
        if (player == null || rb == null) 
        {
            return;
        
        }

        // Start with the enemy's movement speed
        float currentSpeed = moveSpeed;

        //Calculate the 3D distance between the enemy and player
        float distance = Vector3.Distance(transform.position, player.position);

        //Preserve the enemy's current vertical velocity
        //this allows gravity to continue working
        float verticalVelocity = rb.linearVelocity.y;

        // The enemy begins each frame as not moving
        bool isMoving = false;
       

        if (distance <= detectionRange)
        {
            //Find the direction from the enemy to the player
            Vector3 direction = player.position - transform.position;

            //prevent the enemy from flying upward when the player jumps
            direction.y = 0f;

            //Continue only if there is a valid direction
            if (direction.sqrMagnitude > 0.001f)
            {
                // Convert the direction to length of one
                direction.Normalize();

                // Enemy is actively moving, assists with animator (Spent an hour realizing I was missing this)
                isMoving = true;

                // Move toward the player on the x and z axes
                rb.linearVelocity = new Vector3(direction.x * currentSpeed, verticalVelocity, direction.z * currentSpeed);

                // Determine the rotation needed to face the player
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                //Smoothly rotate toward the player
                Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);

                rb.MoveRotation(newRotation);
            }
        }
        else 
        {
            // Stop horizontal movement when outside detection range, but preserve gravity and vertical movement
            rb.linearVelocity = new Vector3(0f, verticalVelocity, 0f);
        }
        // Update the running animation
        if (animator != null)
        {
            animator.SetBool("isRunning", isMoving);
        }   
    }

    public void TakeDamage(float damageAmount)
    {
        // Removes health from the enemy
        health -= damageAmount;

        Debug.Log($"Enemy took {damageAmount} damage!!! Current health: {health}");
        // Update the health bar
        healthBar.UpdateHealthBar(health, maxHealth);
        //Destroy the enemy at zero health
        if (health <= 0)
        {
            Die();
        }        

    }

    void Attack(PlayerController playerHealth)
    {
        //Damage player
        playerHealth.TakeDamage(attackDamage);

        Debug.Log($"Enemy attacked player for  {attackDamage} damage!");

    }

    private void OnCollisionStay(Collision collision)
    {
        // Stop if the attack is cooling down
        if (Time.time < attackTime) 
        {
            return; 
        }

        //Search the collided object and its parents, essentially searching for the playercontroller
        PlayerController playerHealth = collision.gameObject.GetComponentInParent<PlayerController>();

        if (playerHealth != null) 
        {
            // Attack the player
            Attack(playerHealth);

            // Set next allowed attack time
            attackTime = Time.time + attackCooldown;
        }
    }

  
    private void Die() 
    {

        Destroy(gameObject);
    }

    // Shows the range of the player/enemy in the scene, good for adjusting
    private void OnDrawGizmosSelected()
    {
        // Display the enemy's 3D detection range in the scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
