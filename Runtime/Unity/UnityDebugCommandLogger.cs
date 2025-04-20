using UnityEngine;

namespace Ametrin.Command.Unity
{
    public sealed class UnityDebugCommandLogger : ICommandLogger
    {
        public void Log(string message) => Debug.Log(message);
        public void LogWarning(string message) => Debug.LogWarning(message);
        public void LogError(string message) => Debug.LogError(message);
    }
}