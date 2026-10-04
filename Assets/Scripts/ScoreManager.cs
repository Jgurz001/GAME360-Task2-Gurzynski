using System;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public static event Action<int> OnScoreChanged;
    private int score;

    public int CurrentScore => score;


    private void Awake()
    {
        // Destroy duplicate ScoreManagers after scene reload.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Keep the original ScoreManager through scene reloads.
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable() => Coin.OnCoinCollected += AddScore;
    private void OnDisable() => Coin.OnCoinCollected -= AddScore;
    private void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    // lol forgot this
    public void resetScore() 
    {
        score = 0;
        OnScoreChanged?.Invoke(score);

        Debug.Log("Score has been RESET to 0");
    
    }
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
}