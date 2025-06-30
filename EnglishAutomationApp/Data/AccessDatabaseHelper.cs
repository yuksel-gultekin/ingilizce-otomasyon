using System;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms; // Forms kullanıyorsan

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
                var cat = new ADOX.Catalog();
                cat.Create($@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ADOX ile oluşturma hatası: {ex.Message}", "Hata");
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

            // Diğer tabloları da ekle
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
    }
}
