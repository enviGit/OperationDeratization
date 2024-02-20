using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class Cutscene : MonoBehaviour
    {
        [SerializeField] private PlayableDirector timeline;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private TextMeshProUGUI skipText;
        [SerializeField] private bool isWinCutsceneActive = false;
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject[] gOToDisable;
        [SerializeField] private Material vignette;

        private void Start()
        {
            if (!isWinCutsceneActive)
            {
                skipText.gameObject.SetActive(Settings.CanSkipCutscene);
                StartCoroutine(WaitForCutsceneEnd());

                if (Settings.CanSkipCutscene)
                    StartCoroutine(ShowSkipPrompt());
            }
            else
            {
                skipText.gameObject.SetActive(Settings.CanSkipWinCutscene);
                StartCoroutine(WaitForWinCutsceneEnd());

                if (Settings.CanSkipWinCutscene)
                    StartCoroutine(ShowWinSkipPrompt());
            }
        }
        private void Update()
        {
            if (isWinCutsceneActive && timeline.time >= 58f && vignette != null)
            {
                float voronoiIntensity = Mathf.Lerp(0f, 0.75f, 1f);
                float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1f);
                vignette.SetFloat("_VoronoiIntensity", voronoiIntensity);
                vignette.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
            }
        }
        private IEnumerator WaitForCutsceneEnd()
        {
            yield return new WaitForSeconds((float)timeline.duration);

            Settings.CanSkipCutscene = true;

            foreach (GameObject child in gOToDisable)
            {
                if (child.activeSelf)
                    child.SetActive(false);
            }

            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        private IEnumerator ShowSkipPrompt()
        {
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;

            timeline.Stop();

            if (vignette != null)
            {
                vignette.SetFloat("_VoronoiIntensity", 0f);
                vignette.SetFloat("_VignetteRadiusPower", 0f);
            }

            foreach (GameObject child in gOToDisable)
            {
                if (child.activeSelf)
                    child.SetActive(false);
            }

            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        private IEnumerator WaitForWinCutsceneEnd()
        {
            yield return new WaitForSeconds((float)timeline.duration);

            foreach (GameObject child in gOToDisable)
            {
                if (child.activeSelf)
                    child.SetActive(false);
            }

            if (vignette != null)
            {
                vignette.SetFloat("_VoronoiIntensity", 0f);
                vignette.SetFloat("_VignetteRadiusPower", 0f);
            }

            Settings.CanSkipWinCutscene = true;
            victoryScreen.SetActive(true);
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private IEnumerator ShowWinSkipPrompt()
        {
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;

            timeline.Stop();

            foreach (GameObject child in gOToDisable)
            {
                if (child.activeSelf)
                    child.SetActive(false);
            }

            victoryScreen.SetActive(true);
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        private void OnDisable()
        {
            if (vignette != null)
            {
                vignette.SetFloat("_VoronoiIntensity", 0f);
                vignette.SetFloat("_VignetteRadiusPower", 1f);
            }
        }
    }
}