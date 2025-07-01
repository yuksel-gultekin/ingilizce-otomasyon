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

            using var connection = new OleDbConnection(GetConnectionString());
            await connection.OpenAsync();
            await EnsureTablesExistAsync(connection);
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

                var createVocabularyTable = @"
            CREATE TABLE VocabularyWords (
                Id COUNTER PRIMARY KEY,
                EnglishWord TEXT(255),
                TurkishMeaning TEXT(255),
                Pronunciation TEXT(255),
                ExampleSentence MEMO,
                ExampleSentenceTurkish MEMO,
                Difficulty INTEGER,
                PartOfSpeech INTEGER,
                Category TEXT(100),
                AudioUrl TEXT(500),
                IsActive YESNO,
                CreatedDate DATETIME
            )";
                await ExecuteNonQueryAsync(connection, createVocabularyTable);

                // ✅ Users table
                var createUsersTable = @"
            CREATE TABLE Users (
                Id COUNTER PRIMARY KEY,
                Email TEXT(255) NOT NULL,
                PasswordHash TEXT(255),
                FirstName TEXT(100),
                LastName TEXT(100),
                Role TEXT(50),
                IsActive YESNO,
                CreatedDate DATETIME
            )";
                await ExecuteNonQueryAsync(connection, createUsersTable);

                // ✅ Courses table
                var createCoursesTable = @"
            CREATE TABLE Courses (
                Id COUNTER PRIMARY KEY,
                Title TEXT(255),
                DescriptionText TEXT(255),
                ContentText TEXT(255),
                CourseLevel INTEGER,
                CourseType INTEGER,
                Price CURRENCY,
                OrderIndex INTEGER,
                EstimatedDurationMinutes INTEGER,
                Prerequisites TEXT(255),
                Color TEXT(20),
                IsActive YESNO,
                CreatedDate DATETIME
            )";


                await ExecuteNonQueryAsync(connection, createCoursesTable);

                // ✅ UserProgress table
                var createUserProgressTable = @"
            CREATE TABLE UserProgress (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER,
                CourseId INTEGER,
                Status INTEGER,
                ProgressPercentage INTEGER,
                TimeSpentMinutes INTEGER,
                StartDate DATETIME,
                CompletionDate DATETIME,
                LastAccessDate DATETIME
            )";
                await ExecuteNonQueryAsync(connection, createUserProgressTable);

                // ✅ UserVocabulary table
                var createUserVocabularyTable = @"
            CREATE TABLE UserVocabulary (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER,
                VocabularyWordId INTEGER,
                MasteryLevel INTEGER,
                FirstLearnedDate DATETIME,
                LastReviewedDate DATETIME,
                ReviewCount INTEGER,
                CorrectAnswers INTEGER,
                TotalAttempts INTEGER
            )";
                await ExecuteNonQueryAsync(connection, createUserVocabularyTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablo oluşturma hatası: {ex.Message}", "Hata");
                throw;
            }
        }


        private static async Task SeedDataAsync(OleDbConnection connection)
        {
            // Seed admin user
            var adminSql = @"
        INSERT INTO Users (Email, PasswordHash, FirstName, LastName, Role, IsActive, CreatedDate)
        VALUES (?, ?, ?, ?, ?, ?, ?)";

            using var adminCommand = new OleDbCommand(adminSql, connection);
            adminCommand.Parameters.AddWithValue("@p1", "admin@engotomasyon.com");
            adminCommand.Parameters.AddWithValue("@p2", BCrypt.Net.BCrypt.HashPassword("admin123"));
            adminCommand.Parameters.AddWithValue("@p3", "Admin");
            adminCommand.Parameters.AddWithValue("@p4", "User");
            adminCommand.Parameters.AddWithValue("@p5", "Admin");
            adminCommand.Parameters.AddWithValue("@p6", true);
            adminCommand.Parameters.AddWithValue("@p7", DateTime.Now);

            await adminCommand.ExecuteNonQueryAsync();

            // Seed sample vocabulary words
            await SeedVocabularyWordsAsync(connection);

            // Seed sample courses
            await SeedCoursesAsync(connection);
        }

        private static async Task SeedVocabularyWordsAsync(OleDbConnection connection)
        {
            var sampleWords = new[]
            {
                new { English = "Hello", Turkish = "Merhaba", Pronunciation = "/həˈloʊ/", Example = "Hello, how are you?", ExampleTr = "Merhaba, nasılsın?", Difficulty = 1, PartOfSpeech = 8, Category = "Greetings" },
                new { English = "Goodbye", Turkish = "Hoşçakal", Pronunciation = "/ɡʊdˈbaɪ/", Example = "Goodbye, see you tomorrow!", ExampleTr = "Hoşçakal, yarın görüşürüz!", Difficulty = 1, PartOfSpeech = 8, Category = "Greetings" },
                new { English = "Thank you", Turkish = "Teşekkür ederim", Pronunciation = "/θæŋk juː/", Example = "Thank you for your help.", ExampleTr = "Yardımın için teşekkür ederim.", Difficulty = 1, PartOfSpeech = 8, Category = "Greetings" },
                new { English = "Book", Turkish = "Kitap", Pronunciation = "/bʊk/", Example = "I am reading a book.", ExampleTr = "Bir kitap okuyorum.", Difficulty = 1, PartOfSpeech = 1, Category = "Objects" },
                new { English = "Water", Turkish = "Su", Pronunciation = "/ˈwɔːtər/", Example = "I need some water.", ExampleTr = "Biraz suya ihtiyacım var.", Difficulty = 1, PartOfSpeech = 1, Category = "Food & Drink" }
            };

            var sql = @"INSERT INTO VocabularyWords (EnglishWord, TurkishMeaning, Pronunciation, ExampleSentence, ExampleSentenceTurkish, Difficulty, PartOfSpeech, Category, AudioUrl, IsActive, CreatedDate)
                       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            foreach (var word in sampleWords)
            {
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("?", word.English);
                command.Parameters.AddWithValue("?", word.Turkish);
                command.Parameters.AddWithValue("?", word.Pronunciation);
                command.Parameters.AddWithValue("?", word.Example);
                command.Parameters.AddWithValue("?", word.ExampleTr);
                command.Parameters.AddWithValue("?", word.Difficulty);
                command.Parameters.AddWithValue("?", word.PartOfSpeech);
                command.Parameters.AddWithValue("?", word.Category);
                command.Parameters.AddWithValue("?", DBNull.Value); // AudioUrl
                command.Parameters.AddWithValue("?", true); // IsActive
                command.Parameters.AddWithValue("?", DateTime.Now);

                await command.ExecuteNonQueryAsync();
            }
        }

        private static async Task SeedCoursesAsync(OleDbConnection connection)
        {
            var sampleCourses = new[]
            {
                new { Title = "English Basics", Description = "Learn basic English words and phrases", Content = "Introduction to English language fundamentals", Level = 1, Type = 2, Price = 0.0m, OrderIndex = 1, Duration = 30, Prerequisites = "", Color = "#4CAF50" },
                new { Title = "Grammar Fundamentals", Description = "Essential English grammar rules", Content = "Learn the basic grammar structures", Level = 1, Type = 1, Price = 0.0m, OrderIndex = 2, Duration = 45, Prerequisites = "", Color = "#2196F3" },
                new { Title = "Everyday Conversations", Description = "Common phrases for daily communication", Content = "Practice speaking in everyday situations", Level = 2, Type = 3, Price = 0.0m, OrderIndex = 3, Duration = 60, Prerequisites = "English Basics", Color = "#FF9800" }
            };

            var sql = @"INSERT INTO Courses (Title, DescriptionText, ContentText, CourseLevel, CourseType, Price, OrderIndex, EstimatedDurationMinutes, Prerequisites, Color, IsActive, CreatedDate)
                       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            foreach (var course in sampleCourses)
            {
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("?", course.Title);
                command.Parameters.AddWithValue("?", course.Description);
                command.Parameters.AddWithValue("?", course.Content);
                command.Parameters.AddWithValue("?", course.Level);
                command.Parameters.AddWithValue("?", course.Type);
                command.Parameters.AddWithValue("?", course.Price);
                command.Parameters.AddWithValue("?", course.OrderIndex);
                command.Parameters.AddWithValue("?", course.Duration);
                command.Parameters.AddWithValue("?", course.Prerequisites);
                command.Parameters.AddWithValue("?", course.Color);
                command.Parameters.AddWithValue("?", true); // IsActive
                command.Parameters.AddWithValue("?", DateTime.Now);

                await command.ExecuteNonQueryAsync();
            }
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
                    Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2), // DescriptionText
                    Content = reader.IsDBNull(3) ? string.Empty : reader.GetString(3), // ContentText
                    Level = (CourseLevel)reader.GetInt32(4), // CourseLevel
                    Type = (CourseType)reader.GetInt32(5), // CourseType
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

            var sql = "SELECT * FROM VocabularyWords WHERE IsActive = True ORDER BY EnglishWord";
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
                    AudioUrl = reader.IsDBNull(9) ? null : reader.GetString(9), // AudioUrl
                    IsActive = reader.GetBoolean(10), // IsActive
                    CreatedDate = reader.GetDateTime(11) // CreatedDate
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

                var sql = @"INSERT INTO VocabularyWords (EnglishWord, TurkishMeaning, Pronunciation, ExampleSentence, ExampleSentenceTurkish, Difficulty, PartOfSpeech, Category, AudioUrl, IsActive, CreatedDate)
                           VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.AddWithValue("?", word.EnglishWord);
                command.Parameters.AddWithValue("?", word.TurkishMeaning);
                command.Parameters.AddWithValue("?", word.Pronunciation ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.ExampleSentence ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.ExampleSentenceTurkish ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", (int)word.Difficulty);
                command.Parameters.AddWithValue("?", (int)word.PartOfSpeech);
                command.Parameters.AddWithValue("?", word.Category ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.AudioUrl ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("?", word.IsActive);
                command.Parameters.AddWithValue("?", word.CreatedDate);

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // UserProgress operations
        public static async Task<List<UserProgress>> GetUserProgressAsync(int userId)
        {
            var progressList = new List<UserProgress>();
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM UserProgress WHERE UserId = ?";
            using var command = new OleDbCommand(sql, connection);
            command.Parameters.AddWithValue("?", userId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                progressList.Add(new UserProgress
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    CourseId = reader.GetInt32(2),
                    Status = (ProgressStatus)reader.GetInt32(3),
                    ProgressPercentage = reader.GetInt32(4),
                    TimeSpentMinutes = reader.GetInt32(5),
                    StartDate = reader.GetDateTime(6),
                    CompletionDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                    LastAccessDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                });
            }
            return progressList;
        }

        public static async Task<bool> CreateOrUpdateUserProgressAsync(UserProgress progress)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                // Check if progress exists
                var checkSql = "SELECT COUNT(*) FROM UserProgress WHERE UserId = ? AND CourseId = ?";
                using var checkCommand = new OleDbCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("?", progress.UserId);
                checkCommand.Parameters.AddWithValue("?", progress.CourseId);
                var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;

                string sql;
                if (exists)
                {
                    sql = @"UPDATE UserProgress SET Status = ?, ProgressPercentage = ?, TimeSpentMinutes = ?,
                           CompletionDate = ?, LastAccessDate = ?
                           WHERE UserId = ? AND CourseId = ?";
                }
                else
                {
                    sql = @"INSERT INTO UserProgress (UserId, CourseId, Status, ProgressPercentage, TimeSpentMinutes, StartDate, CompletionDate, LastAccessDate)
                           VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                }

                using var command = new OleDbCommand(sql, connection);
                if (exists)
                {
                    command.Parameters.AddWithValue("?", (int)progress.Status);
                    command.Parameters.AddWithValue("?", progress.ProgressPercentage);
                    command.Parameters.AddWithValue("?", progress.TimeSpentMinutes);
                    command.Parameters.AddWithValue("?", progress.CompletionDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("?", progress.LastAccessDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("?", progress.UserId);
                    command.Parameters.AddWithValue("?", progress.CourseId);
                }
                else
                {
                    command.Parameters.AddWithValue("?", progress.UserId);
                    command.Parameters.AddWithValue("?", progress.CourseId);
                    command.Parameters.AddWithValue("?", (int)progress.Status);
                    command.Parameters.AddWithValue("?", progress.ProgressPercentage);
                    command.Parameters.AddWithValue("?", progress.TimeSpentMinutes);
                    command.Parameters.AddWithValue("?", progress.StartDate);
                    command.Parameters.AddWithValue("?", progress.CompletionDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("?", progress.LastAccessDate ?? (object)DBNull.Value);
                }

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // UserVocabulary operations
        public static async Task<List<UserVocabulary>> GetUserVocabularyAsync(int userId)
        {
            var userVocabulary = new List<UserVocabulary>();
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM UserVocabulary WHERE UserId = ?";
            using var command = new OleDbCommand(sql, connection);
            command.Parameters.AddWithValue("?", userId);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                userVocabulary.Add(new UserVocabulary
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    VocabularyWordId = reader.GetInt32(2),
                    MasteryLevel = reader.GetInt32(3),
                    FirstLearnedDate = reader.GetDateTime(4),
                    LastReviewedDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    ReviewCount = reader.GetInt32(6),
                    CorrectAnswers = reader.GetInt32(7),
                    TotalAttempts = reader.GetInt32(8)
                });
            }
            return userVocabulary;
        }

        public static async Task<bool> CreateOrUpdateUserVocabularyAsync(UserVocabulary userVocab)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                // Check if record exists
                var checkSql = "SELECT COUNT(*) FROM UserVocabulary WHERE UserId = ? AND VocabularyWordId = ?";
                using var checkCommand = new OleDbCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("?", userVocab.UserId);
                checkCommand.Parameters.AddWithValue("?", userVocab.VocabularyWordId);
                var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;

                string sql;
                if (exists)
                {
                    sql = @"UPDATE UserVocabulary SET MasteryLevel = ?, LastReviewedDate = ?, ReviewCount = ?,
                           CorrectAnswers = ?, TotalAttempts = ?
                           WHERE UserId = ? AND VocabularyWordId = ?";
                }
                else
                {
                    sql = @"INSERT INTO UserVocabulary (UserId, VocabularyWordId, MasteryLevel, FirstLearnedDate, LastReviewedDate, ReviewCount, CorrectAnswers, TotalAttempts)
                           VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
                }

                using var command = new OleDbCommand(sql, connection);
                if (exists)
                {
                    command.Parameters.AddWithValue("?", userVocab.MasteryLevel);
                    command.Parameters.AddWithValue("?", userVocab.LastReviewedDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("?", userVocab.ReviewCount);
                    command.Parameters.AddWithValue("?", userVocab.CorrectAnswers);
                    command.Parameters.AddWithValue("?", userVocab.TotalAttempts);
                    command.Parameters.AddWithValue("?", userVocab.UserId);
                    command.Parameters.AddWithValue("?", userVocab.VocabularyWordId);
                }
                else
                {
                    command.Parameters.AddWithValue("?", userVocab.UserId);
                    command.Parameters.AddWithValue("?", userVocab.VocabularyWordId);
                    command.Parameters.AddWithValue("?", userVocab.MasteryLevel);
                    command.Parameters.AddWithValue("?", userVocab.FirstLearnedDate);
                    command.Parameters.AddWithValue("?", userVocab.LastReviewedDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("?", userVocab.ReviewCount);
                    command.Parameters.AddWithValue("?", userVocab.CorrectAnswers);
                    command.Parameters.AddWithValue("?", userVocab.TotalAttempts);
                }

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
