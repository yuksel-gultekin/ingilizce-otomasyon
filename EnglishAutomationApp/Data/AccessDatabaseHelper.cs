using System;
using System.Collections.Generic;
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
            var dbPath = @"C:\Users\Administrator\Documents\Database2.accdb";
            return $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};Persist Security Info=False;";
        }

        public static async Task InitializeDatabaseAsync()
        {
            var connectionString = GetConnectionString();

            // Dosya var mı?
            if (!File.Exists(@"C:\Users\Administrator\Documents\Database2.accdb"))
            {
                System.Windows.Forms.MessageBox.Show("Access database bulunamadı!", "Hata");
                return;
            }

            using var connection = new OleDbConnection(connectionString);
            await connection.OpenAsync();

            // Gerekirse tablo yoksa oluştur
            await EnsureTablesExistAsync(connection);

            System.Windows.Forms.MessageBox.Show("Veritabanı bağlantısı başarılı!", "Bilgi");
        }

        private static async Task EnsureTablesExistAsync(OleDbConnection connection)
        {
            // Basit kontrol: "Users" tablosu var mı
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
                System.Windows.Forms.MessageBox.Show("'Users' tablosu bulunamadı. Oluşturuluyor...", "Bilgi");
                await CreateTablesAsync(connection);
                await SeedDataAsync(connection);
                System.Windows.Forms.MessageBox.Show("Tablolar oluşturuldu ve örnek veri eklendi.", "Bilgi");
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

            var createUserProgressTable = @"
                CREATE TABLE UserProgress (
                    Id AUTOINCREMENT PRIMARY KEY,
                    UserId INTEGER NOT NULL,
                    CourseId INTEGER NOT NULL,
                    Status INTEGER DEFAULT 0,
                    ProgressPercentage INTEGER DEFAULT 0,
                    TimeSpentMinutes INTEGER DEFAULT 0,
                    StartDate DATETIME,
                    CompletionDate DATETIME
                )";
            await ExecuteNonQueryAsync(connection, createUserProgressTable);

            var createUserVocabularyTable = @"
                CREATE TABLE UserVocabulary (
                    Id AUTOINCREMENT PRIMARY KEY,
                    UserId INTEGER NOT NULL,
                    VocabularyWordId INTEGER NOT NULL,
                    IsLearned YESNO DEFAULT False,
                    CorrectAnswers INTEGER DEFAULT 0,
                    IncorrectAnswers INTEGER DEFAULT 0,
                    LastPracticeDate DATETIME
                )";
            await ExecuteNonQueryAsync(connection, createUserVocabularyTable);
        }

        private static async Task SeedDataAsync(OleDbConnection connection)
        {
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
        }

        private static async Task ExecuteNonQueryAsync(OleDbConnection connection, string sql)
        {
            using var command = new OleDbCommand(sql, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
