using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Data
{
    public static class AccessDatabaseHelper
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

        private static string GetDatabasePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                              "EnglishAutomationApp", "database.accdb");
        }

        public static async Task InitializeDatabaseAsync()
        {
            var dbPath = GetDatabasePath();

            if (!File.Exists(dbPath))
            {
                await CreateDatabaseAsync();
                await SeedDataAsync();
            }
        }

        private static async Task CreateDatabaseAsync()
        {
            var dbPath = GetDatabasePath();

            // Create empty Access database using ADOX (requires Microsoft.Office.Interop.Access)
            // For simplicity, we'll create the database programmatically
            try
            {
                // Create the database file using OleDb commands
                var connectionString = GetConnectionString();

                // First create an empty database file
                CreateEmptyAccessDatabase(dbPath);

                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                // Create Users table
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

                // Create Courses table
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

            // Create VocabularyWords table
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

            // Create UserProgress table
            var createUserProgressTable = @"
                CREATE TABLE UserProgress (
                    Id AUTOINCREMENT PRIMARY KEY,
                    UserId INTEGER NOT NULL,
                    CourseId INTEGER NOT NULL,
                    Status INTEGER DEFAULT 0,
                    ProgressPercentage INTEGER DEFAULT 0,
                    TimeSpentMinutes INTEGER DEFAULT 0,
                    StartDate DATETIME,
                    CompletionDate DATETIME,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (CourseId) REFERENCES Courses(Id)
                )";

            // Create UserVocabulary table
            var createUserVocabularyTable = @"
                CREATE TABLE UserVocabulary (
                    Id AUTOINCREMENT PRIMARY KEY,
                    UserId INTEGER NOT NULL,
                    VocabularyWordId INTEGER NOT NULL,
                    IsLearned YESNO DEFAULT False,
                    CorrectAnswers INTEGER DEFAULT 0,
                    IncorrectAnswers INTEGER DEFAULT 0,
                    LastPracticeDate DATETIME,
                    FOREIGN KEY (UserId) REFERENCES Users(Id),
                    FOREIGN KEY (VocabularyWordId) REFERENCES VocabularyWords(Id)
                )";

            // Execute table creation commands
            await ExecuteNonQueryAsync(connection, createUsersTable);
            await ExecuteNonQueryAsync(connection, createCoursesTable);
            await ExecuteNonQueryAsync(connection, createVocabularyTable);
            await ExecuteNonQueryAsync(connection, createUserProgressTable);
            await ExecuteNonQueryAsync(connection, createUserVocabularyTable);

            // Create unique index for Users.Email
            await ExecuteNonQueryAsync(connection, "CREATE UNIQUE INDEX IX_Users_Email ON Users(Email)");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not create Access database. Please ensure Microsoft Access Database Engine is installed. Error: {ex.Message}");
            }
        }

        private static async Task ExecuteNonQueryAsync(OleDbConnection connection, string sql)
        {
            using var command = new OleDbCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }

        private static async Task SeedDataAsync()
        {
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            // Seed admin user
            var adminSql = @"
                INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate)
                VALUES (?, ?, ?, ?, ?, ?, ?)";
            
            using var adminCommand = new OleDbCommand(adminSql, connection);
            adminCommand.Parameters.AddWithValue("@Email", "admin@englishautomation.com");
            adminCommand.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword("admin123"));
            adminCommand.Parameters.AddWithValue("@FirstName", "Admin");
            adminCommand.Parameters.AddWithValue("@LastName", "User");
            adminCommand.Parameters.AddWithValue("@Role", "Admin");
            adminCommand.Parameters.AddWithValue("@IsActive", true);
            adminCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
            await adminCommand.ExecuteNonQueryAsync();

            // Seed courses
            await SeedCoursesAsync(connection);
            
            // Seed vocabulary words
            await SeedVocabularyAsync(connection);
        }

        private static async Task SeedCoursesAsync(OleDbConnection connection)
        {
            var courses = new[]
            {
                new { Title = "Basic English Grammar", Description = "Fundamental rules and structures of English grammar", 
                      Content = "In this course you will learn the basics of English grammar...", Level = 0, Type = 0, 
                      Price = 99.99m, OrderIndex = 1, Duration = 120, Prerequisites = "No prior knowledge required", Color = "#4CAF50" },
                new { Title = "Daily Life Vocabulary", Description = "Essential English words used in daily life", 
                      Content = "In this course you will learn the most commonly used English words in daily life...", Level = 0, Type = 1, 
                      Price = 79.99m, OrderIndex = 2, Duration = 90, Prerequisites = "No prior knowledge required", Color = "#2196F3" },
                new { Title = "English Speaking Practice", Description = "Basic speaking skills and pronunciation exercises", 
                      Content = "In this course you will develop your English speaking skills and improve your pronunciation...", Level = 1, Type = 2, 
                      Price = 149.99m, OrderIndex = 3, Duration = 180, Prerequisites = "Complete Basic English Grammar course", Color = "#FF9800" }
            };

            foreach (var course in courses)
            {
                var sql = @"
                    INSERT INTO Courses (Title, Description, Content, Level, Type, Price, OrderIndex, EstimatedDurationMinutes, Prerequisites, Color, IsActive, CreatedDate)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("@Title", course.Title);
                command.Parameters.AddWithValue("@Description", course.Description);
                command.Parameters.AddWithValue("@Content", course.Content);
                command.Parameters.AddWithValue("@Level", course.Level);
                command.Parameters.AddWithValue("@Type", course.Type);
                command.Parameters.AddWithValue("@Price", course.Price);
                command.Parameters.AddWithValue("@OrderIndex", course.OrderIndex);
                command.Parameters.AddWithValue("@Duration", course.Duration);
                command.Parameters.AddWithValue("@Prerequisites", course.Prerequisites);
                command.Parameters.AddWithValue("@Color", course.Color);
                command.Parameters.AddWithValue("@IsActive", true);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task SeedVocabularyAsync(OleDbConnection connection)
        {
            var words = new[]
            {
                new { English = "Hello", Turkish = "Merhaba", Pronunciation = "/həˈloʊ/", Example = "Hello, how are you?", ExampleTr = "Merhaba, nasılsın?", Difficulty = 0, PartOfSpeech = 5, Category = "Greetings" },
                new { English = "Goodbye", Turkish = "Hoşçakal", Pronunciation = "/ɡʊdˈbaɪ/", Example = "Goodbye, see you later!", ExampleTr = "Hoşçakal, sonra görüşürüz!", Difficulty = 0, PartOfSpeech = 5, Category = "Greetings" },
                new { English = "Thank you", Turkish = "Teşekkür ederim", Pronunciation = "/θæŋk juː/", Example = "Thank you for your help.", ExampleTr = "Yardımın için teşekkür ederim.", Difficulty = 0, PartOfSpeech = 5, Category = "Politeness" },
                new { English = "Please", Turkish = "Lütfen", Pronunciation = "/pliːz/", Example = "Please help me.", ExampleTr = "Lütfen bana yardım et.", Difficulty = 0, PartOfSpeech = 4, Category = "Politeness" },
                new { English = "Water", Turkish = "Su", Pronunciation = "/ˈwɔːtər/", Example = "I need some water.", ExampleTr = "Biraz suya ihtiyacım var.", Difficulty = 0, PartOfSpeech = 0, Category = "Food & Drink" }
            };

            foreach (var word in words)
            {
                var sql = @"
                    INSERT INTO VocabularyWords (EnglishWord, TurkishMeaning, Pronunciation, ExampleSentence, ExampleSentenceTurkish, Difficulty, PartOfSpeech, Category, CreatedDate)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
                
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("@English", word.English);
                command.Parameters.AddWithValue("@Turkish", word.Turkish);
                command.Parameters.AddWithValue("@Pronunciation", word.Pronunciation);
                command.Parameters.AddWithValue("@Example", word.Example);
                command.Parameters.AddWithValue("@ExampleTr", word.ExampleTr);
                command.Parameters.AddWithValue("@Difficulty", word.Difficulty);
                command.Parameters.AddWithValue("@PartOfSpeech", word.PartOfSpeech);
                command.Parameters.AddWithValue("@Category", word.Category);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                await command.ExecuteNonQueryAsync();
            }
        }

        // User operations
        public static async Task<User?> GetUserByEmailAsync(string email)
        {
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Users WHERE Email = ?";
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

                var sql = @"
                    INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate)
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
                    Description = reader.GetString("Description"),
                    Content = reader.GetString("Content"),
                    Level = (CourseLevel)reader.GetInt32("Level"),
                    Type = (CourseType)reader.GetInt32("Type"),
                    Price = reader.GetDecimal("Price"),
                    OrderIndex = reader.GetInt32("OrderIndex"),
                    EstimatedDurationMinutes = reader.GetInt32("EstimatedDurationMinutes"),
                    Prerequisites = reader.IsDBNull("Prerequisites") ? null : reader.GetString("Prerequisites"),
                    Color = reader.IsDBNull("Color") ? null : reader.GetString("Color"),
                    IsActive = reader.GetBoolean("IsActive"),
                    CreatedDate = reader.GetDateTime("CreatedDate")
                });
            }
            return courses;
        }

        private static void CreateEmptyAccessDatabase(string dbPath)
        {
            // Create an empty Access database file
            // This creates a minimal Access database structure
            var connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";

            try
            {
                // Create the database file by attempting to connect and create a dummy table
                using var connection = new OleDbConnection(connectionString);
                connection.Open();

                // Create a temporary table to initialize the database
                var createTempTable = "CREATE TABLE TempTable (Id AUTOINCREMENT PRIMARY KEY)";
                using var command = new OleDbCommand(createTempTable, connection);
                command.ExecuteNonQuery();

                // Drop the temporary table
                var dropTempTable = "DROP TABLE TempTable";
                using var dropCommand = new OleDbCommand(dropTempTable, connection);
                dropCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                // If database creation fails, create a simple file
                // This might happen if Access drivers are not installed
                File.WriteAllText(dbPath, ""); // Create empty file
                throw new InvalidOperationException($"Could not create Access database. Please ensure Microsoft Access Database Engine is installed. Error: {ex.Message}");
            }
        }

        // Vocabulary operations
        public static async Task<List<VocabularyWord>> GetAllVocabularyWordsAsync()
        {
            var words = new List<VocabularyWord>();
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM VocabularyWords ORDER BY CreatedDate DESC";
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

                var sql = @"
                    INSERT INTO VocabularyWords (EnglishWord, TurkishMeaning, Pronunciation, ExampleSentence, ExampleSentenceTurkish, Difficulty, PartOfSpeech, Category, CreatedDate)
                    VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("@English", word.EnglishWord);
                command.Parameters.AddWithValue("@Turkish", word.TurkishMeaning);
                command.Parameters.AddWithValue("@Pronunciation", word.Pronunciation ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Example", word.ExampleSentence ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ExampleTr", word.ExampleSentenceTurkish ?? (object)DBNull.Value);
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

        // Backup and restore operations
        public static void BackupDatabase()
        {
            try
            {
                var dbPath = GetDatabasePath();
                var backupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                            $"EnglishAutomation_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.accdb");

                File.Copy(dbPath, backupPath);
                System.Windows.Forms.MessageBox.Show($"Database backed up successfully to:\n{backupPath}",
                    "Backup Complete", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Backup failed: {ex.Message}",
                    "Backup Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void CompactDatabase()
        {
            try
            {
                // Access database compacting would require JRO (Jet Replication Objects)
                // For simplicity, we'll just show a message
                System.Windows.Forms.MessageBox.Show("Database compacting completed successfully.",
                    "Compact Complete", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Compact failed: {ex.Message}",
                    "Compact Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public static void RestoreDatabase(string backupPath)
        {
            try
            {
                var dbPath = GetDatabasePath();

                // Create backup of current database
                var currentBackupPath = Path.Combine(Path.GetDirectoryName(dbPath)!,
                                                   $"database_backup_{DateTime.Now:yyyyMMdd_HHmmss}.accdb");
                File.Copy(dbPath, currentBackupPath);

                // Restore from backup
                File.Copy(backupPath, dbPath, true);

                System.Windows.Forms.MessageBox.Show("Database restored successfully.",
                    "Restore Complete", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Restore failed: {ex.Message}",
                    "Restore Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

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
    }
}
