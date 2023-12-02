using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    private PlayerHealth playerStatus;

    private void Start()
    {
        playerStatus = FindObjectOfType<PlayerHealth>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) 
                Resume();
            else
                Pause();
        }
        if (!playerStatus.isAlive)
        {
            Transform[] children = transform.GetComponentsInChildren<Transform>(true);

            foreach (Transform child in children)
            {
                if (child != transform && !child.name.Contains("Menu") && !IsPartOfMenu(child))
                    child.gameObject.SetActive(false);
            }
        }
    }
    private bool IsPartOfMenu(Transform obj)
    {
        Transform parent = obj.parent;

        while (parent != null)
        {
            if (parent.name.Contains("Menu"))
                return true;

            parent = parent.parent;
        }

        return false;
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
       
        if (playerStatus.isAlive)
        {
            PlayerShoot pointer = FindObjectOfType<PlayerShoot>();
            pointer.enabled = true;
        }
    }
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerShoot pointer = FindObjectOfType<PlayerShoot>();
        pointer.enabled = false;
    }
    public void LoadMenu()
    {
        Debug.Log("Loading Menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }
}