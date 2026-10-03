using UnityEngine;
using TMPro;
public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;
    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScore;
        //Coin.OnCoinCollectedString += UpdateMessage;
    }
    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateScore;
    }
    // Start is called once before the first execution of Update after theMonoBehaviour is created
    private void Awake()
    {
    }
    void Start()
    {
        scoreText.text = "Score: 0";
    }
    // Update is called once per frame
    void Update()
    {
    }
    void UpdateScore(int s) =>
    scoreText.text = "Score: " + s;
}