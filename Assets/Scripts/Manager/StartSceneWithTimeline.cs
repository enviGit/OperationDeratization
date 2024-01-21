using UnityEngine;
using UnityEngine.Playables;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class StartSceneWithTimeline : MonoBehaviour
    {
        public PlayableDirector playableDirector;
        public float timeScale = 1.0f;

        private void Start()
        {
            if (playableDirector == null)
                return;

            PlayTimelineWithSpeedUp();
        }
        private void PlayTimelineWithSpeedUp()
        {
            playableDirector.playableGraph.GetRootPlayable(0).SetSpeed(timeScale);
            playableDirector.Play();
        }
    }
}