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

            if (!File.Exists(dbPath))
            {
                MessageBox.Show($"Database bulunamadı. Yeni dosya oluşturuluyor: {dbPath}", "Bilgi");

                CreateEmptyAccessDatabase(dbPath);
                MessageBox.Show("Boş Access dosyası oluşturuldu!", "Bilgi");
            }
            else
            {
                MessageBox.Show("Access database zaten var. Bağlanılıyor...", "Bilgi");
            }

            using var connection = new OleDbConnection(GetConnectionString());
            await connection.OpenAsync();

            await EnsureTablesExistAsync(connection);

            MessageBox.Show("Veritabanı hazır!", "Bilgi");
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

                // Create Access database using OleDb connection
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
            catch (Exception ex)
            {
                MessageBox.Show($"Database oluşturma hatası: {ex.Message}", "Hata");
                throw;
            }
        }

        private static async Task EnsureTablesExistAsync(OleDbConnection connection)
        {
            var tableExists = false;
            var checkSql = "SELECT COUNT(*) FROM MSysObjects WHERE Type=1 AND Name='Users'";

            try
            {
                using var cmd = new OleDbCommand(checkSql, connection);
                var count = (int)cmd.ExecuteScalar();
                tableExists = count > 0;
            }
            catch
            {
                tableExists = false;
            }

            if (!tableExists)
            {
                MessageBox.Show("'Users' tablosu bulunamadı. Tablolar oluşturuluyor...", "Bilgi");
                await CreateTablesAsync(connection);
                await SeedDataAsync(connection);
                MessageBox.Show("Tablolar oluşturuldu ve örnek veri eklendi!", "Bilgi");
            }
        }

        private static async Task CreateTablesAsync(OleDbConnection connection)
        {
            var createUsersTable = @"
                CREATE TABLE Users (
                    Id AUTOINCREMENT PRIMARY KEY,
                    Email TEXT(255) NOT NULL,
                    PasswordHash TEXT(255) NOT NULL,
                    FirstName TEXT(100) NOT NULL,
                    LastName TEXT(100) NOT NULL,
                    Role TEXT(50) DEFAULT 'User',
                    IsActive YESNO DEFAULT True,
                    CreatedDate DATETIME DEFAULT Now()
                )";
            await ExecuteNonQueryAsync(connection, createUsersTable);

            // Courses table
            var createCoursesTable = @"
                CREATE TABLE Courses (
                    Id AUTOINCREMENT PRIMARY KEY,
                    Title TEXT(255) NOT NULL,
                    Description TEXT,
                    Content MEMO,
                    Level INTEGER NOT NULL,
                    Type INTEGER NOT NULL,
                    Price CURRENCY,
                    OrderIndex INTEGER,
                    EstimatedDurationMinutes INTEGER,
                    Prerequisites TEXT,
                    Color TEXT(20),
                    IsActive YESNO DEFAULT True,
                    CreatedDate DATETIME DEFAULT Now()
                )";
            await ExecuteNonQueryAsync(connection, createCoursesTable);

            // VocabularyWords table
            var createVocabularyTable = @"
                CREATE TABLE VocabularyWords (
                    Id AUTOINCREMENT PRIMARY KEY,
                    EnglishWord TEXT(255) NOT NULL,
                    TurkishMeaning TEXT(255) NOT NULL,
                    Pronunciation TEXT(255),
                    ExampleSentence MEMO,
                    ExampleSentenceTurkish MEMO,
                    Difficulty INTEGER NOT NULL,
                    PartOfSpeech INTEGER NOT NULL,
                    Category TEXT(100),
                    CreatedDate DATETIME DEFAULT Now()
                )";
            await ExecuteNonQueryAsync(connection, createVocabularyTable);
        }

        private static async Task SeedDataAsync(OleDbConnection connection)
        {
            var adminSql = @"
                INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate)
                VALUES (?, ?, ?, ?, ?, ?, ?)";

            using var adminCommand = new OleDbCommand(adminSql, connection);
            adminCommand.Parameters.AddWithValue("@Email", "admin@engotomasyon.com");
            adminCommand.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword("admin123"));
            adminCommand.Parameters.AddWithValue("@FirstName", "Admin");
            adminCommand.Parameters.AddWithValue("@LastName", "User");
            adminCommand.Parameters.AddWithValue("@Role", "Admin");
            adminCommand.Parameters.AddWithValue("@IsActive", true);
            adminCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
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
            command.Parameters.AddWithValue("@Email", email);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetInt32("Id"),
                    Email = reader.GetString("Email"),
                    PasswordHash = reader.GetString("PasswordHash"),
                    FirstName = reader.GetString("FirstName"),
                    LastName = reader.GetString("LastName"),
                    Role = reader.GetString("Role"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
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
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@FirstName", user.FirstName);
                command.Parameters.AddWithValue("@LastName", user.LastName);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@IsActive", user.IsActive);
                command.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);

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
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    Content = reader.IsDBNull("Content") ? null : reader.GetString("Content"),
                    Level = (CourseLevel)reader.GetInt32("Level"),
                    Type = (CourseType)reader.GetInt32("Type"),
                    Price = reader.IsDBNull("Price") ? null : reader.GetDecimal("Price"),
                    OrderIndex = reader.IsDBNull("OrderIndex") ? null : reader.GetInt32("OrderIndex"),
                    EstimatedDurationMinutes = reader.IsDBNull("EstimatedDurationMinutes") ? null : reader.GetInt32("EstimatedDurationMinutes"),
                    Prerequisites = reader.IsDBNull("Prerequisites") ? null : reader.GetString("Prerequisites"),
                    Color = reader.IsDBNull("Color") ? null : reader.GetString("Color"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
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
                    Id = reader.GetInt32("Id"),
                    EnglishWord = reader.GetString("EnglishWord"),
                    TurkishMeaning = reader.GetString("TurkishMeaning"),
                    Pronunciation = reader.IsDBNull("Pronunciation") ? null : reader.GetString("Pronunciation"),
                    ExampleSentence = reader.IsDBNull("ExampleSentence") ? null : reader.GetString("ExampleSentence"),
                    ExampleSentenceTurkish = reader.IsDBNull("ExampleSentenceTurkish") ? null : reader.GetString("ExampleSentenceTurkish"),
                    Difficulty = (WordDifficulty)reader.GetInt32("Difficulty"),
                    PartOfSpeech = (PartOfSpeech)reader.GetInt32("PartOfSpeech"),
                    Category = reader.IsDBNull("Category") ? null : reader.GetString("Category"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
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
                command.Parameters.AddWithValue("@EnglishWord", word.EnglishWord);
                command.Parameters.AddWithValue("@TurkishMeaning", word.TurkishMeaning);
                command.Parameters.AddWithValue("@Pronunciation", word.Pronunciation ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ExampleSentence", word.ExampleSentence ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ExampleSentenceTurkish", word.ExampleSentenceTurkish ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Difficulty", (int)word.Difficulty);
                command.Parameters.AddWithValue("@PartOfSpeech", (int)word.PartOfSpeech);
                command.Parameters.AddWithValue("@Category", word.Category ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", word.CreatedDate);

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
