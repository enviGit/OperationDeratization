using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RatGamesStudios.OperationDeratization
{
    public class SceneStart : MonoBehaviour
    {
        public PlayableDirector timelineDirector;

        // Start is called before the first frame update
        void Start()
        {
            timelineDirector.Play();

        }

    }
}
