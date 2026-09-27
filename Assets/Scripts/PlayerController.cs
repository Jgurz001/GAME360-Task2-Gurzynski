using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 7f;
    //For rotating the character while moving, 720 is degrees per second
    public float rotationSpeed = 720f;

    [Header("Components")]
    private Rigidbody rb;
    private bool isGrounded = false;



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

    }

    // Called once per frame
    void Update()
    {
        HandleMovement();
        HandleJumping();
    }

    void HandleMovement()
    {
        // Get input from keyboard
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right arrows
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down arrows

        // Create movement vector
        Vector3 movement = new Vector3(horizontal, 0f, vertical);

        //With the help of google, this allows the player to rotate
        if (movement != Vector3.zero) {
            Quaternion playerRotation = Quaternion.LookRotation(movement);

            //Quaternion is a 
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                playerRotation,
                rotationSpeed = Time.deltaTime

                );
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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


}