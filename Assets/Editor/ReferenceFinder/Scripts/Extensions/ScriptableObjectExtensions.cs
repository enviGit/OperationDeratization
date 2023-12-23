using ReferenceFinder.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ReferenceFinder.Extensions
{
    /// <summary>
    /// Static class with extension methods related to the ScriptableObject class.
    /// </summary>
    public static class ScriptableObjectExtensions
    {
        /// <summary>
        /// Extension method that checks if the game object has any references
        /// related to the given Object parameter, 'to', within its components
        /// and returns the result.
        /// </summary>
        /// <param name="scriptableObject">
        /// ScriptableObject to be check for references
        /// </param>
        /// <param name="to">
        /// Object to find references
        /// </param>
        /// <returns>
        /// True if a reference was found, false otherwise
        /// </returns>
        public static bool HasReferenceTo(this ScriptableObject scriptableObject, Object to)
        {
            return EditorUtils.HasReferenceTo<ScriptableObject>(scriptableObject, to);
        }
    }
}
