using System.Collections.Generic;
using ReferenceFinder.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReferenceFinder.Extensions
{
    /// <summary>
    /// Static class with extension methods related to the GameObject class.
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Extension method gets all children GameObjects from a GameObject.
        /// </summary>
        /// <param name="gameObject">
        /// GameObject to get all children objects
        /// </param>
        /// <returns>
        /// Array of GameObjects that were children of the main object (includes the
        /// object itself)
        /// </returns>
        public static GameObject[] GetAllAssociatedGameObjects(this GameObject gameObject)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform transform in transforms)
            {
                gameObjects.Add(transform.gameObject);
            }
            return gameObjects.ToArray();
        }

        /// <summary>
        /// Extension method that checks if the game object has any references
        /// related to the given Object parameter, 'to', within its components
        /// and returns the result.
        /// </summary>
        /// <param name="gameObject">
        /// GameObject to be check for references
        /// </param>
        /// <param name="to">
        /// Object to find references
        /// </param>
        /// <returns>
        /// True if a reference was found, false otherwise
        /// </returns>
        public static bool HasReferenceTo(this GameObject gameObject, Object to)
        {
            Component[] components = gameObject.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component == null)
                {
                    continue;
                }
                if (EditorUtils.HasReferenceTo<Component>(component, to))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Extension method that checks if the game object is a prefab asset
        /// </summary>
        /// <param name="gameObject">
        /// GameObject to be checked as a prefab
        /// </param>
        /// <returns>
        /// True if it's a prefab asset, false otherwise
        /// </returns>
        public static bool IsPrefabAsset(this GameObject gameObject)
        {
            return PrefabUtility.IsPartOfPrefabAsset(gameObject);
        }
    }
}
