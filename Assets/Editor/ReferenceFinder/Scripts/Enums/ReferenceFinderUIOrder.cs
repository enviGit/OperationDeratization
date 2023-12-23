using System;

namespace ReferenceFinder.Enums
{
    /// <summary>
    /// Helper enum with the order of each part of the tool. 
    /// </summary>
    [Serializable]
    public enum ReferenceFinderUIOrder
    {
        Toolbar = 0,
        SearchType = 1,
        SearchParameters = 2,
        FindReferences = 3,
        ReferencesLists = 4,
    }
}
