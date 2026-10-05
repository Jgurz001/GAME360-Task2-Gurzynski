using UnityEngine;
using TMPro;
public class ScoreUI : MonoBehaviour
{
    // Texxt that will display the score
    public TMP_Text scoreText;

    // Listen for the score changes
    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScore;
        
    }
    //Stop listening for the score changes
    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateScore;
    }
   
    void Start()
    {
        // Display start score
        scoreText.text = "Score: 0";
    }
  
    // Update the new score
    void UpdateScore(int s) =>
    scoreText.text = "Score: " + s;
}