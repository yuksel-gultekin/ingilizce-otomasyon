# English Automation Platform 🌟

This project is a modern and comprehensive desktop application developed for those who want to learn English automation. It is developed using C# Windows Forms technology and provides a user-friendly interface with an Access-like database system.

## 🚀 Features

### ✅ Functional Requirements

#### 1. User Management
- ✅ User registration system (email registration/password setting)
- ✅ Login (authentication)
- ✅ Password reset mechanism
- ✅ Profile management

#### 2. Modern UI/UX
- ✅ Modern Windows Forms design
- ✅ Card-based layout
- ✅ Responsive design
- ✅ User-friendly interface

#### 3. English Automation Learning System
- ✅ Level-based courses (Beginner, Intermediate, Advanced)
- ✅ Course types (Grammar, Vocabulary, Speaking, Listening, Reading, Writing, Pronunciation)
- ✅ Interactive lessons and quizzes
- ✅ Vocabulary learning system
- ✅ Pronunciation practice
- ✅ Progress tracking

#### 4. Course Management
- ✅ Course creation and editing
- ✅ Lesson organization
- ✅ Content management
- ✅ Course categorization by level and type

#### 5. Assessment System
- ✅ Interactive quizzes
- ✅ Multiple choice questions
- ✅ Automatic scoring
- ✅ Progress evaluation

#### 6. Achievement System
- ✅ Learning milestones
- ✅ Badge collection
- ✅ Progress rewards
- ✅ Motivation system

#### 7. Payment System
- ✅ Course purchase functionality
- ✅ Payment history display
- ✅ Transaction numbers and detailed payment information

#### 8. Dashboard
- ✅ Modern panel showing user progress
- ✅ Course completion statistics
- ✅ Vocabulary learning statistics
- ✅ Recent activities list
- ✅ Quick access buttons
- ✅ Motivational messages

#### 9. Database Management (Access-like)
- ✅ File-based database system
- ✅ Database backup and restore
- ✅ Database compacting
- ✅ Database information display
- ✅ Admin panel for database management

## 🛠️ Technologies

- **Framework**: .NET 9.0
- **UI Framework**: Windows Forms
- **Database**: SQLite (Access-like file-based database)
- **Encryption**: BCrypt.Net
- **Language**: C# 12.0
- **Platform**: Windows Desktop

## 📁 Project Structure

```
EnglishAutomationApp/
├── Data/
│   └── AppDbContext.cs          # Database context
├── Models/                      # Data models
│   ├── User.cs
│   ├── Course.cs
│   ├── Payment.cs
│   ├── UserProgress.cs
│   ├── Achievement.cs
│   ├── Lesson.cs
│   ├── Quiz.cs
│   ├── Vocabulary.cs
│   └── LessonProgress.cs
├── Services/                    # Business logic
│   └── AuthenticationService.cs
├── Views/                       # UI Forms
│   ├── LoginForm.cs
│   ├── MainForm.cs
│   ├── RegisterForm.cs
│   └── Pages/                   # User Controls
│       ├── DashboardUserControl.cs
│       ├── CoursesUserControl.cs
│       ├── VocabularyUserControl.cs
│       ├── ProgressUserControl.cs
│       ├── AchievementsUserControl.cs
│       ├── PaymentsUserControl.cs
│       └── AdminUserControl.cs
├── Helpers/                     # Utility classes
│   ├── ErrorHandler.cs
│   └── AccessDatabaseHelper.cs  # Database management
├── Program.cs                   # Application entry point
└── app.manifest                 # Application manifest
```

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Windows 10/11

### Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/english-automation-platform.git
cd english-automation-platform
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run --project EnglishAutomationApp
```

### Default Admin Account
- **Email**: admin@englishautomation.com
- **Password**: admin123

## 📊 Database (Access-like)

The application uses SQLite as a file-based database, providing Access-like functionality without requiring Microsoft Access installation. The database file is automatically created in:
```
%LocalAppData%\EnglishAutomationApp\database.db
```

### Database Features
- **File-based**: Single database file, easy to backup and share
- **Portable**: No server installation required
- **Access-like**: Similar functionality to Microsoft Access
- **Backup/Restore**: Built-in backup and restore functionality
- **Compact**: Database compacting to optimize size
- **Admin Tools**: Database management through admin panel

## 🎯 Key Features

### Student Features
- **Course Enrollment**: Browse and enroll in English automation courses
- **Interactive Learning**: Engage with lessons, quizzes, and vocabulary exercises
- **Progress Tracking**: Monitor learning progress and achievements
- **Dashboard**: View personalized learning statistics and recent activities

### Admin Features
- **User Management**: Manage student accounts and permissions
- **Course Management**: Create, edit, and organize learning content
- **Database Management**: Backup, restore, and compact database
- **Analytics**: View system-wide statistics and user progress
- **Content Creation**: Add new lessons, quizzes, and vocabulary

## 🔧 Development

### Adding New Features
1. Create models in the `Models/` folder
2. Update `AppDbContext.cs` with new DbSets
3. Create services in the `Services/` folder
4. Design UI in the `Views/` folder
5. Run database migrations if needed

### Database Migrations
```bash
dotnet ef migrations add MigrationName --project EnglishAutomationApp
dotnet ef database update --project EnglishAutomationApp
```

### Database Management
The application includes built-in database management tools:
- **Backup**: Create database backups to desktop
- **Restore**: Restore from backup files
- **Compact**: Optimize database size
- **Info**: View database statistics and information

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

1. Fork the project
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📞 Support

For support and questions, please contact:
- Email: support@englishautomation.com

## 🎉 Acknowledgments

- Built with ❤️ for English automation learners worldwide
- Uses SQLite for Access-like database functionality
- Designed as a student-friendly desktop application
