using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private Slider progressBar;
    private float target;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    public async void LoadScene(int sceneID)
    {
        target = 0;
        progressBar.value = 0;
        var scene = SceneManager.LoadSceneAsync(sceneID);
        scene.allowSceneActivation = false;
        loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);
        scene.allowSceneActivation = true;
        loaderCanvas.SetActive(false);
    }
    private void Update()
    {
        progressBar.value = Mathf.MoveTowards(progressBar.value, target, 3 * Time.deltaTime);
    }
}
