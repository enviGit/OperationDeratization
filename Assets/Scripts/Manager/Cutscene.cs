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

        private void Start()
        {
            skipText.gameObject.SetActive(Settings.CanSkipCutscene);
            StartCoroutine(WaitForCutsceneEnd());

            if (Settings.CanSkipCutscene)
                StartCoroutine(ShowSkipPrompt());
        }
        private IEnumerator WaitForCutsceneEnd()
        {
            yield return new WaitForSeconds((float)timeline.duration);

            Settings.CanSkipCutscene = true;
            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        private IEnumerator ShowSkipPrompt()
        {
            while (!Input.GetKeyDown(KeyCode.Space))
                yield return null;

            timeline.Stop();
            sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}