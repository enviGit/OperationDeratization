using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RatGamesStudios.OperationDeratization.UI.Menu;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private GameObject LoaderUI;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private GameObject[] gObjectsToDeactivate;
        private AudioSource loadingSound;
        private Image loadingImage;

        private void Start()
        {
            loadingSound = GetComponent<AudioSource>();
            loadingImage = LoaderUI.transform.GetChild(0).GetComponent<Image>();
        }
        public void RestartScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            loadingImage.sprite = Resources.Load<Sprite>("Art/Loading/" + currentSceneIndex);
            StartCoroutine(LoadScene_Coroutine(currentSceneIndex));
        }
        public void LoadScene(int index)
        {
            loadingImage.sprite = Resources.Load<Sprite>("Art/Loading/" + index);
            StartCoroutine(LoadScene_Coroutine(index));
        }
        public IEnumerator LoadScene_Coroutine(int index)
        {
            foreach (GameObject child in gObjectsToDeactivate)
                child.SetActive(false);
            
            progressSlider.value = 0;
            LoaderUI.SetActive(true);
            PauseMenu.GameIsPaused = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            loadingSound.Play();
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
            asyncOperation.allowSceneActivation = false;
            float progress = 0;

            while (!asyncOperation.isDone)
            {
                progress = Mathf.MoveTowards(progress, asyncOperation.progress, Time.deltaTime);
                progressSlider.value = progress;

                if (progress >= 0.9f)
                {
                    progressSlider.value = 1;
                    asyncOperation.allowSceneActivation = true;
                    loadingSound.Stop();
                }

                yield return null;
            }
        }
    }
}