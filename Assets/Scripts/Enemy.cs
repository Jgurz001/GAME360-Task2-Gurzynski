using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]

    //Number of hits the enemy can take
    [SerializeField] float health, maxHealth = 3f;

    //Normal enemy movement speed
    public float moveSpeed = 2f;

    //How quickly the enemy turns toard the player
    public float rotateSpeed = 360f;

    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1.5f;
    //Enemy needs a cool down because spamming would cause issues
    [SerializeField] private float attackCooldown = 1f;
    private float attackTime;

    [SerializeField] FloatingHealthBar healthBar;

    [Header("AI")]

    //Enemy begins chasing when the player enters this range
    public float detectionRange = 5f;

    //Ref to the player's transform
    private Transform player;

    //Reference to the enemy's 3D RigidBoy
    private Rigidbody rb;

    //Reference to the Animator Component
    private Animator animator;

    private void Awake()
    {
        //Get the Rgidbody attached to the enemy
        rb = GetComponent<Rigidbody>();

        //Component here in the event Animator is located on the model
        animator = GetComponentInChildren<Animator>();

        if (animator == null) 
        {
            Debug.Log("Enemy could not find the Animator");
        
        
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        if (healthBar != null) {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
        //Find the gameobject tagged player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
 

        //Store the player's transform if one was found
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

        // Start with the enemy's normal movement speed
        float currentSpeed = moveSpeed;

        //Increase enemy speed as the player's score increases, will be written later


        //Calculate the 3D distance between the enemy and player
        float distance = Vector3.Distance(transform.position, player.position);

        //Preserve the enemy's current vertical velocity
        //this allows gravity to continue working
        float verticalVelocity = rb.linearVelocity.y;

        // The enemy begins each physics frame considered stationary
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
                direction.Normalize();

                // Enemy is actively moving (Spent an hour realizing I was missing this)
                isMoving = true;

                // Move toward the player on the z and z axes
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

        if (animator != null)
        {
            animator.SetBool("isRunning", isMoving);
        
        
        }
      
    
    }

    public void TakeDamage(float damageAmount)
    {
        
        health -= damageAmount;

        Debug.Log($"Enemy took {damageAmount} damage!!! Current health: {health}");
        healthBar.UpdateHealthBar(health, maxHealth);
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
        if (Time.time < attackTime) 
        {
            return;
        
        }

        //Search the collided object and its parents
        PlayerController playerHealth = collision.gameObject.GetComponentInParent<PlayerController>();

        if (playerHealth != null) 
        {
            Attack(playerHealth);

            //cooldown
            attackTime = Time.time + attackCooldown;
        }
    }

    //Death method that kills off the enemy, same will be applied in the player
    private void Die() 
    {

        Destroy(gameObject);
    }

    // This is the method that displays the enemies detection range, this is purely for visual testing
    private void OnDrawGizmosSelected()
    {
        // Display the enemy's 3D detection range in the scene view

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
