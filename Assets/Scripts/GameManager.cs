using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Reference to our game over panel
    [SerializeField] private GameObject GameOverPanel;

    public static GameManager Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // If the game over panel is hidden at the beginning of the game
        if(GameOverPanel != null){
            // Set to inactive aka false
            GameOverPanel.SetActive(false);
        }
        
    }

    public void postGameOver() 
    {
        // Set the panel to true and post it on players screen
        GameOverPanel.SetActive(true);
        // Freeze everything in the game
        Time.timeScale = 0f;
    }

    public void resetGame()
    {

        // Reset the time scale back to normal before reloading
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            SceneManager.LoadScene(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Score() { 
    }
}
