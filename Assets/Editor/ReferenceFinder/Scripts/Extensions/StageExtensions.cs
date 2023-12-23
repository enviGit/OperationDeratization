using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ReferenceFinder.Extensions
{
    /// <summary>
    /// Static class with extension methods related to stages.
    /// </summary>
    public static class StageExtensions
    {
        /// <summary>
        /// Extension method that gets all GameObjects from the stage.
        /// </summary>
        /// <param name="stage">
        /// Stage to find the objects
        /// </param>
        /// <returns>
        /// Array with all of the GameObjects found in the stage
        /// </returns>
        public static GameObject[] FindGameObjectsInStage(this Stage stage)
        {
            List<GameObject> stageObjects = new List<GameObject>();
            Transform[] stageObjectTransforms = stage.FindComponentsOfType<Transform>();
            foreach (Transform objectTransform in stageObjectTransforms)
            {
                stageObjects.Add(objectTransform.gameObject);
            }
            return stageObjects.ToArray();
        }
    }
}
