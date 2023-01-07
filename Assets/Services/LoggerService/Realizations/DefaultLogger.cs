using System;

namespace Services.LoggerService
{
	public static class DefaultLogger
	{
		private static ILogger logger;

		public static void Initialize(ILogger logger)
		{
			DefaultLogger.logger = logger;
		}

		public static void Log(string message)
		{
			logger.Log(message);
		}

		public static void Error(string message)
		{
			logger.Error(message);
		}

		public static void Error(Exception exception)
		{
			logger.Exception(exception);
		}
	}
}