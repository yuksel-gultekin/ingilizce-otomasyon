using Microsoft.EntityFrameworkCore;
using EnglishAutomationApp.Models;
using System.IO;

namespace EnglishAutomationApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }

        public DbSet<UserProgress> UserProgresses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<VocabularyWord> VocabularyWords { get; set; }
        public DbSet<UserVocabulary> UserVocabularies { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }

        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Access database path - Microsoft Access database file
                var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                        "EnglishAutomationApp", "database.accdb");

                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(dbPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                // Access connection string using OLE DB provider
                var connectionString = $@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath};";

                // Note: For Access database, we'll use SQLite as EF Core doesn't directly support Access
                // This provides similar file-based database functionality
                var sqliteDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                               "EnglishAutomationApp", "database.db");
                optionsBuilder.UseSqlite($"Data Source={sqliteDbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).HasDefaultValue("User");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
            });

            // Course entity configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.Level).HasConversion<int>();
            });



            // UserProgress entity configuration
            modelBuilder.Entity<UserProgress>(entity =>
            {
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.ProgressPercentage).HasDefaultValue(0);
                entity.Property(e => e.TimeSpentMinutes).HasDefaultValue(0);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserProgresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.UserProgresses)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                // A user can only register for a course once
                entity.HasIndex(e => new { e.UserId, e.CourseId }).IsUnique();
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // English automation courses
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Title = "Basic English Grammar",
                    Description = "Fundamental rules and structures of English grammar",
                    Content = "In this course you will learn the basics of English grammar...",
                    Level = CourseLevel.Beginner,
                    Type = CourseType.Grammar,
                    Price = 99.99m,
                    OrderIndex = 1,
                    EstimatedDurationMinutes = 120,
                    Prerequisites = "No prior knowledge required",
                    Color = "#4CAF50"
                },
                new Course
                {
                    Id = 2,
                    Title = "Daily Life Vocabulary",
                    Description = "Essential English words used in daily life",
                    Content = "In this course you will learn the most commonly used English words in daily life...",
                    Level = CourseLevel.Beginner,
                    Type = CourseType.Vocabulary,
                    Price = 79.99m,
                    OrderIndex = 2,
                    EstimatedDurationMinutes = 90,
                    Prerequisites = "No prior knowledge required",
                    Color = "#2196F3"
                },
                new Course
                {
                    Id = 3,
                    Title = "English Speaking Practice",
                    Description = "Basic speaking skills and pronunciation exercises",
                    Content = "In this course you will develop your English speaking skills and improve your pronunciation...",
                    Level = CourseLevel.Intermediate,
                    Type = CourseType.Speaking,
                    Price = 149.99m,
                    OrderIndex = 3,
                    EstimatedDurationMinutes = 180,
                    Prerequisites = "Complete Basic English Grammar course",
                    Color = "#FF9800"
                }
            );

            // Achievements
            modelBuilder.Entity<Achievement>().HasData(
                new Achievement
                {
                    Id = 1,
                    Name = "First Steps",
                    Description = "Complete your first lesson",
                    Type = AchievementType.LessonCompletion,
                    Icon = "🎯",
                    Color = "#4CAF50",
                    RequiredValue = 1,
                    Points = 10
                },
                new Achievement
                {
                    Id = 2,
                    Name = "Word Master",
                    Description = "Learn 50 vocabulary words",
                    Type = AchievementType.VocabularyMastery,
                    Icon = "📖",
                    Color = "#2196F3",
                    RequiredValue = 50,
                    Points = 50
                },
                new Achievement
                {
                    Id = 3,
                    Name = "Course Completer",
                    Description = "Complete your first course",
                    Type = AchievementType.CourseCompletion,
                    Icon = "🏆",
                    Color = "#FF9800",
                    RequiredValue = 1,
                    Points = 100
                }
            );
        }
    }
}
