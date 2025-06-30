using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Data
{
    public static class AppDbContext
    {
        private static string GetConnectionString()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    "EnglishAutomationApp", "database.accdb");

            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }

        public static async Task InitializeDatabaseAsync()
        {
            await AccessDatabaseHelper.InitializeDatabaseAsync();
        }
    }
}
