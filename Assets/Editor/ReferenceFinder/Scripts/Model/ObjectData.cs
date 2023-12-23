using System.Collections.Generic;
using UnityEngine;

namespace ReferenceFinder.Model
{
    /// <summary>
    /// Class that corresponds to the data/info of an object,
    /// namely it's file IDs and the object itself.
    /// </summary>
    public class ObjectData
    {
        public string Name { get; private set; }
        public Component[] Components { get; private set; }
        public Object FoundObject { get; private set; }

        /// <summary>
        /// Constructor for ObjectData class.
        /// </summary>
        /// <param name="name">
        /// Name of the object
        /// </param>
        /// <param name="components">
        /// Object's components
        /// </param>
        /// <param name="foundObject">
        /// Object whose data belongs to
        /// </param>
        protected ObjectData(string name, Component[] components, Object foundObject)
        {
            this.Name = name;
            this.Components = components;
            this.FoundObject = foundObject;
        }

        /// <summary>
        /// Static method that transforms an Object into its respective
        /// ObjectData.
        /// </summary>
        /// <param name="objectToTransformIntoData">
        /// Object to transform into ObjectData
        /// </param>
        /// <returns>
        /// ObjectData version of the given object, null if the 
        /// given value is also null
        /// </returns>
        public static ObjectData TransformIntoObjectData(Object objectToTransformIntoData)
        {
            if (objectToTransformIntoData == null)
            {
                return null;
            }
            List<Component> filteredComponents = new List<Component>();
            if (objectToTransformIntoData is GameObject gameObject)
            {
                Component[] components = gameObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        continue;
                    }
                    filteredComponents.Add(component);
                }
            }
            return new ObjectData(objectToTransformIntoData.name, filteredComponents.ToArray(), objectToTransformIntoData);
        }
    }
}
