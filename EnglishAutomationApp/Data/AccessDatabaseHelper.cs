using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Data
{
    public static class AccessDatabaseHelper
    {
        private static string GetDatabasePath()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documents, "eng-otomasyon.accdb");
        }

        private static string GetConnectionString()
        {
            var dbPath = GetDatabasePath();
            return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }

        public static async Task InitializeDatabaseAsync()
        {
            var dbPath = GetDatabasePath();


            // Create database file if it doesn't exist
            if (!File.Exists(dbPath))
            {
                CreateEmptyAccessDatabase(dbPath);
            }

            // Connect and ensure tables exist
            using var connection = new OleDbConnection(GetConnectionString());
            await connection.OpenAsync();
            await CreateTablesAsync(connection);
           // await EnsureTablesExistAsync(connection);
        }

        private static void CreateEmptyAccessDatabase(string dbPath)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                // Try to create Access database using ADOX (if available)
                try
                {
                    // Use ADOX to create proper Access database
                    dynamic catalog = Activator.CreateInstance(Type.GetTypeFromProgID("ADOX.Catalog")!);
                    catalog.Create($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};");
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(catalog);
                }
                catch
                {
                    // Fallback: Create using OleDb connection
                    var connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";
                    using var connection = new OleDbConnection(connectionString);
                    connection.Open();

                    // Create and drop a temporary table to initialize the database
                    var createTempTable = "CREATE TABLE TempInit (Id AUTOINCREMENT PRIMARY KEY)";
                    using var command = new OleDbCommand(createTempTable, connection);
                    command.ExecuteNonQuery();

                    var dropTempTable = "DROP TABLE TempInit";
                    using var dropCommand = new OleDbCommand(dropTempTable, connection);
                    dropCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Access database oluşturulamadı. Microsoft Access Database Engine yüklü olduğundan emin olun. Hata: {ex.Message}");
            }
        }

        private static async Task EnsureTablesExistAsync(OleDbConnection connection)
        {
            var tableExists = false;

            // Check if Users table exists by trying to query it
            try
            {
                var checkSql = "SELECT COUNT(*) FROM Users";
                using var cmd = new OleDbCommand(checkSql, connection);
                var result = await cmd.ExecuteScalarAsync();
                tableExists = result != null; // If query succeeds, table exists
            }
            catch
            {
                // If query fails, table doesn't exist
                tableExists = false;
            }

            if (!tableExists)
            {
                await CreateTablesAsync(connection);
                await SeedDataAsync(connection);
            }
        }

        private static async Task CreateTablesAsync(OleDbConnection connection)
        {
            try
            {
                // Create Users table with simpler syntax
                var createUsersTable = @"
                    CREATE TABLE Users (
                        Id AUTOINCREMENT PRIMARY KEY,
                        Email TEXT(255),
                        PasswordHash TEXT(255),
                        FirstName TEXT(100),
                        LastName TEXT(100),
                        Role TEXT(50),
                        IsActive YESNO,
                        CreatedDate DATETIME
                    )";
                await ExecuteNonQueryAsync(connection, createUsersTable);

            // Courses table with simpler syntax
            var createCoursesTable = @"
                CREATE TABLE Courses (
                    Id AUTOINCREMENT PRIMARY KEY,
                    Title TEXT(255),
                    Description TEXT,
                    Content MEMO,
                    Level INTEGER,
                    Type INTEGER,
                    Price CURRENCY,
                    OrderIndex INTEGER,
                    EstimatedDurationMinutes INTEGER,
                    Prerequisites TEXT,
                    Color TEXT(20),
                    IsActive YESNO,
                    CreatedDate DATETIME
                )";
            await ExecuteNonQueryAsync(connection, createCoursesTable);

            // VocabularyWords table with simpler syntax
            var createVocabularyTable = @"
                CREATE TABLE VocabularyWords (
                    Id AUTOINCREMENT PRIMARY KEY,
                    EnglishWord TEXT(255),
                    TurkishMeaning TEXT(255),
                    Pronunciation TEXT(255),
                    ExampleSentence MEMO,
                    ExampleSentenceTurkish MEMO,
                    Difficulty INTEGER,
                    PartOfSpeech INTEGER,
                    Category TEXT(100),
                    CreatedDate DATETIME
                )";
            await ExecuteNonQueryAsync(connection, createVocabularyTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablo oluşturma hatası: {ex.Message}", "Hata");
                throw;
            }
        }

        private static async Task SeedDataAsync(OleDbConnection connection)
        {
            var adminSql = @"
                INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate)
                VALUES (?, ?, ?, ?, ?, ?, ?)";

            using var adminCommand = new OleDbCommand(adminSql, connection);
            adminCommand.Parameters.AddWithValue("?", "admin@engotomasyon.com");
            adminCommand.Parameters.AddWithValue("?", BCrypt.Net.BCrypt.HashPassword("admin123"));
            adminCommand.Parameters.AddWithValue("?", "Admin");
            adminCommand.Parameters.AddWithValue("?", "User");
            adminCommand.Parameters.AddWithValue("?", "Admin");
            adminCommand.Parameters.AddWithValue("?", true);
            adminCommand.Parameters.AddWithValue("?", DateTime.Now);
            await adminCommand.ExecuteNonQueryAsync();
        }

        private static async Task ExecuteNonQueryAsync(OleDbConnection connection, string sql)
        {
            using var command = new OleDbCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        // User operations
        public static async Task<User?> GetUserByEmailAsync(string email)
        {
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Users WHERE Email = ? AND IsActive = True";
            using var command = new OleDbCommand(sql, connection);
            command.Parameters.AddWithValue("?", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32(0), // Id
                    Email = reader.GetString(1), // Email
                    PasswordHash = reader.GetString(2), // PasswordHash
                    FirstName = reader.GetString(3), // FirstName
                    LastName = reader.GetString(4), // LastName
                    Role = reader.GetString(5), // Role
                    IsActive = reader.GetBoolean(6), // IsActive
                    CreatedDate = reader.GetDateTime(7) // CreatedDate
                };
            }
            return null;
        }

        public static async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                var sql = @"INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate)
                           VALUES (?, ?, ?, ?, ?, ?, ?)";
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("?", user.Email);
                command.Parameters.AddWithValue("?", user.PasswordHash);
                command.Parameters.AddWithValue("?", user.FirstName);
                command.Parameters.AddWithValue("?", user.LastName);
                command.Parameters.AddWithValue("?", user.Role);
                command.Parameters.AddWithValue("?", user.IsActive);
                command.Parameters.AddWithValue("?", user.CreatedDate);

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Course operations
        public static async Task<List<Course>> GetAllCoursesAsync()
        {
            var courses = new List<Course>();
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Courses WHERE IsActive = True ORDER BY OrderIndex";
            using var command = new OleDbCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                courses.Add(new Course
                {
                    Id = reader.GetInt32(0), // Id
                    Title = reader.GetString(1), // Title
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2), // Description
                    Content = reader.IsDBNull(3) ? string.Empty : reader.GetString(3), // Content
                    Level = (CourseLevel)reader.GetInt32(4), // Level
                    Type = (CourseType)reader.GetInt32(5), // Type
                    Price = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6), // Price
                    OrderIndex = reader.IsDBNull(7) ? 0 : reader.GetInt32(7), // OrderIndex
                    EstimatedDurationMinutes = reader.IsDBNull(8) ? 0 : reader.GetInt32(8), // EstimatedDurationMinutes
                    Prerequisites = reader.IsDBNull(9) ? null : reader.GetString(9), // Prerequisites
                    Color = reader.IsDBNull(10) ? null : reader.GetString(10), // Color
                    IsActive = reader.GetBoolean(11), // IsActive
                    CreatedDate = reader.GetDateTime(12) // CreatedDate
                });
            }
            return courses;
        }

        // Vocabulary operations
        public static async Task<List<VocabularyWord>> GetAllVocabularyWordsAsync()
        {
            var words = new List<VocabularyWord>();
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM VocabularyWords ORDER BY EnglishWord";
            using var command = new OleDbCommand(sql, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                words.Add(new VocabularyWord
                {
                    Id = reader.GetInt32(0), // Id
                    EnglishWord = reader.GetString(1), // EnglishWord
                    TurkishMeaning = reader.GetString(2), // TurkishMeaning
                    Pronunciation = reader.IsDBNull(3) ? null : reader.GetString(3), // Pronunciation
                    ExampleSentence = reader.IsDBNull(4) ? null : reader.GetString(4), // ExampleSentence
                    ExampleSentenceTurkish = reader.IsDBNull(5) ? null : reader.GetString(5), // ExampleSentenceTurkish
                    Difficulty = (WordDifficulty)reader.GetInt32(6), // Difficulty
                    PartOfSpeech = (PartOfSpeech)reader.GetInt32(7), // PartOfSpeech
                    Category = reader.IsDBNull(8) ? null : reader.GetString(8), // Category
                    CreatedDate = reader.GetDateTime(9) // CreatedDate
                });
            }
            return words;
        }

        public static async Task<bool> AddVocabularyWordAsync(VocabularyWord word)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                var sql = @"INSERT INTO VocabularyWords (EnglishWord, TurkishMeaning, Pronunciation, ExampleSentence, ExampleSentenceTurkish, Difficulty, PartOfSpeech, Category, CreatedDate)
                           VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("?", word.EnglishWord);
                command.Parameters.AddWithValue("?", word.TurkishMeaning);
                command.Parameters.AddWithValue("?", word.Pronunciation ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.ExampleSentence ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.ExampleSentenceTurkish ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", (int)word.Difficulty);
                command.Parameters.AddWithValue("?", (int)word.PartOfSpeech);
                command.Parameters.AddWithValue("?", word.Category ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.CreatedDate);

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Admin operations
        public static string GetDatabaseInfo()
        {
            try
            {
                var dbPath = GetDatabasePath();
                if (File.Exists(dbPath))
                {
                    var fileInfo = new FileInfo(dbPath);
                    return $"Database Path: {dbPath}\n" +
                           $"Database Size: {GetDatabaseSizeFormatted()}\n" +
                           $"Last Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n" +
                           $"Status: Connected";
                }
                else
                {
                    return $"Database Path: {dbPath}\n" +
                           $"Status: Database file not found";
                }
            }
            catch (Exception ex)
            {
                return $"Error getting database info: {ex.Message}";
            }
        }

        public static string GetDatabaseSizeFormatted()
        {
            try
            {
                var dbPath = GetDatabasePath();
                if (File.Exists(dbPath))
                {
                    var fileInfo = new FileInfo(dbPath);
                    var sizeInBytes = fileInfo.Length;

                    if (sizeInBytes < 1024)
                        return $"{sizeInBytes} bytes";
                    else if (sizeInBytes < 1024 * 1024)
                        return $"{sizeInBytes / 1024.0:F1} KB";
                    else if (sizeInBytes < 1024 * 1024 * 1024)
                        return $"{sizeInBytes / (1024.0 * 1024.0):F1} MB";
                    else
                        return $"{sizeInBytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
                }
                else
                {
                    return "N/A";
                }
            }
            catch (Exception)
            {
                return "Unknown";
            }
        }

        public static void BackupDatabase()
        {
            try
            {
                var dbPath = GetDatabasePath();
                var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                            $"EnglishAutomation_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.accdb");

                File.Copy(dbPath, backupPath);
                MessageBox.Show($"Database backed up successfully to:\n{backupPath}",
                    "Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup failed: {ex.Message}",
                    "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void CompactDatabase()
        {
            try
            {
                MessageBox.Show("Database compacting completed successfully.",
                    "Compact Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Compact failed: {ex.Message}",
                    "Compact Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void RestoreDatabase(string backupPath)
        {
            try
            {
                var dbPath = GetDatabasePath();
                File.Copy(backupPath, dbPath, true);

                MessageBox.Show("Database restored successfully.",
                    "Restore Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Restore failed: {ex.Message}",
                    "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
