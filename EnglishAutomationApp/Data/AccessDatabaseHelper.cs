using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Models;
using static EnglishAutomationApp.Models.WordDifficulty;
using static EnglishAutomationApp.Models.CourseLevel;
using static EnglishAutomationApp.Models.CourseType;

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
            try
            {
                var dbPath = GetDatabasePath();

                if (!File.Exists(dbPath))
                {
                    await CreateEmptyAccessDatabaseAsync(dbPath);
                }

                using var connection = new OleDbConnection(GetConnectionString());
                await connection.OpenAsync();

                await EnsureTablesExistAsync(connection);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task CreateEmptyAccessDatabaseAsync(string dbPath)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Creating new Access database at: {dbPath}");

                // Ensure the directory exists
                var directory = Path.GetDirectoryName(dbPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create empty Access database using ADOX
                try
                {
                    var catalogType = Type.GetTypeFromProgID("ADOX.Catalog");
                    if (catalogType == null) throw new InvalidOperationException("ADOX.Catalog not available");

                    dynamic catalog = Activator.CreateInstance(catalogType)!;
                    catalog.Create($"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};");
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(catalog);

                }
                catch (Exception adoxEx)
                {

                    throw new Exception($"Could not create Access database. Please ensure Microsoft Access Database Engine is installed. Error: {adoxEx.Message}");
                }

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating database: {ex.Message}");
                throw;
            }
        }




        private static async Task EnsureTablesExistAsync(OleDbConnection connection)
        {
            var tableExists = false;

            try
            {
                var checkSql = "SELECT COUNT(*) FROM Users";
                using var cmd = new OleDbCommand(checkSql, connection);
                var result = await cmd.ExecuteScalarAsync();
                tableExists = result != null; // If query succeeds, table exists
                System.Diagnostics.Debug.WriteLine("Tables already exist");
            }
            catch (Exception ex)
            {
                // If query fails, table doesn't exist
                System.Diagnostics.Debug.WriteLine($"Tables don't exist, will create them. Error: {ex.Message}");
                tableExists = false;
            }

            if (!tableExists)
            {
                System.Diagnostics.Debug.WriteLine("Creating database tables...");
                await CreateTablesAsync(connection);
                System.Diagnostics.Debug.WriteLine("Seeding initial data...");
                await SeedDataAsync(connection);
                System.Diagnostics.Debug.WriteLine("Database setup completed");
            }
        }

        private static async Task CreateTablesAsync(OleDbConnection connection)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Creating VocabularyWords table...");
                var createVocabularyTable = @"
            CREATE TABLE VocabularyWords (
                Id COUNTER PRIMARY KEY,
                EnglishWord TEXT(255) NOT NULL,
                TurkishMeaning TEXT(255) NOT NULL,
                Pronunciation TEXT(255),
                ExampleSentence MEMO,
                ExampleSentenceTurkish MEMO,
                Difficulty INTEGER NOT NULL,
                PartOfSpeech INTEGER NOT NULL,
                Category TEXT(100),
                AudioUrl TEXT(255),
                IsActive INTEGER NOT NULL,
                CreatedDate DATETIME NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createVocabularyTable);

                System.Diagnostics.Debug.WriteLine("Creating Users table...");
                // ✅ Users table
                var createUsersTable = @"
            CREATE TABLE Users (
                Id COUNTER PRIMARY KEY,
                Email TEXT(255) NOT NULL,
                PasswordHash TEXT(255) NOT NULL,
                Role TEXT(50) NOT NULL,
                FirstName TEXT(100),
                LastName TEXT(100),
                IsActive INTEGER NOT NULL,
                IsAdmin INTEGER NOT NULL DEFAULT 0,
                CreatedDate DATETIME NOT NULL,
                LastLoginDate DATETIME
            )";
                await ExecuteNonQueryAsync(connection, createUsersTable);

                // ✅ Courses table
                var createCoursesTable = @"
            CREATE TABLE Courses (
                Id COUNTER PRIMARY KEY,
                Title TEXT(255) NOT NULL,
                DescriptionText TEXT(255),
                ContentText MEMO,
                CourseLevel INTEGER NOT NULL,
                CourseType INTEGER NOT NULL,
                Price CURRENCY,
                OrderIndex INTEGER,
                EstimatedDurationMinutes INTEGER,
                Prerequisites TEXT(255),
                Color TEXT(20),
                IsActive INTEGER NOT NULL,
                CreatedDate DATETIME NOT NULL
            )";


                await ExecuteNonQueryAsync(connection, createCoursesTable);

                // ✅ UserProgress table
                var createUserProgressTable = @"
            CREATE TABLE UserProgress (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER NOT NULL,
                CourseId INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                ProgressPercentage INTEGER NOT NULL,
                TimeSpentMinutes INTEGER NOT NULL,
                StartDate DATETIME NOT NULL,
                CompletionDate DATETIME,
                LastAccessDate DATETIME
            )";
                await ExecuteNonQueryAsync(connection, createUserProgressTable);

                // ✅ UserVocabulary table
                var createUserVocabularyTable = @"
            CREATE TABLE UserVocabulary (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER NOT NULL,
                VocabularyWordId INTEGER NOT NULL,
                MasteryLevel INTEGER NOT NULL,
                FirstLearnedDate DATETIME NOT NULL,
                LastReviewedDate DATETIME,
                ReviewCount INTEGER NOT NULL,
                CorrectAnswers INTEGER NOT NULL,
                TotalAttempts INTEGER NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createUserVocabularyTable);

                // ✅ Lessons table
                var createLessonsTable = @"
            CREATE TABLE Lessons (
                Id COUNTER PRIMARY KEY,
                CourseId INTEGER NOT NULL,
                Title TEXT(200) NOT NULL,
                ContentText MEMO NOT NULL,
                LessonType INTEGER NOT NULL,
                OrderIndex INTEGER NOT NULL,
                EstimatedDurationMinutes INTEGER,
                VideoUrl TEXT(255),
                AudioUrl TEXT(255),
                IsActive INTEGER NOT NULL,
                CreatedDate DATETIME NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createLessonsTable);

                // ✅ Quizzes table
                var createQuizzesTable = @"
            CREATE TABLE Quizzes (
                Id COUNTER PRIMARY KEY,
                LessonId INTEGER NOT NULL,
                Title TEXT(200) NOT NULL,
                DescriptionText MEMO NOT NULL,
                PassingScore INTEGER NOT NULL,
                TimeLimit INTEGER,
                IsActive INTEGER NOT NULL,
                CreatedDate DATETIME NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createQuizzesTable);

                // ✅ Questions table
                var createQuestionsTable = @"
            CREATE TABLE Questions (
                Id COUNTER PRIMARY KEY,
                QuizId INTEGER NOT NULL,
                QuestionText MEMO NOT NULL,
                QuestionType INTEGER NOT NULL,
                OrderIndex INTEGER NOT NULL,
                Points INTEGER NOT NULL,
                Explanation MEMO
            )";
                await ExecuteNonQueryAsync(connection, createQuestionsTable);

                // ✅ Answers table
                var createAnswersTable = @"
            CREATE TABLE Answers (
                Id COUNTER PRIMARY KEY,
                QuestionId INTEGER NOT NULL,
                AnswerText MEMO NOT NULL,
                IsCorrect INTEGER NOT NULL,
                OrderIndex INTEGER NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createAnswersTable);

                // ✅ QuizAttempts table
                var createQuizAttemptsTable = @"
            CREATE TABLE QuizAttempts (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER NOT NULL,
                QuizId INTEGER NOT NULL,
                StartTime DATETIME NOT NULL,
                EndTime DATETIME,
                Score INTEGER,
                IsPassed INTEGER NOT NULL,
                IsCompleted INTEGER NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createQuizAttemptsTable);

                // ✅ QuizAnswers table
                var createQuizAnswersTable = @"
            CREATE TABLE QuizAnswers (
                Id COUNTER PRIMARY KEY,
                QuizAttemptId INTEGER NOT NULL,
                QuestionId INTEGER NOT NULL,
                SelectedAnswer MEMO,
                IsCorrect INTEGER NOT NULL,
                PointsEarned INTEGER NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createQuizAnswersTable);

                // ✅ LessonProgress table
                var createLessonProgressTable = @"
            CREATE TABLE LessonProgress (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER NOT NULL,
                LessonId INTEGER NOT NULL,
                Status INTEGER NOT NULL,
                ProgressPercentage INTEGER NOT NULL,
                TimeSpentMinutes INTEGER NOT NULL,
                StartDate DATETIME NOT NULL,
                CompletionDate DATETIME,
                LastAccessDate DATETIME
            )";
                await ExecuteNonQueryAsync(connection, createLessonProgressTable);

                // ✅ Achievements table
                var createAchievementsTable = @"
            CREATE TABLE Achievements (
                Id COUNTER PRIMARY KEY,
                Name TEXT(100) NOT NULL,
                DescriptionText MEMO NOT NULL,
                AchievementType INTEGER NOT NULL,
                Icon TEXT(10),
                Color TEXT(20),
                RequiredValue INTEGER NOT NULL,
                Points INTEGER NOT NULL,
                IsActive INTEGER NOT NULL,
                CreatedDate DATETIME NOT NULL
            )";
                await ExecuteNonQueryAsync(connection, createAchievementsTable);

                // ✅ UserAchievements table
                var createUserAchievementsTable = @"
            CREATE TABLE UserAchievements (
                Id COUNTER PRIMARY KEY,
                UserId INTEGER NOT NULL,
                AchievementId INTEGER NOT NULL,
                EarnedDate DATETIME NOT NULL,
                CurrentValue INTEGER
            )";
                await ExecuteNonQueryAsync(connection, createUserAchievementsTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tablo oluşturma hatası: {ex.Message}", "Hata");
                throw;
            }
        }


        private static async Task SeedDataAsync(OleDbConnection connection)
        {
            await SeedAdminUserAsync(connection);
            await SeedVocabularyWordsAsync(connection);
            await SeedCoursesAsync(connection);
        }

        private static async Task SeedAdminUserAsync(OleDbConnection connection)
        {
            try
            {
                // Check if admin user already exists
                var checkSql = "SELECT COUNT(*) FROM Users WHERE Email = ?";
                using var checkCommand = new OleDbCommand(checkSql, connection);
                checkCommand.Parameters.Add("?", OleDbType.VarChar, 255).Value = "admin@engotomasyon.com";
                var result = await checkCommand.ExecuteScalarAsync();

                if (result != null && (int)result > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Admin user already exists");
                    return;
                }

                // Create admin user
                var sql = @"INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, IsActive, IsAdmin, CreatedDate, LastLoginDate)
                           VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using var command = new OleDbCommand(sql, connection);
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = "admin@engotomasyon.com";
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = BCrypt.Net.BCrypt.HashPassword("admin123");
                command.Parameters.Add("?", OleDbType.VarChar, 50).Value = "Admin";
                command.Parameters.Add("?", OleDbType.VarChar, 100).Value = "System";
                command.Parameters.Add("?", OleDbType.VarChar, 100).Value = "Administrator";
                command.Parameters.Add("?", OleDbType.Integer).Value = 1; // IsActive
                command.Parameters.Add("?", OleDbType.Integer).Value = 1; // IsAdmin
                command.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                command.Parameters.Add("?", OleDbType.Date).Value = DBNull.Value; // LastLoginDate

                await command.ExecuteNonQueryAsync();
                System.Diagnostics.Debug.WriteLine("Admin user created: admin@engotomasyon.com / admin123");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating admin user: {ex.Message}");
            }
        }

        private static async Task SeedVocabularyWordsAsync(OleDbConnection connection)
        {
            // Check if vocabulary words already exist
            var checkSql = "SELECT COUNT(*) FROM VocabularyWords";
            using var checkCommand = new OleDbCommand(checkSql, connection);
            var result = await checkCommand.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;

            if (count > 0) return; // Already seeded

            var sampleWords = new[]
            {
                new { English = "Hello", Turkish = "Merhaba", Pronunciation = "/həˈloʊ/", Example = "Hello, how are you?", ExampleTr = "Merhaba, nasılsın?", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Interjection, Category = "Greetings" },
                new { English = "Goodbye", Turkish = "Hoşçakal", Pronunciation = "/ɡʊdˈbaɪ/", Example = "Goodbye, see you tomorrow!", ExampleTr = "Hoşçakal, yarın görüşürüz!", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Interjection, Category = "Greetings" },
                new { English = "Thank you", Turkish = "Teşekkür ederim", Pronunciation = "/θæŋk juː/", Example = "Thank you for your help.", ExampleTr = "Yardımın için teşekkür ederim.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Interjection, Category = "Greetings" },
                new { English = "Please", Turkish = "Lütfen", Pronunciation = "/pliːz/", Example = "Please help me.", ExampleTr = "Lütfen bana yardım et.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Adverb, Category = "Greetings" },
                new { English = "Yes", Turkish = "Evet", Pronunciation = "/jes/", Example = "Yes, I agree.", ExampleTr = "Evet, katılıyorum.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Adverb, Category = "Basic" },
                new { English = "No", Turkish = "Hayır", Pronunciation = "/noʊ/", Example = "No, I don't think so.", ExampleTr = "Hayır, öyle düşünmüyorum.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Adverb, Category = "Basic" },
                new { English = "Book", Turkish = "Kitap", Pronunciation = "/bʊk/", Example = "I am reading a book.", ExampleTr = "Bir kitap okuyorum.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Noun, Category = "Objects" },
                new { English = "Water", Turkish = "Su", Pronunciation = "/ˈwɔːtər/", Example = "I need some water.", ExampleTr = "Biraz suya ihtiyacım var.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Noun, Category = "Food & Drink" },
                new { English = "Food", Turkish = "Yemek", Pronunciation = "/fuːd/", Example = "The food is delicious.", ExampleTr = "Yemek lezzetli.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Noun, Category = "Food & Drink" },
                new { English = "House", Turkish = "Ev", Pronunciation = "/haʊs/", Example = "This is my house.", ExampleTr = "Bu benim evim.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Noun, Category = "Places" },
                new { English = "School", Turkish = "Okul", Pronunciation = "/skuːl/", Example = "I go to school every day.", ExampleTr = "Her gün okula giderim.", Difficulty = (int)WordDifficulty.Beginner, PartOfSpeech = (int)Models.PartOfSpeech.Noun, Category = "Places" },
                new { English = "Beautiful", Turkish = "Güzel", Pronunciation = "/ˈbjuːtɪfəl/", Example = "She is beautiful.", ExampleTr = "O güzel.", Difficulty = (int)WordDifficulty.Intermediate, PartOfSpeech = (int)Models.PartOfSpeech.Adjective, Category = "Adjectives" },
                new { English = "Important", Turkish = "Önemli", Pronunciation = "/ɪmˈpɔːrtənt/", Example = "This is very important.", ExampleTr = "Bu çok önemli.", Difficulty = (int)WordDifficulty.Intermediate, PartOfSpeech = (int)Models.PartOfSpeech.Adjective, Category = "Adjectives" },
                new { English = "Understand", Turkish = "Anlamak", Pronunciation = "/ˌʌndərˈstænd/", Example = "I understand you.", ExampleTr = "Seni anlıyorum.", Difficulty = (int)WordDifficulty.Intermediate, PartOfSpeech = (int)Models.PartOfSpeech.Verb, Category = "Verbs" },
                new { English = "Learn", Turkish = "Öğrenmek", Pronunciation = "/lɜːrn/", Example = "I want to learn English.", ExampleTr = "İngilizce öğrenmek istiyorum.", Difficulty = (int)WordDifficulty.Intermediate, PartOfSpeech = (int)Models.PartOfSpeech.Verb, Category = "Verbs" }
            };

            var sql = @"INSERT INTO VocabularyWords (EnglishWord, TurkishMeaning, Pronunciation, ExampleSentence, ExampleSentenceTurkish, Difficulty, PartOfSpeech, Category, AudioUrl, IsActive, CreatedDate)
                       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            foreach (var word in sampleWords)
            {
                try
                {
                    using var command = new OleDbCommand(sql, connection);

                    // EnglishWord - TEXT(255) NOT NULL
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.English;

                    // TurkishMeaning - TEXT(255) NOT NULL
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.Turkish;

                    // Pronunciation - TEXT(255) nullable
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.Pronunciation ?? (object)DBNull.Value;

                    // ExampleSentence - MEMO nullable
                    command.Parameters.Add("?", OleDbType.LongVarChar).Value = word.Example ?? (object)DBNull.Value;

                    // ExampleSentenceTurkish - MEMO nullable
                    command.Parameters.Add("?", OleDbType.LongVarChar).Value = word.ExampleTr ?? (object)DBNull.Value;

                    // Difficulty - INTEGER NOT NULL
                    command.Parameters.Add("?", OleDbType.Integer).Value = word.Difficulty;

                    // PartOfSpeech - INTEGER NOT NULL
                    command.Parameters.Add("?", OleDbType.Integer).Value = word.PartOfSpeech;

                    // Category - TEXT(100) nullable
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = word.Category ?? (object)DBNull.Value;

                    // AudioUrl - TEXT(255) nullable
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = DBNull.Value;

                    // IsActive - INTEGER NOT NULL
                    command.Parameters.Add("?", OleDbType.Integer).Value = 1;

                    // CreatedDate - DATETIME NOT NULL
                    command.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;

                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    // Log error but continue with other words
                    System.Diagnostics.Debug.WriteLine($"Error inserting word '{word.English}': {ex.Message}");
                }
            }
        }

        private static async Task SeedCoursesAsync(OleDbConnection connection)
        {
            // Check if courses already exist
            var checkSql = "SELECT COUNT(*) FROM Courses";
            using var checkCommand = new OleDbCommand(checkSql, connection);
            var result = await checkCommand.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;

            if (count > 0) return; // Already seeded

            var sampleCourses = new[]
            {
                new { Title = "English Basics", Description = "Learn basic English words and phrases", Content = "Introduction to English language fundamentals. This course covers essential vocabulary, basic grammar structures, and common phrases used in everyday communication. Perfect for beginners starting their English learning journey.", Level = (int)CourseLevel.Beginner, Type = (int)CourseType.Vocabulary, Price = 0.00, OrderIndex = 1, Duration = 30, Prerequisites = "", Color = "#4CAF50" },
                new { Title = "Grammar Fundamentals", Description = "Essential English grammar rules", Content = "Master the building blocks of English grammar. Learn about sentence structure, verb tenses, articles, prepositions, and more. This course provides a solid foundation for proper English communication.", Level = (int)CourseLevel.Beginner, Type = (int)CourseType.Grammar, Price = 0.00, OrderIndex = 2, Duration = 45, Prerequisites = "", Color = "#2196F3" },
                new { Title = "Everyday Conversations", Description = "Common phrases for daily communication", Content = "Practice speaking in everyday situations. Learn how to introduce yourself, ask for directions, order food, make appointments, and handle common social interactions with confidence.", Level = (int)CourseLevel.Intermediate, Type = (int)CourseType.Speaking, Price = 0.00, OrderIndex = 3, Duration = 60, Prerequisites = "English Basics", Color = "#FF9800" },
                new { Title = "Business English", Description = "Professional English for workplace", Content = "Develop professional communication skills for the workplace. Learn business vocabulary, email writing, presentation skills, and formal communication protocols.", Level = (int)CourseLevel.Intermediate, Type = (int)CourseType.Speaking, Price = 0.00, OrderIndex = 4, Duration = 90, Prerequisites = "Grammar Fundamentals", Color = "#9C27B0" },
                new { Title = "Advanced Vocabulary", Description = "Expand your English vocabulary", Content = "Build an extensive vocabulary with advanced words and phrases. Learn synonyms, idioms, phrasal verbs, and sophisticated expressions to enhance your English fluency.", Level = (int)CourseLevel.Advanced, Type = (int)CourseType.Vocabulary, Price = 0.00, OrderIndex = 5, Duration = 75, Prerequisites = "Everyday Conversations", Color = "#F44336" }
            };

            var sql = @"INSERT INTO Courses (Title, DescriptionText, ContentText, CourseLevel, CourseType, Price, OrderIndex, EstimatedDurationMinutes, Prerequisites, Color, IsActive, CreatedDate)
                       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

            foreach (var course in sampleCourses)
            {
                try
                {
                    using var command = new OleDbCommand(sql, connection);

                    // Title - TEXT(255) NOT NULL
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = course.Title;

                    // DescriptionText - TEXT(255) nullable
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = course.Description ?? (object)DBNull.Value;

                    // ContentText - MEMO nullable
                    command.Parameters.Add("?", OleDbType.LongVarChar).Value = course.Content ?? (object)DBNull.Value;

                    // CourseLevel - INTEGER NOT NULL
                    command.Parameters.Add("?", OleDbType.Integer).Value = course.Level;

                    // CourseType - INTEGER NOT NULL
                    command.Parameters.Add("?", OleDbType.Integer).Value = course.Type;

                    // Price - CURRENCY nullable
                    command.Parameters.Add("?", OleDbType.Currency).Value = (decimal)course.Price;

                    // OrderIndex - INTEGER nullable
                    command.Parameters.Add("?", OleDbType.Integer).Value = course.OrderIndex;

                    // EstimatedDurationMinutes - INTEGER nullable
                    command.Parameters.Add("?", OleDbType.Integer).Value = course.Duration;

                    // Prerequisites - TEXT(255) nullable
                    command.Parameters.Add("?", OleDbType.VarChar, 255).Value = string.IsNullOrEmpty(course.Prerequisites) ? (object)DBNull.Value : course.Prerequisites;

                    // Color - TEXT(20) nullable
                    command.Parameters.Add("?", OleDbType.VarChar, 20).Value = course.Color ?? (object)DBNull.Value;

                    // IsActive - INTEGER NOT NULL
                    command.Parameters.Add("?", OleDbType.Integer).Value = 1;

                    // CreatedDate - DATETIME NOT NULL
                    command.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;

                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    // Log error but continue with other courses
                    System.Diagnostics.Debug.WriteLine($"Error inserting course '{course.Title}': {ex.Message}");
                }
            }
        }

        private static async Task ExecuteNonQueryAsync(OleDbConnection connection, string sql)
        {
            try
            {
                using var command = new OleDbCommand(sql, connection);
                await command.ExecuteNonQueryAsync();
                System.Diagnostics.Debug.WriteLine("SQL executed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SQL execution failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"SQL: {sql}");
                throw;
            }
        }

        // User operations
        public static async Task<User?> GetUserByEmailAsync(string email)
        {
            var connectionString = GetConnectionString();
            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Users WHERE Email = ? AND IsActive = 1";
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
                    Role = reader.GetString(3), // Role
                    FirstName = reader.IsDBNull(4) ? null : reader.GetString(4), // FirstName
                    LastName = reader.IsDBNull(5) ? null : reader.GetString(5), // LastName
                    IsActive = reader.GetInt32(6) == 1, // IsActive
                    IsAdmin = reader.GetInt32(7) == 1, // IsAdmin
                    CreatedDate = reader.GetDateTime(8), // CreatedDate
                    LastLoginDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9) // LastLoginDate
                };
            }
            return null;
        }

        public static async Task<bool> CreateUserAsync(User user)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreateUserAsync called for: {user.Email}");
                System.Diagnostics.Debug.WriteLine($"User data: FirstName={user.FirstName}, LastName={user.LastName}, Role={user.Role}");

                var connectionString = GetConnectionString();
                System.Diagnostics.Debug.WriteLine($"Connection string: {connectionString}");

                using var connection = new OleDbConnection(connectionString);
                System.Diagnostics.Debug.WriteLine("Opening connection...");
                await connection.OpenAsync();
                System.Diagnostics.Debug.WriteLine("Connection opened successfully");

                // SQL sorgusunu tablodaki field sırası ile eşleştir (Id hariç - AUTO INCREMENT)
                var sql = @"INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName, IsActive, IsAdmin, CreatedDate, LastLoginDate)
                           VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";

                System.Diagnostics.Debug.WriteLine($"SQL: {sql}");
                System.Diagnostics.Debug.WriteLine($"Parameters: Email='{user.Email}', Role='{user.Role}', FirstName='{user.FirstName}', LastName='{user.LastName}', IsActive={user.IsActive}, IsAdmin={user.IsAdmin}");

                using var command = new OleDbCommand(sql, connection);

                // Email - TEXT(255)
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = user.Email ?? "";

                // PasswordHash - TEXT(255)
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = user.PasswordHash ?? "";

                // Role - TEXT(50)
                command.Parameters.Add("?", OleDbType.VarChar, 50).Value = user.Role ?? "User";

                // FirstName - TEXT(100) nullable
                if (string.IsNullOrWhiteSpace(user.FirstName))
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = DBNull.Value;
                else
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = user.FirstName;

                // LastName - TEXT(100) nullable
                if (string.IsNullOrWhiteSpace(user.LastName))
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = DBNull.Value;
                else
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = user.LastName;

                // IsActive - INTEGER
                command.Parameters.Add("?", OleDbType.Integer).Value = user.IsActive ? 1 : 0;

                // IsAdmin - INTEGER
                command.Parameters.Add("?", OleDbType.Integer).Value = user.IsAdmin ? 1 : 0;

                // CreatedDate - DATETIME
                command.Parameters.Add("?", OleDbType.Date).Value = user.CreatedDate;

                // LastLoginDate - DATETIME nullable
                if (user.LastLoginDate.HasValue)
                    command.Parameters.Add("?", OleDbType.Date).Value = user.LastLoginDate.Value;
                else
                    command.Parameters.Add("?", OleDbType.Date).Value = DBNull.Value;

                System.Diagnostics.Debug.WriteLine("Executing SQL...");
                var rowsAffected = await command.ExecuteNonQueryAsync();
                System.Diagnostics.Debug.WriteLine($"Rows affected: {rowsAffected}");
                System.Diagnostics.Debug.WriteLine("User created successfully");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating user: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public static async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var users = new List<User>();
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                var sql = "SELECT * FROM Users WHERE Role = 'User' ORDER BY CreatedDate DESC";
                using var command = new OleDbCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0), // Id
                        Email = reader.GetString(1), // Email
                        PasswordHash = reader.GetString(2), // PasswordHash
                        Role = reader.GetString(3), // Role
                        FirstName = reader.IsDBNull(4) ? null : reader.GetString(4), // FirstName
                        LastName = reader.IsDBNull(5) ? null : reader.GetString(5), // LastName
                        IsActive = reader.GetInt32(6) == 1, // IsActive
                        IsAdmin = reader.GetInt32(7) == 1, // IsAdmin
                        CreatedDate = reader.GetDateTime(8), // CreatedDate
                        LastLoginDate = reader.IsDBNull(9) ? null : reader.GetDateTime(9) // LastLoginDate
                    });
                }
                return users;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting users: {ex.Message}");
                return new List<User>();
            }
        }

        public static async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                using var connection = new OleDbConnection(GetConnectionString());
                await connection.OpenAsync();

                var sql = "DELETE FROM Users WHERE Id = ?";
                using var command = new OleDbCommand(sql, connection);
                command.Parameters.Add("?", OleDbType.Integer).Value = userId;

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                using var connection = new OleDbConnection(GetConnectionString());
                await connection.OpenAsync();

                var sql = @"UPDATE Users SET
                           Email = ?,
                           Role = ?,
                           FirstName = ?,
                           LastName = ?,
                           IsActive = ?,
                           IsAdmin = ?
                           WHERE Id = ?";

                using var command = new OleDbCommand(sql, connection);

                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = user.Email ?? "";
                command.Parameters.Add("?", OleDbType.VarChar, 50).Value = user.Role ?? "User";

                if (string.IsNullOrWhiteSpace(user.FirstName))
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = DBNull.Value;
                else
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = user.FirstName;

                if (string.IsNullOrWhiteSpace(user.LastName))
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = DBNull.Value;
                else
                    command.Parameters.Add("?", OleDbType.VarChar, 100).Value = user.LastName;

                command.Parameters.Add("?", OleDbType.Integer).Value = user.IsActive ? 1 : 0;
                command.Parameters.Add("?", OleDbType.Integer).Value = user.IsAdmin ? 1 : 0;
                command.Parameters.Add("?", OleDbType.Integer).Value = user.Id;

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating user: {ex.Message}");
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

            var sql = "SELECT * FROM Courses WHERE IsActive = 1 ORDER BY OrderIndex";
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
                    IsActive = reader.GetInt32(11) == 1, // IsActive
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

            var sql = "SELECT * FROM VocabularyWords WHERE IsActive = 1 ORDER BY EnglishWord";
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
                    IsActive = reader.GetInt32(10) == 1, // IsActive
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

                // EnglishWord - TEXT(255) NOT NULL
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.EnglishWord;

                // TurkishMeaning - TEXT(255) NOT NULL
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.TurkishMeaning;

                // Pronunciation - TEXT(255) nullable
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.Pronunciation ?? (object)DBNull.Value;

                // ExampleSentence - MEMO nullable
                command.Parameters.Add("?", OleDbType.LongVarChar).Value = word.ExampleSentence ?? (object)DBNull.Value;

                // ExampleSentenceTurkish - MEMO nullable
                command.Parameters.Add("?", OleDbType.LongVarChar).Value = word.ExampleSentenceTurkish ?? (object)DBNull.Value;

                // Difficulty - INTEGER NOT NULL
                command.Parameters.Add("?", OleDbType.Integer).Value = (int)word.Difficulty;

                // PartOfSpeech - INTEGER NOT NULL
                command.Parameters.Add("?", OleDbType.Integer).Value = (int)word.PartOfSpeech;

                // Category - TEXT(100) nullable
                command.Parameters.Add("?", OleDbType.VarChar, 100).Value = word.Category ?? (object)DBNull.Value;

                // AudioUrl - TEXT(255) nullable
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.AudioUrl ?? (object)DBNull.Value;

                // IsActive - INTEGER NOT NULL
                command.Parameters.Add("?", OleDbType.Integer).Value = word.IsActive ? 1 : 0;

                // CreatedDate - DATETIME NOT NULL
                command.Parameters.Add("?", OleDbType.Date).Value = word.CreatedDate;

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
                checkCommand.Parameters.Add("?", OleDbType.Integer).Value = progress.UserId;
                checkCommand.Parameters.Add("?", OleDbType.Integer).Value = progress.CourseId;
                var result = await checkCommand.ExecuteScalarAsync();
                var exists = result != null && (int)result > 0;

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
                    // UPDATE parameters
                    command.Parameters.Add("?", OleDbType.Integer).Value = (int)progress.Status;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.ProgressPercentage;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.TimeSpentMinutes;
                    command.Parameters.Add("?", OleDbType.Date).Value = progress.CompletionDate ?? (object)DBNull.Value;
                    command.Parameters.Add("?", OleDbType.Date).Value = progress.LastAccessDate ?? (object)DBNull.Value;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.UserId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.CourseId;
                }
                else
                {
                    // INSERT parameters
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.UserId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.CourseId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = (int)progress.Status;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.ProgressPercentage;
                    command.Parameters.Add("?", OleDbType.Integer).Value = progress.TimeSpentMinutes;
                    command.Parameters.Add("?", OleDbType.Date).Value = progress.StartDate;
                    command.Parameters.Add("?", OleDbType.Date).Value = progress.CompletionDate ?? (object)DBNull.Value;
                    command.Parameters.Add("?", OleDbType.Date).Value = progress.LastAccessDate ?? (object)DBNull.Value;
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
                checkCommand.Parameters.Add("?", OleDbType.Integer).Value = userVocab.UserId;
                checkCommand.Parameters.Add("?", OleDbType.Integer).Value = userVocab.VocabularyWordId;
                var result = await checkCommand.ExecuteScalarAsync();
                var exists = result != null && (int)result > 0;

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
                    // UPDATE parameters
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.MasteryLevel;
                    command.Parameters.Add("?", OleDbType.Date).Value = userVocab.LastReviewedDate ?? (object)DBNull.Value;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.ReviewCount;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.CorrectAnswers;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.TotalAttempts;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.UserId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.VocabularyWordId;
                }
                else
                {
                    // INSERT parameters
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.UserId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.VocabularyWordId;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.MasteryLevel;
                    command.Parameters.Add("?", OleDbType.Date).Value = userVocab.FirstLearnedDate;
                    command.Parameters.Add("?", OleDbType.Date).Value = userVocab.LastReviewedDate ?? (object)DBNull.Value;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.ReviewCount;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.CorrectAnswers;
                    command.Parameters.Add("?", OleDbType.Integer).Value = userVocab.TotalAttempts;
                }

                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> DeleteVocabularyWordAsync(int wordId)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                // First delete related user vocabulary records
                var deleteUserVocabSql = "DELETE FROM UserVocabulary WHERE VocabularyWordId = ?";
                using var deleteUserVocabCommand = new OleDbCommand(deleteUserVocabSql, connection);
                deleteUserVocabCommand.Parameters.Add("?", OleDbType.Integer).Value = wordId;
                await deleteUserVocabCommand.ExecuteNonQueryAsync();

                // Then delete the vocabulary word
                var deleteWordSql = "DELETE FROM VocabularyWords WHERE Id = ?";
                using var deleteWordCommand = new OleDbCommand(deleteWordSql, connection);
                deleteWordCommand.Parameters.Add("?", OleDbType.Integer).Value = wordId;

                var rowsAffected = await deleteWordCommand.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> UpdateVocabularyWordAsync(VocabularyWord word)
        {
            try
            {
                var connectionString = GetConnectionString();
                using var connection = new OleDbConnection(connectionString);
                await connection.OpenAsync();

                var sql = @"UPDATE VocabularyWords SET
                           EnglishWord = ?, TurkishMeaning = ?, Pronunciation = ?,
                           ExampleSentence = ?, ExampleSentenceTurkish = ?,
                           Difficulty = ?, PartOfSpeech = ?, Category = ?,
                           AudioUrl = ?, IsActive = ?
                           WHERE Id = ?";

                using var command = new OleDbCommand(sql, connection);

                // Add parameters in the same order as the SQL
                command.Parameters.Add("?", OleDbType.VarChar, 100).Value = word.EnglishWord;
                command.Parameters.Add("?", OleDbType.VarChar, 200).Value = word.TurkishMeaning;
                command.Parameters.Add("?", OleDbType.VarChar, 100).Value = word.Pronunciation ?? (object)DBNull.Value;
                command.Parameters.Add("?", OleDbType.LongVarChar).Value = word.ExampleSentence ?? (object)DBNull.Value;
                command.Parameters.Add("?", OleDbType.LongVarChar).Value = word.ExampleSentenceTurkish ?? (object)DBNull.Value;
                command.Parameters.Add("?", OleDbType.Integer).Value = (int)word.Difficulty;
                command.Parameters.Add("?", OleDbType.Integer).Value = (int)word.PartOfSpeech;
                command.Parameters.Add("?", OleDbType.VarChar, 100).Value = word.Category ?? (object)DBNull.Value;
                command.Parameters.Add("?", OleDbType.VarChar, 255).Value = word.AudioUrl ?? (object)DBNull.Value;
                command.Parameters.Add("?", OleDbType.Integer).Value = word.IsActive ? 1 : 0;
                command.Parameters.Add("?", OleDbType.Integer).Value = word.Id;

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch
            {
                return false;
            }
        }

    }
}
