using RatGamesStudios.OperationDeratization.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RatGamesStudios.OperationDeratization.UI.Menu
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private GameObject endGameScreen;
        [SerializeField] private TextMeshProUGUI endGameText;
        [SerializeField] private GameObject optionsMenu;
        [SerializeField] private WindowManager windowManager;
        private GameObject player;
        private PlayerHealth playerHealth;
        private PlayerShoot playerShoot;
        private PlayerInventory playerInventory;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerHealth = player.GetComponent<PlayerHealth>();
            playerShoot = player.GetComponent<PlayerShoot>();
            playerInventory = player.GetComponent<PlayerInventory>();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && playerHealth.isAlive)
            {
                if (!optionsMenu.activeSelf)
                {
                    if (GameIsPaused)
                        Resume();
                    else
                        Pause();
                }
                else
                    CloseOptions();
            }
            if (!playerHealth.isAlive)
            {
                Transform[] children = transform.GetComponentsInChildren<Transform>(true);

                foreach (Transform child in children)
                {
                    if (child != transform && !child.name.Contains("Menu") && !IsPartOfMenu(child))
                        child.gameObject.SetActive(false);
                }

                endGameScreen.SetActive(true);
                endGameText.text = "GAME   OVER";
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
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
            Cursor.visible = false;
            playerShoot.enabled = true;
            playerInventory.enabled = true;
        }
        private void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            playerShoot.enabled = false;
            playerInventory.enabled = false;
        }
        private void CloseOptions()
        {
            windowManager.AbortChanges();
            optionsMenu.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
        public void LoadMenu()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
            AudioListener.pause = false;
            SceneManager.LoadScene(0);
        }
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}