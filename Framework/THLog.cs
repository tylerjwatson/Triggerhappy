using System;

namespace TriggerHappy {

	public enum LogLevel {
		Debug,
		Info,
		Warning,
		Error
	}

	public static class THLog {
		static readonly object __consoleLock = new object();

		static ConsoleColor GetColour(LogLevel level) {
			switch (level) {
			case LogLevel.Debug:
				return ConsoleColor.Gray;
			case LogLevel.Error:
				return ConsoleColor.Red;
			case LogLevel.Warning:
				return ConsoleColor.Yellow;
			case LogLevel.Info:
			default:
				return ConsoleColor.Gray;
			}
		}

		public static void Log(LogLevel Level, string MessageFormat, params object[] args) {
			ConsoleColor oldConsoleColour = Console.ForegroundColor;
			lock (__consoleLock) {
				Console.ForegroundColor = GetColour(Level);
				Console.WriteLine(" <TH> {0}: {1}", Level.ToString(), string.Format(MessageFormat, args));
				Console.ForegroundColor = oldConsoleColour;
			}
		}

	}
}

