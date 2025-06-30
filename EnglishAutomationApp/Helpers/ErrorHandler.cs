using System;
using System.IO;
using System.Windows.Forms;

namespace EnglishAutomationApp.Helpers
{
    public static class ErrorHandler
    {
        private static string LogFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EnglishAutomationApp", "Logs", "error.log");

        public static void Initialize()
        {
            // Create logs directory if it doesn't exist
            var logDirectory = Path.GetDirectoryName(LogFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory!);
            }

            // Set up global exception handling
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            LogError(e.Exception);
            ShowErrorMessage("An unexpected error occurred. Please check the log file for details.", e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogError(ex);
                ShowErrorMessage("A critical error occurred. The application will now close.", ex);
            }
        }

        public static void LogError(Exception exception, string? additionalInfo = null)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {exception.Message}\n" +
                              $"Stack Trace: {exception.StackTrace}\n";

                if (!string.IsNullOrEmpty(additionalInfo))
                {
                    logEntry += $"Additional Info: {additionalInfo}\n";
                }

                logEntry += new string('-', 80) + "\n";

                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // If we can't log the error, there's not much we can do
            }
        }

        public static void LogInfo(string message)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO: {message}\n";
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // If we can't log the info, continue silently
            }
        }

        private static void ShowErrorMessage(string message, Exception? exception = null)
        {
            var fullMessage = message;
            if (exception != null)
            {
                fullMessage += $"\n\nError Details: {exception.Message}";
            }

            MessageBox.Show(fullMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void HandleError(Exception exception, string userMessage = "An error occurred.")
        {
            LogError(exception);
            ShowErrorMessage(userMessage, exception);
        }
    }
}
