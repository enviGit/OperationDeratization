using System;

namespace ReferenceFinder.Enums
{
    /// <summary>
    /// Helper enum with all the allowed log levels.
    /// </summary>
    [Serializable]
    public enum LogLevel
    {
        All,
        WarningAndError,
        ErrorOnly,
        None
    }
}
