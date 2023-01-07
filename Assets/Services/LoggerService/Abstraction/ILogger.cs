using System;

namespace Services.LoggerService
{
    public interface ILogger
    {
        void Log(string message);
        void Error(string message);
        void Exception(Exception exception);
    }
}