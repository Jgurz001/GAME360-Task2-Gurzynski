using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    //Reference to our game over panel
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private Transform player;

    [SerializeField] private float fallKillZone = -10f;

    private bool isGameOver = false;

    private void Start()
    {
        // Game is running when the scene begins
        Time.timeScale = 1f;

        //Hide game over screen
        GameOverPanel.SetActive(false);
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

    public void resetGame()
    {
        Time.timeScale = 1f;

        // Lock mouse for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Reset the time scale back to normal before reloading
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

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
