using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Views;
using Microsoft.EntityFrameworkCore;

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
                // Create Access-like database if it doesn't exist
                AccessDatabaseHelper.CreateAccessDatabaseIfNotExists();

                // Test database connection
                if (!AccessDatabaseHelper.TestConnection())
                {
                    MessageBox.Show("Failed to connect to database!", "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var context = new AppDbContext();
                await context.Database.EnsureCreatedAsync();

                // Seed initial data if needed
                await SeedInitialData(context);

                // Log database info
                ErrorHandler.LogInfo($"Database initialized successfully. {AccessDatabaseHelper.GetDatabaseInfo()}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                ErrorHandler.LogError(ex, "Database initialization failed");
            }
        }

        private static async Task SeedInitialData(AppDbContext context)
        {
            // Check if admin user exists
            var adminExists = await context.Users.AnyAsync(u => u.Role == "Admin");
            if (!adminExists)
            {
                var adminUser = new User
                {
                    Email = "admin@englishautomation.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Admin",
                    LastName = "User",
                    Role = "Admin",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
