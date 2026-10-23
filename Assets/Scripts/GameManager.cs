using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    //Current state of whole game
    public GameState State { get; private set; } = GameState.Paused;

    //Notifies observers when the game state has changed
    public static event Action<GameState> OnGameStateChanged;

    // Prevents pausing before the game begins
    private bool pauseEnabled = false;

    // Provides global access to one scoremanager 
    public static GameManager Instance { get; private set; }
    // Announces when the score has changed
    public static event Action<int> OnScoreChanged;
    // Variable for the score
    private int score;
    // Allows other scripts to read the score
    public int CurrentScore => score;

    void Update()
    {
        // Ignore pause input when pausing is disabled
        if (!pauseEnabled || Keyboard.current == null)
        {
            return;
        }

        //Continue only when ESC is pressed
        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }
        // Toggle between Playing and Paused
        switch (State)
        {
            case GameState.Paused: SwitchTo(GameState.Playing);  break;
            case GameState.Playing: SwitchTo(GameState.Paused); break;
            // DO NOT allow pausing after game ends
            case GameState.Won:
            case GameState.Lost: break;
        }

    }
    //Method that is allowd to change game state
    private void SwitchTo(GameState next)
    {
        // Do nothing if already in requested state
        if (State == next) return;

        // For debugging
        Debug.Log("Game: " + State + " -> " + next);

        // Enter new state
        State = next;

        // Only playing utilizese normal game speed
        Time.timeScale = State == GameState.Playing ? 1f : 0f;

        //Notify UI and other obserevers
        OnGameStateChanged?.Invoke(State);
        
    }

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
    // Starts game and enables pausing
    public void gameStart() 
    {
        pauseEnabled = true;
        SwitchTo(GameState.Playing);
    }
    // End game with a win or lose
    public void gameEnd(GameState result) 
    {
        // Will ONLY accept valid ending states
        if (result != GameState.Won && result != GameState.Lost) { return; }
        // Disable pausing after the game ends
        pauseEnabled = false;
        SwitchTo(result);
    }

    public void resumeGame()
    {
        if (State == GameState.Paused)
        {
            SwitchTo(GameState.Playing);
        }
    }
}