using UnityEngine;

namespace EdenMeng.AssetManager
{
    internal class AssetLogger
    {
        public static bool IsEnabled = true;

        public static void Log(string message)
        {
            if (!IsEnabled)
                return;
            Debug.Log(message);
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (!IsEnabled)
                return;
            Debug.LogFormat(format, args);
        }

        public static void LogWarning(string message)
        {
            if (!IsEnabled)
                return;
            Debug.LogWarning(message);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            if (!IsEnabled)
                return;
            Debug.LogWarningFormat(format, args);
        }

        public static void LogError(string message)
        {
            if (!IsEnabled)
                return;
            Debug.LogError(message);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            if (!IsEnabled)
                return;
            Debug.LogErrorFormat(format, args);
        }
    }
}