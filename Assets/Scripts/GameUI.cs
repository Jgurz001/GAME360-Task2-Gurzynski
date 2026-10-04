using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("Game Over Settings")]
    //Reference to our game over panel
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private Transform player;
    [SerializeField] private float fallKillZone = -10f;
    private bool isGameOver = false;

    [Header("Win Settings")]
    [SerializeField] private GameObject WinPanel;
    [SerializeField] private int winScore = 50;

    [Header("Main Menu Settings")]
    [SerializeField] private GameObject MainMenu;
    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += CheckWinCondition;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= CheckWinCondition;
    }

    private void Start()
    {
        // Game is running when the scene begins
        Time.timeScale = 1f;

        isGameOver = false;

        //Hide game over screen
        GameOverPanel.SetActive(false);
        WinPanel.SetActive(false);
    }

    private void Update()
    {
        // Stop checking after Game Over has happened
        if (isGameOver || player == null) return;

        // Display game over when the player falls too far, spent an hour realizing I never set this
        if (player.position.y <= fallKillZone) postGameOver();
    }
    public void postGameOver()
    {
        // Set the panel to true and post it on players screen
        GameOverPanel.SetActive(true);

        //release and display the mouse for the UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze everything in the game
        Time.timeScale = 0f;
    }

    public void CheckWinCondition(int currentScore)
    {
        if (isGameOver) 
        {
            return;
        
        }

        if (currentScore >= winScore) 
        {
            winScreen();
        }

    }

    public void winScreen()
    {
        // Stop the screen from showing
        isGameOver = true;

        //Display only the win scren
        WinPanel.SetActive(true);
        GameOverPanel.SetActive(false);

        //Hide main menu panel
        if (MainMenu != null) 
        {
            MainMenu.SetActive(false);
        
        }

        //Unlock the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze everything in the game
        Time.timeScale = 0f;

    }

    
    public void resetGame()
    {
        Time.timeScale = 1f;

        // Resets the score stored within the persistent Singleton (Took me an hour to realize I never did this)
        if (ScoreManager.Instance != null) 
        {
            ScoreManager.Instance.resetScore();
        }

        // Lock mouse for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reset the time scale back to normal before reloading
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void startGame()
    {
        MainMenu.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void quitGame() 
    {
        Application.Quit();
    
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(0);
        }
    }

}
