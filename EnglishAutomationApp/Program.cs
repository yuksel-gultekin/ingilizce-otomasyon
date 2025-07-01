using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Views;

namespace EnglishAutomationApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable visual styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize error handling
            ErrorHandler.Initialize();

            // Initialize database
            InitializeDatabase().Wait();

            // Start the application with login form
            Application.Run(new LoginForm());
        }

        private static async Task InitializeDatabase()
        {
            try
            {
                // Initialize Access database
                await Data.AccessDatabaseHelper.InitializeDatabaseAsync();

                ErrorHandler.LogInfo("Access database initialized successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}\n\nPlease ensure Microsoft Access Database Engine is installed.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorHandler.LogError(ex, "Database initialization failed");
            }
        }


    }
}
