using System;
using UnityEngine;

namespace Services.LoggerService
{
    public class DebugLogger : ILogger
    {
        public DebugLogger()
        {
            DefaultLogger.Initialize(this);
        }
        
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void Error(string message)
        {
            Debug.LogError(message);
        }

        public void Exception(Exception exception)
        {
           Debug.LogException(exception);
        }
    }
}