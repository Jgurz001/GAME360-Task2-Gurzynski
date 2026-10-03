using System;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    public static event Action<int> OnScoreChanged;
    private int score;
    private void OnEnable() => Coin.OnCoinCollected += AddScore;
    private void OnDisable() => Coin.OnCoinCollected -= AddScore;
    private void AddScore(int amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
}