using System.Collections.Generic;
using Object = UnityEngine.Object;
using GameObject = UnityEngine.GameObject;
using ScriptableObject = UnityEngine.ScriptableObject;
using Component = UnityEngine.Component;
using UnityEngine.SceneManagement;
using ReferenceFinder.Enums;
using ReferenceFinder.Extensions;
using ReferenceFinder.Model;
using ReferenceFinder.Utils;
using UnityEditor.SceneManagement;

namespace ReferenceFinder.Editor
{
    /// <summary>
    /// Reference finder's editor window presenter.
    /// </summary>
    public class ReferenceFinderPresenter
    {
        private ReferenceFinderView _view;
        private LogLevel _logLevel;
        private SearchType _currentSearchType;
        private ObjectData _objectData;

        /// <summary>
        /// Constructor for the editor window presenter.
        /// </summary>
        /// <param name="view">
        /// Editor window view
        /// </param>
        public ReferenceFinderPresenter(ReferenceFinderView view)
        {
            _view = view;
            _currentSearchType = SearchType.AutoGameObject;
            _logLevel = LogLevel.ErrorOnly;
        }

        /// <summary>
        /// Creates the GUI for the editor window.
        /// </summary>
        public void CreateGUI()
        {
            _view.CreateToolbar();
            _view.CreateSearchTypeField();
            _view.CreateObjectField();
            _view.CreateFindReferencesElements("No object is currently chosen");
        }

        /// <summary>
        /// Clears the lists.
        /// </summary>
        public void Clear()
        {
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.ReferencesLists);
        }

        /// <summary>
        /// Refreshes UI.
        /// </summary>
        public void Refresh()
        {
            _view.ClearGUI();
            Reset();
            CreateGUI();
        }

        /// <summary>
        /// Updates log level value.
        /// </summary>
        /// <param name="newValue">
        /// Newly selected log level
        /// </param>
        public void ChangeLogLevel(LogLevel newValue)
        {
            _logLevel = newValue;
        }

        /// <summary>
        /// Updates search type value and resets fields.
        /// </summary>
        /// <param name="newValue">
        /// Newly selected search type
        /// </param>
        public void ChangeSearchType(SearchType newValue)
        {
            _currentSearchType = newValue;
            ResetFields();
        }

        /// <summary>
        /// Updates object data value and validates the data.
        /// </summary>
        /// <param name="newValue">
        /// Newly selected Object
        /// </param>
        public void ChangeObject(Object newValue)
        {
            _objectData = ObjectData.TransformIntoObjectData(newValue);
            Validate();
        }

        /// <summary>
        /// Starts finding references of the objects from the active scenes.
        /// <para/>
        /// Previous reference lists are also cleared.
        /// </summary>
        public void FindReferences()
        {
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.ReferencesLists);
            Dictionary<string, Object[]> objectsPerSceneDictionary = new Dictionary<string, Object[]>();

            GetReferencesInPrefabStage(objectsPerSceneDictionary);
            GetReferencesPerScene(objectsPerSceneDictionary, GetGameObjectsPerScene());
            GetReferencesInAssets(objectsPerSceneDictionary);
            if (objectsPerSceneDictionary.Count == 0)
            {
                return;
            }
            _view.ShowReferencesPerScene(objectsPerSceneDictionary);
        }

        /// <summary>
        /// Get objects where object is referenced on the stage and save them
        /// in a dictionary.
        /// </summary>
        /// <param name="objectsWithReferencesPerLocationDictionary">
        /// Dictionary where the results will be saved
        /// </param>
        private void GetReferencesInPrefabStage(Dictionary<string, Object[]> objectsWithReferencesPerLocationDictionary)
        {
            Stage currentStage = StageUtility.GetCurrentStage();
            // Prefab preview has the name as empty
            if (currentStage.name != string.Empty)
            {
                return;
            }
            Object[] objectsWithReferences = GetReferences(currentStage.FindGameObjectsInStage());
            objectsWithReferencesPerLocationDictionary.Add("Prefab preview stage", objectsWithReferences);
        }

        /// <summary>
        /// Get objects where object is referenced per scene and save them
        /// in a dictionary.
        /// </summary>
        /// <param name="objectsWithReferencesPerLocationDictionary">
        /// Dictionary where the results will be saved
        /// </param>
        /// <param name="objectsPerSceneDictionary">
        /// Dictionary with the objects with references per scene
        /// </param>
        private void GetReferencesPerScene(Dictionary<string, Object[]> objectsWithReferencesPerLocationDictionary, Dictionary<Scene, GameObject[]> objectsPerSceneDictionary)
        {
            foreach (Scene scene in objectsPerSceneDictionary.Keys)
            {
                Object[] objectsWithReferences = GetReferences(objectsPerSceneDictionary[scene]);
                objectsWithReferencesPerLocationDictionary.Add(scene.name, objectsWithReferences);
            }
        }

        /// <summary>
        /// Gets objects with references from assets.
        /// </summary>
        /// <param name="objectsWithReferencesPerLocationDictionary">
        /// Dictionary where the results will be saved
        /// </param>
        private void GetReferencesInAssets(Dictionary<string, Object[]> objectsWithReferencesPerLocationDictionary)
        {
            GetReferencesInPrefabs(objectsWithReferencesPerLocationDictionary);
            GetReferencesInScriptableObjects(objectsWithReferencesPerLocationDictionary);
        }

        /// <summary>
        /// Gets references in prefabs.
        /// </summary>
        /// <param name="objectsWithReferencesPerLocationDictionary">
        /// Dictionary where the results will be saved
        /// </param>
        private void GetReferencesInPrefabs(Dictionary<string, Object[]> objectsWithReferencesPerLocationDictionary)
        {
            string locationName = "Assets (Prefabs)";
            Object[] objectsWithReferences = GetReferences(EditorUtils.GetAllPrefabsFromProject());
            objectsWithReferencesPerLocationDictionary.Add(locationName, objectsWithReferences);
        }

        /// <summary>
        /// Gets references in ScriptableObjects.
        /// </summary>
        /// <param name="objectsWithReferencesPerLocationDictionary">
        /// Dictionary where the results will be saved
        /// </param>
        private void GetReferencesInScriptableObjects(Dictionary<string, Object[]> objectsWithReferencesPerLocationDictionary)
        {
            string locationName = "Assets (ScriptableObjects)";
            Object[] objectsWithReferences = GetReferences(EditorUtils.GetAllScriptableObjectsFromProject());
            objectsWithReferencesPerLocationDictionary.Add(locationName, objectsWithReferences);
        }

        /// <summary>
        /// Get GameObjects where object is referenced.
        /// </summary>
        /// <param name="objects">
        /// Objects list to find references on
        /// </param>
        /// <returns>
        /// Array with the GameObjects where the object is referenced
        /// </returns>
        private Object[] GetReferences(Object[] objects)
        {
            List<Object> objectsWithReferences = new List<Object>();
            foreach (Object objectToFindReference in objects)
            {
                if (objectToFindReference is GameObject gameObject)
                {
                    if (gameObject.HasReferenceTo(_objectData.FoundObject))
                    {
                        objectsWithReferences.Add(objectToFindReference);
                        continue;
                    }
                }
                else if (objectToFindReference is ScriptableObject scriptableObject)
                {
                    if (scriptableObject.HasReferenceTo(_objectData.FoundObject))
                    {
                        objectsWithReferences.Add(objectToFindReference);
                        continue;
                    }
                }
                foreach (Component component in _objectData.Components)
                {
                    if (objectToFindReference is GameObject gameObject2)
                    {
                        if (gameObject2.HasReferenceTo(component))
                        {
                            objectsWithReferences.Add(objectToFindReference);
                            break;
                        }
                    }
                    else if (objectToFindReference is ScriptableObject scriptableObject2)
                    {
                        if (scriptableObject2.HasReferenceTo(_objectData.FoundObject))
                        {
                            objectsWithReferences.Add(objectToFindReference);
                            break;
                        }
                    }
                }
            }
            return objectsWithReferences.ToArray();
        }

        /// <summary>
        /// Gets all objects from every loaded scene.
        /// </summary>
        /// <returns>
        /// Dictionary with objects per scene entry
        /// </returns>
        private Dictionary<Scene, GameObject[]> GetGameObjectsPerScene()
        {
            Scene[] activeScenes = EditorUtils.GetAllActiveScenes();
            Dictionary<Scene, GameObject[]> objectsPerSceneDictionary = new Dictionary<Scene, GameObject[]>();
            foreach (Scene scene in activeScenes)
            {
                GameObject[] sceneObjects = scene.FindGameObjectsInScene();
                objectsPerSceneDictionary.Add(scene, sceneObjects);
            }
            return objectsPerSceneDictionary;
        }

        /// <summary>
        /// Callback for when a scene is saved, clearing the UI in case
        /// the object has changed name.
        /// </summary>
        /// <param name="scene">
        /// Scene being saved
        /// </param>
        public void OnSceneSaved(Scene scene)
        {
            if (CheckHierarchyCheckRequirements()
                && (!scene.ContainsGameObject((GameObject)_objectData.FoundObject, out GameObject foundObject)
                || foundObject.name != _objectData.Name))
            {
                ResetFields();
            }
        }

        /// <summary>
        /// Callback for when a scene is closed, clearing the UI in case
        /// the object was in the scene.
        /// </summary>
        /// <param name="scene">
        /// Scene closed
        /// </param>
        public void OnSceneClosing(Scene scene)
        {
            if (CheckHierarchyCheckRequirements()
                && scene.ContainsGameObject((GameObject)_objectData.FoundObject, out GameObject _))
            {
                ResetFields();
            }
        }

        /// <summary>
        /// Callback for when something on the hierarchy is changed,
        /// clearing the UI in case the object is no longer found within
        /// the available scenes.
        /// </summary>
        public void OnHierarchyChange()
        {
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.ReferencesLists);
            Scene[] scenes = EditorUtils.GetAllActiveScenes();
            if (CheckHierarchyCheckRequirements())
            {
                foreach (Scene scene in scenes)
                {
                    if (scene.ContainsGameObject((GameObject)_objectData.FoundObject, out GameObject _))
                    {
                        return;
                    }
                }
                ResetFields();
            }
        }

        /// <summary>
        /// Resets object data and rebuilds the UI.
        /// </summary>
        private void ResetFields()
        {
            _objectData = null;
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.ReferencesLists);
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.FindReferences);
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.SearchParameters);
            _view.CreateObjectField(_currentSearchType);
            _view.CreateFindReferencesElements();
        }

        /// <summary>
        /// Helper method to check for the hierarchy/scene check requirements.
        /// <para/>
        /// (The requirements are to assess whether the scene no longer has a
        /// reference to the game object in the object data)
        /// </summary>
        /// <returns>
        /// True if all requirements are met, false otherwise
        /// </returns>
        private bool CheckHierarchyCheckRequirements()
        {
            return _currentSearchType == SearchType.AutoGameObject
                && _objectData != null
                && _objectData.FoundObject is GameObject gameObject
                && !gameObject.IsPrefabAsset();
        }

        /// <summary>
        /// Validates object data and updates "find references" button
        /// field with new tooltip and state.
        /// <para/>
        /// Also clears the lists from a previous search.
        /// </summary>
        private void Validate()
        {
            string tooltip = string.Empty;
            bool buttonState = false;
            if (_objectData == null)
            {
                tooltip = "No object is currently chosen";
                buttonState = false;
            }
            else
            {
                buttonState = true;
            }
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.ReferencesLists);
            _view.ClearRootAtIndex((int)ReferenceFinderUIOrder.FindReferences);
            _view.CreateFindReferencesElements(tooltip, buttonState);
        }

        /// <summary>
        /// Resets presenter's contents (besides the view).
        /// </summary>
        public void Reset()
        {
            _objectData = null;
            _currentSearchType = SearchType.AutoGameObject;
        }

        /// <summary>
        /// Get the current log level.
        /// </summary>
        /// <returns>
        /// Current log level value
        /// </returns>
        public LogLevel GetCurrentLogLevel()
        {
            return _logLevel;
        }
    }
}
