using System;
using System.IO;
using System.Data.OleDb;
using System.Windows.Forms;

namespace EnglishAutomationApp.Helpers
{
    public static class AccessDatabaseHelper
    {
        private static string GetAccessDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "EnglishAutomationApp", "database.accdb");
        }

        private static string GetSQLiteDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                               "EnglishAutomationApp", "database.db");
        }

        public static string GetConnectionString()
        {
            // For this implementation, we'll use SQLite as it's more compatible with Entity Framework Core
            // But we'll structure it to work like Access (file-based database)
            var dbPath = GetSQLiteDatabasePath();
            
            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            return $"Data Source={dbPath}";
        }

        public static bool TestConnection()
        {
            try
            {
                var connectionString = GetConnectionString();
                // Test SQLite connection
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
                connection.Open();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "Database connection test failed");
                return false;
            }
        }

        public static void CreateAccessDatabaseIfNotExists()
        {
            try
            {
                var accessDbPath = GetAccessDatabasePath();
                var directory = Path.GetDirectoryName(accessDbPath);
                
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                // Note: Creating an actual Access database requires Access to be installed
                // For this student project, we'll use SQLite which provides similar functionality
                // but is more portable and doesn't require Access installation

                if (!File.Exists(GetSQLiteDatabasePath()))
                {
                    // Create empty SQLite database file
                    using var connection = new Microsoft.Data.Sqlite.SqliteConnection(GetConnectionString());
                    connection.Open();
                    connection.Close();
                    
                    MessageBox.Show($"Database created successfully at:\n{GetSQLiteDatabasePath()}", 
                        "Database Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Failed to create database");
            }
        }

        public static void BackupDatabase()
        {
            try
            {
                var sourceDb = GetSQLiteDatabasePath();
                if (!File.Exists(sourceDb))
                {
                    MessageBox.Show("Database file not found!", "Backup Failed", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var backupPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"EnglishAutomation_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");

                File.Copy(sourceDb, backupPath);
                
                MessageBox.Show($"Database backup created successfully:\n{backupPath}", 
                    "Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Failed to backup database");
            }
        }

        public static void RestoreDatabase(string backupFilePath)
        {
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    MessageBox.Show("Backup file not found!", "Restore Failed", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var targetDb = GetSQLiteDatabasePath();
                
                // Create backup of current database before restore
                if (File.Exists(targetDb))
                {
                    var currentBackup = Path.Combine(
                        Path.GetDirectoryName(targetDb)!,
                        $"database_before_restore_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                    File.Copy(targetDb, currentBackup);
                }

                File.Copy(backupFilePath, targetDb, true);
                
                MessageBox.Show("Database restored successfully!", "Restore Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Failed to restore database");
            }
        }

        public static long GetDatabaseSize()
        {
            try
            {
                var dbPath = GetSQLiteDatabasePath();
                if (File.Exists(dbPath))
                {
                    var fileInfo = new FileInfo(dbPath);
                    return fileInfo.Length;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static string GetDatabaseSizeFormatted()
        {
            var sizeInBytes = GetDatabaseSize();
            if (sizeInBytes == 0) return "0 KB";

            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = sizeInBytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }

        public static void CompactDatabase()
        {
            try
            {
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection(GetConnectionString());
                connection.Open();
                
                using var command = connection.CreateCommand();
                command.CommandText = "VACUUM;";
                command.ExecuteNonQuery();
                
                MessageBox.Show("Database compacted successfully!", "Compact Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex, "Failed to compact database");
            }
        }

        public static bool DatabaseExists()
        {
            return File.Exists(GetSQLiteDatabasePath());
        }

        public static string GetDatabaseInfo()
        {
            var dbPath = GetSQLiteDatabasePath();
            if (!File.Exists(dbPath))
                return "Database not found";

            var fileInfo = new FileInfo(dbPath);
            return $"Database: {Path.GetFileName(dbPath)}\n" +
                   $"Location: {dbPath}\n" +
                   $"Size: {GetDatabaseSizeFormatted()}\n" +
                   $"Created: {fileInfo.CreationTime:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}";
        }
    }
}
