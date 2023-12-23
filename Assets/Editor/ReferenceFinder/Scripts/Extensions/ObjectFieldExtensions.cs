using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ReferenceFinder.Extensions
{
    /// <summary>
    /// Static class with extension methods related to the ObjectField.
    /// </summary>
    public static class ObjectFieldExtensions
    {
        /// <summary>
        /// Extension method that disables the search button from
        /// the object field.
        /// </summary>
        /// <param name="objectField">
        /// ObjectField to be modified
        /// </param>
        public static void DisableSearchObjectButton(this ObjectField objectField)
        {
            objectField.Children().Last().Children().Last().SetEnabled(false);
        }

        /// <summary>
        /// Extension method that disables the drag and drop functionality
        /// from the object field.
        /// </summary>
        /// <param name="objectField">
        /// ObjectField to be modified
        /// </param>
        public static void DisableDragAndDrop(this ObjectField objectField)
        {
            objectField.RegisterCallback<DragEnterEvent>(PreventEvent);
            objectField.RegisterCallback<DragUpdatedEvent>(PreventEvent);
            objectField.RegisterCallback<DragExitedEvent>(PreventEvent);
            objectField.RegisterCallback<DragPerformEvent>(PreventEvent);
            objectField.RegisterCallback<DragLeaveEvent>(PreventEvent);
        }

        /// <summary>
        /// Helper callback to prevent events from propagating.
        /// </summary>
        /// <param name="eventBase">
        /// Event base data
        /// </param>
        private static void PreventEvent(EventBase eventBase)
        {
            eventBase.PreventDefault();
            eventBase.StopImmediatePropagation();
        }
    }
}
