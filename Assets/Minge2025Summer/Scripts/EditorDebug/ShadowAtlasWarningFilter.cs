using System;
using UnityEngine;

namespace Minge2025Summer.Scripts.EditorDebug
{
    public sealed class ShadowAtlasWarningFilter : ILogHandler
    {
        private static ILogHandler handler;

        private static readonly string[] Phrases =
        {
            "Reduced additional punctual light shadows resolution by"
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (handler != null)
                return;

            handler = Debug.unityLogger.logHandler;
            Debug.unityLogger.logHandler = new ShadowAtlasWarningFilter();
        }
        
        # if UNITY_EDITOR
            [UnityEditor.InitializeOnLoadMethod]
            private static void InstallInEditor() => Install();
        # endif

        /// <summary>
        /// </summary>
        /// <param name="logType"></param>
        /// <param name="context"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string message;
            try
            {
                message = (args != null && args.Length > 0) ? string.Format(format, args) : format;
            }
            catch
            {
                message = format;
            }

            if (ShouldFilterType(logType) && ShouldFilter(message))
                return;
            
            handler?.LogFormat(logType, context, format, args);
        }
        
        public void LogException(Exception exception, UnityEngine.Object context) => handler?.LogException(exception, context);
        
        private static bool ShouldFilterType(LogType logType) => logType == LogType.Warning || logType == LogType.Log;

        /// <summary>
        /// フィルタするかどうか
        /// </summary>
        /// <param name="message">フィルタするメッセージ</param>
        /// <returns>フィルタするかどうか</returns>
        private static bool ShouldFilter(string message)
        {
            foreach(var phrase in Phrases)
                if (message.IndexOf(phrase, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;

            return false;
        }
    }
}