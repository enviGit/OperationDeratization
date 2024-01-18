using UnityEngine;
using UnityEngine.Playables;

public class StartSceneWithTimeline : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public float timeScale = 2.0f;

    void Start()
    {
        if (playableDirector == null)
            return;

        PlayTimelineWithSpeedUp();
    }

    void PlayTimelineWithSpeedUp()
    {
        playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(timeScale);
        playableDirector.Play();
    }
}