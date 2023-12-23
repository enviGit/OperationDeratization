using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ReferenceFinder.Extensions
{
    /// <summary>
    /// Static class with extension methods related to scenes.
    /// </summary>
    public static class SceneExtensions
    {
        /// <summary>
        /// Extension method that gets all GameObjects from the scene.
        /// </summary>
        /// <param name="scene">
        /// Scene to find the objects
        /// </param>
        /// <returns>
        /// Array with all of the GameObjects found in the scene
        /// </returns>
        public static GameObject[] FindGameObjectsInScene(this Scene scene)
        {
            List<GameObject> sceneObjects = new List<GameObject>();
            GameObject[] sceneRootObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in sceneRootObjects)
            {
                GameObject[] gameObjects = gameObject.GetAllAssociatedGameObjects();
                foreach (GameObject gameObjectInGameObject in gameObjects)
                {
                    sceneObjects.Add(gameObjectInGameObject);
                }
            }
            return sceneObjects.ToArray();
        }

        /// <summary>
        /// Extension method that checks if a game object is in the scene.
        /// </summary>
        /// <param name="scene">
        /// Scene to find the object
        /// </param>
        /// <param name="gameObject">
        /// GameObject to find
        /// </param>
        /// <param name="foundObject">
        /// GameObject found
        /// </param>
        /// <returns>
        /// True and the object found if any was found, false and null otherwise
        /// </returns>
        public static bool ContainsGameObject(this Scene scene, GameObject gameObject, out GameObject foundObject)
        {
            GameObject[] sceneObjects = scene.FindGameObjectsInScene();
            foreach (GameObject sceneObject in sceneObjects)
            {
                if (gameObject.Equals(sceneObject))
                {
                    foundObject = sceneObject;
                    return true;
                }
            }
            foundObject = null;
            return false;
        }
    }
}
