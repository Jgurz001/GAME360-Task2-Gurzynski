using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static event Action<int> OnCoinCollected;
    [Header("Coin Settings")]
    public int scoreValue = 10;
    public float rotationSpeed = 50f;
    public Vector3 spin = new Vector3(30f, 30f, 30f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the collectible for visual appeal
        transform.Rotate(spin * Time.deltaTime);
        
        
    }
    
    // Called when another collider enters this trigger collider
    void OnTriggerEnter(Collider other)
    {
        // Check if the player touched this collectible
        if (other.CompareTag("Player"))
        {
            // Get the PlayerController component
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                // Add score to player
                //player.AddScore(scoreValue);
                OnCoinCollected?.Invoke(scoreValue);

                // Log collection
                Debug.Log("COLLECTED: " + gameObject.name + " for " + scoreValue + " points!");

                // Destroy this collectible
                Destroy(gameObject);
            }
        }
    }
}
