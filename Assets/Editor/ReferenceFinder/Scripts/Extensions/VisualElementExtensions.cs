using UnityEngine.UIElements;

namespace ReferenceFinder.Extensions
{
    /// <summary>
    /// Static class with extension methods related to the VisualElements.
    /// </summary>
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Extension method that adds style class to a visual element and
        /// returns it (for chaining).
        /// </summary>
        /// <param name="visualElement">
        /// Visual element to add the style class
        /// </param>
        /// <param name="styleClass">
        /// Style class to add to the element
        /// </param>
        /// <returns>
        /// The visual element that had the style class
        /// </returns>
        public static VisualElement AddStyleClass(this VisualElement visualElement, string styleClass)
        {
            visualElement.AddToClassList(styleClass);
            return visualElement;
        }
    }
}
