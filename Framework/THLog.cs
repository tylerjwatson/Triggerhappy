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
        public static bool debugMode = false;

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

        public static void Debug(string MessageFormat, params object[] args) {
            Debug(GetColour(LogLevel.Debug), MessageFormat, args);
        }

        public static void Debug(ConsoleColor? colour = null, string MessageFormat = "", params object[] args) {
            if (debugMode == false) {
                return;
            }
            ConsoleColor oldConsoleColour = Console.ForegroundColor;
            lock (__consoleLock) {
                Console.ForegroundColor = colour ?? GetColour(LogLevel.Debug);
                Console.WriteLine(" <TH> {0}: {1}", "Debug", string.Format(MessageFormat, args));
                Console.ForegroundColor = oldConsoleColour;
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

