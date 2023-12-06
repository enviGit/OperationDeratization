using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private GameObject attentionCanvas;

    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
    public void LoadScene(int sceneID)
    {
        attentionCanvas.SetActive(false);
        SceneLoader.instance.LoadScene(sceneID);
    }
}
