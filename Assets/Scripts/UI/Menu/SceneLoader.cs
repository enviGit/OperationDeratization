using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject LoaderUI;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject gOToDeactivate;

    public void LoadScene(int index)
    {
        StartCoroutine(LoadScene_Coroutine(index));
    }
    public IEnumerator LoadScene_Coroutine(int index)
    {
        gOToDeactivate.SetActive(false);
        progressSlider.value = 0;
        LoaderUI.SetActive(true);
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(index);
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
            }

            yield return null;
        }
    }
}
