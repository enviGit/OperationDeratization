using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) 
                Resume();
            else
                Pause();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerHealth playerStatus = FindObjectOfType<PlayerHealth>();
       
        if (playerStatus.isAlive)
        {
            PlayerShoot pointer = FindObjectOfType<PlayerShoot>();
            pointer.enabled = true;
            PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
            inventory.enabled = true;
            Tracker tracker = FindObjectOfType<Tracker>();
            tracker.indicator.gameObject.SetActive(true);
            tracker.enabled = true;
        }
    }
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerShoot pointer = FindObjectOfType<PlayerShoot>();
        pointer.enabled = false;
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.enabled = false;
        Tracker tracker = FindObjectOfType<Tracker>();
        tracker.indicator.gameObject.SetActive(false);
        tracker.enabled = false;
    }
    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }
}