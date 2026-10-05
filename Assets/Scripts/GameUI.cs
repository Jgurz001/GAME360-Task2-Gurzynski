using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("Game Over Settings")]
    //Reference to our game over panel
    [SerializeField] private GameObject GameOverPanel;

    // Player used for checking fall distance
    [SerializeField] private Transform player;
    // Lowest allowed player position, used for kill zone
    [SerializeField] private float fallKillZone = -10f;
    //Tracking for end of the run
    private bool isGameOver = false;

    [Header("Win Settings")]
    // Victory screen 
    [SerializeField] private GameObject WinPanel;
    //Score that is required to win
    [SerializeField] private int winScore = 50;

    [Header("Main Menu Settings")]
    //Main menu screen
    [SerializeField] private GameObject MainMenu;
    // Tracks if the gameplay has started
    private bool gameStarted = false;
    //Checks if the scene is replaying
    private static bool gameReplay = false;

    /// <summary>
    ///  Listener for score change
    /// </summary>
    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += CheckWinCondition;
    }

    // Stop listening for the score change
    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= CheckWinCondition;
    }

    private void Start()
    {
        
        // Reset the state of the game
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
        // Win after reaching the required score
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
        // Stop the gameplay
        gameStarted = false;
        isGameOver = false;

        //Show the main menu
        MainMenu.SetActive(true);
        WinPanel.SetActive(false);
        GameOverPanel.SetActive(false);

        // Pause the game while the menu is open
        Time.timeScale = 0f;
        // Release the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void resetGame()
    {
        // Return game to normal speed
        Time.timeScale = 1f;

        // Resets the score stored within the persistent Singleton (Took me an hour to realize I never did this)
        if (ScoreManager.Instance != null) 
        {
            ScoreManager.Instance.resetScore();
        }

        // This will cause Start method to skip the main menu after replay
        gameReplay = true;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void startGame()
    {
        Debug.Log("START GAME CALLED");

        // Begin the gameplay
        gameStarted = true;
        isGameOver = false;

        // Show no screens 
        MainMenu.SetActive(false);
        GameOverPanel.SetActive(false);
        WinPanel.SetActive(false);

        // Set game to normal speed/ resume
        Time.timeScale = 1f;
        //Lock the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void quitGame() 
    {
        // Close the entire game
        Application.Quit();
    
    }



}
