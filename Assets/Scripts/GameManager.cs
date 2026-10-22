using System;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    // Provides global access to one scoremanager 
    public static ScoreManager Instance { get; private set; }
    // Announces when the score has changed
    public static event Action<int> OnScoreChanged;
    // Variable for the score
    private int score;
    // Allows other scripts to read the score
    public int CurrentScore => score;


    private void Awake()
    {
        // Destroy duplicate ScoreManagers after scene reload.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Store this as a singleton
        Instance = this;

        // Keep the original ScoreManager through scene reloads.
        DontDestroyOnLoad(gameObject);
    }
    //Listen for coin collects
    private void OnEnable() => Coin.OnCoinCollected += AddScore;
    //Stop listening for collected coins
    private void OnDisable() => Coin.OnCoinCollected -= AddScore;
    private void AddScore(int amount)
    {
        // Add coin value
        score += amount;
        //Notify every score listener
        OnScoreChanged?.Invoke(score);
    }

   
    public void resetScore() 
    {
        // Set the score back to zero
        score = 0;
        // Notify every score listener about this reset
        OnScoreChanged?.Invoke(score);

        Debug.Log("Score has been RESET to 0");
    
    }
}