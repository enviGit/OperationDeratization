using System;
using System.Collections;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ReferenceFinder.Utils
{
    /// <summary>
    /// Static class responsible with general purpose functions/methods
    /// related to visual elements and their creation.
    /// </summary>
    public static class VisualElementUtils
    {
        /// <summary>
        /// Helper method that creates an enum field.
        /// </summary>
        /// <param name="onChangeCallback">
        /// Callback for when the value changes
        /// </param>
        /// <param name="label">
        /// Visual element label
        /// </param>
        /// <param name="initEnum">
        /// Enum used to initialize the field
        /// </param>
        /// <returns>
        /// Ready to use enum field
        /// </returns>
        public static EnumField CreateEnumField(EventCallback<ChangeEvent<Enum>> onChangeCallback, string label, Enum initEnum)
        {
            EnumField enumField = new EnumField()
            {
                label = label
            };
            enumField.Init(initEnum);
            enumField.RegisterValueChangedCallback(onChangeCallback);
            return enumField;
        }

        /// <summary>
        /// Helper method that creates a button.
        /// </summary>
        /// <param name="onClickCallback">
        /// Callback for when the button is pressed
        /// </param>
        /// <param name="text">
        /// Button display text value
        /// </param>
        /// <param name="tooltip">
        /// Button's tooltip
        /// </param>
        /// <returns>
        /// Ready to use button
        /// </returns>
        public static Button CreateButton(Action onClickCallback, string text, string tooltip)
        {
            return new Button(onClickCallback)
            {
                text = text,
                tooltip = tooltip, 
            };
        }

        /// <summary>
        /// Helper method that creates a list view.
        /// </summary>
        /// <param name="items">
        /// Items to be shown
        /// </param>
        /// <param name="bindItem">
        /// Callback for binding an item
        /// </param>
        /// <param name="makeItem">
        /// Callback to make an item
        /// </param>
        /// <param name="itemsDisplayed">
        /// Number of items displayed at one time (Default value = 8)
        /// </param>
        /// <param name="height">
        /// Item height (Default value = 20)
        /// </param>
        /// <param name="selectionType">
        /// Item selection type (Default value = SelectionType.None)
        /// </param>
        /// <returns>
        /// Ready to use list view
        /// </returns>
        public static ListView CreateListView(IList items, Action<VisualElement, int> bindItem, Func<VisualElement> makeItem, int itemsDisplayed = 8, int height = 20, SelectionType selectionType = SelectionType.None)
        {
            ListView listView = new ListView(items, height, makeItem, bindItem);
            listView.selectionType = selectionType;
            listView.style.minHeight = items.Count > itemsDisplayed ? itemsDisplayed * height : items.Count * height;
            return listView;
        }

        /// <summary>
        /// Helper method that creates an object field.
        /// </summary>
        /// <param name="onChangeCallback">
        /// Callback for when the value changes
        /// </param>
        /// <param name="objectType">
        /// Object field type filter
        /// </param>
        /// <param name="label">
        /// Visual element label
        /// </param>
        /// <param name="allowSceneObjects">
        /// Allow scene objects to be viewed in the search window
        /// </param>
        /// <returns>
        /// Ready to use object field
        /// </returns>
        public static ObjectField CreateObjectField(EventCallback<ChangeEvent<Object>> onChangeCallback, Type objectType, string label, bool allowSceneObjects = false)
        {
            ObjectField objectField = new ObjectField(label);
            objectField.allowSceneObjects = allowSceneObjects;
            objectField.objectType = objectType;
            objectField.RegisterValueChangedCallback(onChangeCallback);
            return objectField;
        }
    }
}
