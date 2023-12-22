using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PlayerHealth playerStatus;
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private TextMeshProUGUI endGameText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerStatus.isAlive)
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

            endGameScreen.SetActive(true);
            endGameText.text = "GAME   OVER";
            Transform endGameChild = endGameScreen.transform.GetChild(0).transform;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            StartCoroutine(ActivateChildrenRandomly(endGameChild));
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
        PlayerShoot pointer = FindObjectOfType<PlayerShoot>();
        pointer.enabled = true;
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.enabled = true;
    }
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        AudioListener.pause = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerShoot pointer = FindObjectOfType<PlayerShoot>();
        pointer.enabled = false;
        PlayerInventory inventory = FindObjectOfType<PlayerInventory>();
        inventory.enabled = false;
    }
    public void LoadMenu()
    {
        Time.timeScale = 1f;
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
        SceneManager.LoadScene(3);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private IEnumerator ActivateChildrenRandomly(Transform parent)
    {
        int childCount = parent.childCount;
        bool[] activatedChildren = new bool[childCount];

        while (!AllChildrenActivated(activatedChildren))
        {
            int randomChildIndex = GetRandomChildIndex(childCount, activatedChildren);

            if (randomChildIndex != -1)
            {
                Transform child = parent.GetChild(randomChildIndex);
                child.gameObject.SetActive(true);
                activatedChildren[randomChildIndex] = true;

                yield return new WaitForSeconds(0.2f);
            }
        }
    }
    private int GetRandomChildIndex(int childCount, bool[] activatedChildren)
    {
        int randomIndex = Random.Range(0, childCount);

        for (int i = 0; i < childCount; i++)
        {
            int index = (randomIndex + i) % childCount;

            if (!activatedChildren[index])
                return index;
        }

        return -1;
    }
    private bool AllChildrenActivated(bool[] activatedChildren)
    {
        foreach (bool activated in activatedChildren)
        {
            if (!activated)
                return false;
        }

        return true;
    }
}