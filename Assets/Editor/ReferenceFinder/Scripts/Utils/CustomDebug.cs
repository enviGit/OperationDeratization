using UnityEngine;
using ReferenceFinder.Enums;

namespace ReferenceFinder.Utils
{
    /// <summary>
    /// Custom debug class to filter logs based on the log
    /// level selected.
    /// </summary>
    public static class CustomDebug
    {
        public static LogLevel s_CurrentLogLevel;

        /// <summary>
        /// Logs message with log type and level.
        /// </summary>
        /// <param name="message">
        /// Message to log
        /// </param>
        /// <param name="logType">
        /// Log type (Log, Warning, Error)
        /// </param>
        /// <param name="allowedLevel">
        /// Allowed log level, supresses logs with lower values
        /// </param>
        public static void Log(string message, LogType logType, LogLevel allowedLevel)
        {
            switch (logType)
            {
                case LogType.Warning:
                    CustomDebug.LogWarning(message, allowedLevel);
                    break;
                case LogType.Error:
                    CustomDebug.LogError(message, allowedLevel);
                    break;
                case LogType.Log:
                default:
                    CustomDebug.Log(message, allowedLevel);
                    break;
            }
        }

        /// <summary>
        /// Logs a normal message if message is within allowed log level.
        /// </summary>
        /// <param name="message">
        /// Message to be shown
        /// </param>
        /// <param name="logLevel">
        /// Current log level
        /// </param>
        private static void Log(string message, LogLevel logLevel)
        {
            if (CheckForLogLevel(LogLevel.All, logLevel))
            {
                Debug.Log(message);
            }

        }

        /// <summary>
        /// Logs a warning message if message is within allowed log level.
        /// </summary>
        /// <param name="message">
        /// Message to be shown
        /// </param>
        /// <param name="logLevel">
        /// Current log level
        /// </param>
        private static void LogWarning(string message, LogLevel logLevel)
        {
            if (CheckForLogLevel(LogLevel.WarningAndError, logLevel))
            {
                Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Logs an error message if message is within allowed log level.
        /// </summary>
        /// <param name="message">
        /// Message to be shown
        /// </param>
        /// <param name="logLevel">
        /// Current log level
        /// </param>
        private static void LogError(string message, LogLevel logLevel)
        {
            if (CheckForLogLevel(LogLevel.ErrorOnly, logLevel))
            {
                Debug.LogError(message);
            }
        }

        /// <summary>
        /// Helper method that checks if a log level is within the allowed value.
        /// </summary>
        /// <param name="allowedLevel">
        /// Allowed level
        /// </param>
        /// <param name="logLevel">
        /// Level to be checked
        /// </param>
        /// <returns>
        /// True if within allowed log level, false otherwise
        /// </returns>
        private static bool CheckForLogLevel(LogLevel allowedLevel, LogLevel logLevel)
        {
            return (int)logLevel <= (int)allowedLevel;
        }
    }
}
