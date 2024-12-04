using System;
using System.Collections.Generic;
using FakeItEasy;
using FakeItEasy.Configuration;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
namespace dotRMDY.TestingTools;

[PublicAPI]
	public static class LoggerExtensions
	{
		public static void VerifyLogErrorMustHaveHappened<T>(this ILogger<T> logger, Exception exception, string message)
		{
			try
			{
				logger.VerifyLog(LogLevel.Error, message, exception)
					.MustHaveHappenedOnceExactly();
			}
			catch (Exception e)
			{
				throw new ExpectationException($"while verifying a call to log error with message: \"{message}\"", e);
			}
		}

		public static void VerifyLogTraceMustHaveHappened<T>(this ILogger<T> logger, string message)
		{
			VerifyLogMustHaveHappened(logger, LogLevel.Trace, message);
		}

		public static void VerifyLogDebugMustHaveHappened<T>(this ILogger<T> logger, string message)
		{
			VerifyLogMustHaveHappened(logger, LogLevel.Debug, message);
		}

		public static void VerifyLogInformationMustHaveHappened<T>(this ILogger<T> logger, string message)
		{
			VerifyLogMustHaveHappened(logger, LogLevel.Information, message);
		}

		public static void VerifyLogWarningMustHaveHappened<T>(this ILogger<T> logger, string message)
		{
			VerifyLogMustHaveHappened(logger, LogLevel.Warning, message);
		}

		public static void VerifyLogErrorMustHaveHappened<T>(this ILogger<T> logger, string message)
		{
			VerifyLogMustHaveHappened(logger, LogLevel.Error, message);
		}

		public static void VerifyLogCriticalMustHaveHappened<T>(this ILogger<T> logger, string message)
		{
			VerifyLogMustHaveHappened(logger, LogLevel.Critical, message);
		}

		public static void VerifyLogMustNotHaveHappened<T>(this ILogger<T> logger)
		{
			A.CallTo(logger)
				.Where(call => call.Method.Name == nameof(ILogger.Log))
				.MustNotHaveHappened();
		}

		private static void VerifyLogMustHaveHappened<T>(this ILogger<T> logger, LogLevel logLevel, string message)
		{
			try
			{
				logger.VerifyLog(logLevel, message).MustHaveHappenedOnceExactly();
			}
			catch (Exception e)
			{
				throw new ExpectationException($"while verifying a call to log {logLevel} with message \"{message}\"", e);
			}
		}

		private static IVoidArgumentValidationConfiguration VerifyLog<T>(this ILogger<T> logger, LogLevel level, string message, Exception? exception = null)
		{
			return A.CallTo(logger)
				.Where(call => call.Method.Name == nameof(ILogger.Log)
				               && call.GetArgument<Exception>(3) == exception
				               && call.GetArgument<LogLevel>(0) == level
				               && call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString() == message);
		}
	}