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

    private bool gameStarted = false;
    private static bool gameReplay = false;
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
        

        isGameOver = false;
        gameStarted = false;

        //Hide both end screens
        GameOverPanel.SetActive(false);
        WinPanel.SetActive(false);

        // Check if the game is being replayed
        if (gameReplay)
        {
            // immediate replay begins new run
            gameReplay = false;
            startGame();
        }
        else
        {
            // if its the first launch of the game then it will display the main menu
            mainMenu();
        }


    }

    private void Update()
    {
        // Stop checking after Game Over has happened, had to add gaemstarted so player does not lose before start is pressed
        if (!gameStarted || isGameOver || player == null) return;

        // Display game over when the player falls too far, spent an hour realizing I never set this
        if (player.position.y <= fallKillZone) postGameOver();
    }
    public void postGameOver()
    {
        // Current run as finished
        isGameOver = true;
        gameStarted = false; 

        // Display only the game over screen
        GameOverPanel.SetActive(true);
        WinPanel.SetActive(false);
        MainMenu.SetActive(false);

        //release and display the mouse for the UI interaction
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze everything in the game
        Time.timeScale = 0f;
    }

    public void CheckWinCondition(int currentScore)
    {
        Debug.Log(
    $"Win check: Score={currentScore}, Started={gameStarted}, Ended={isGameOver}"
);

        // Only check the victory during the active gameplay
        if (!gameStarted || isGameOver) 
        {
            return;
        
        }

        if (currentScore >= winScore) 
        {
            Debug.Log("Win score reached");
            winScreen();
        }

    }

    public void winScreen()
    {
        // Stop the screen from showing
        isGameOver = true;
        gameStarted = false;

        //Display only the win scren
        WinPanel.SetActive(true);
        GameOverPanel.SetActive(false);
        MainMenu.SetActive(false);

        //Unlock the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Freeze everything in the game
        Time.timeScale = 0f;

    }

    public void mainMenu() 
    {
        gameStarted = false;
        isGameOver = false;

        MainMenu.SetActive(true);
        WinPanel.SetActive(false);
        GameOverPanel.SetActive(false);

        // Pause the game while the menu is open
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void resetGame()
    {
        Time.timeScale = 1f;

        // Resets the score stored within the persistent Singleton (Took me an hour to realize I never did this)
        if (ScoreManager.Instance != null) 
        {
            ScoreManager.Instance.resetScore();
        }

        // This will cause Start method to skip the main menu after replay
        gameReplay = true;

        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void startGame()
    {
        Debug.Log("START GAME CALLED");

        gameStarted = true;
        isGameOver = false;

        MainMenu.SetActive(false);
        GameOverPanel.SetActive(false);
        WinPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void quitGame() 
    {
        Application.Quit();
    
    }



}
