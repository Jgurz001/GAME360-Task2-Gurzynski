using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerController;

public class GameManager : MonoBehaviour
{
    public GameState State { get; private set; } = GameState.Paused;

    public static event Action<GameState> OnGameStateChanged;

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
        

        if (!pauseEnabled || Keyboard.current == null)
        {
            return;
        }

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }


        switch (State)
        {
            case GameState.Paused: SwitchTo(GameState.Paused); break;
            case GameState.Playing: SwitchTo(GameState.Playing); break;
            case GameState.Won:
            case GameState.Lost: break;
        }

    }
    private void SwitchTo(GameState next)
    {
        if (State == next) return;

        Debug.Log("Game: " + State + " -> " + next);

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
    public void gameStart() 
    {
        pauseEnabled = true;
        SwitchTo(GameState.Playing);
    }

    public void gameEnd(GameState result) 
    {
        if (result != GameState.Won && result != GameState.Lost) { return; }

        pauseEnabled = false;
        SwitchTo(result);
    }
}