using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishAutomationApp.Services
{
    public class AuthenticationService
    {
        private static User? _currentUser;

        public static User? CurrentUser
        {
            get => _currentUser;
            private set => _currentUser = value;
        }

        public static bool IsLoggedIn => CurrentUser != null;
        public static bool IsAdmin => CurrentUser?.IsAdmin == true;

        public static async Task<(bool Success, string Message)> LoginAsync(string email, string password)
        {
            try
            {
                using var context = new AppDbContext();
                
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return (false, "Email address not found.");
                }

                if (!user.IsActive)
                {
                    return (false, "Your account is deactivated. Please contact administrator.");
                }

                if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    return (false, "Incorrect password.");
                }

                // Successful login
                user.LastLoginDate = DateTime.Now;
                await context.SaveChangesAsync();

                CurrentUser = user;
                return (true, "Login successful.");
            }
            catch (Exception ex)
            {
                return (false, $"Login error occurred: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> RegisterAsync(string email, string password, string firstName, string lastName)
        {
            try
            {
                using var context = new AppDbContext();
                
                // Email check
                var existingUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (existingUser != null)
                {
                    return (false, "This email address is already in use.");
                }

                // Password validation
                if (password.Length < 6)
                {
                    return (false, "Password must be at least 6 characters long.");
                }

                // Create new user
                var newUser = new User
                {
                    Email = email.ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    FirstName = firstName,
                    LastName = lastName,
                    Role = "User",
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                return (true, "Registration successful. You can now login.");
            }
            catch (Exception ex)
            {
                return (false, $"Registration error occurred: {ex.Message}");
            }
        }

        public static async Task<(bool Success, string Message)> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            if (CurrentUser == null)
            {
                return (false, "Not logged in.");
            }

            try
            {
                using var context = new AppDbContext();
                
                var user = await context.Users.FindAsync(CurrentUser.Id);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    return (false, "Current password is incorrect.");
                }

                if (newPassword.Length < 6)
                {
                    return (false, "New password must be at least 6 characters long.");
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await context.SaveChangesAsync();

                return (true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error changing password: {ex.Message}");
            }
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static async Task<(bool Success, string Message)> ResetPasswordAsync(string email)
        {
            try
            {
                using var context = new AppDbContext();
                
                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user == null)
                {
                    return (false, "Email address not found.");
                }

                // Generate temporary password
                var tempPassword = GenerateTemporaryPassword();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(tempPassword);
                await context.SaveChangesAsync();

                // In a real application, you would send this via email
                return (true, $"Temporary password: {tempPassword}");
            }
            catch (Exception ex)
            {
                return (false, $"Error resetting password: {ex.Message}");
            }
        }

        private static string GenerateTemporaryPassword()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static async Task<(bool Success, string Message)> UpdateProfileAsync(string firstName, string lastName)
        {
            if (CurrentUser == null)
            {
                return (false, "Not logged in.");
            }

            try
            {
                using var context = new AppDbContext();
                
                var user = await context.Users.FindAsync(CurrentUser.Id);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                user.FirstName = firstName;
                user.LastName = lastName;
                await context.SaveChangesAsync();

                // Update current user
                CurrentUser.FirstName = firstName;
                CurrentUser.LastName = lastName;

                return (true, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating profile: {ex.Message}");
            }
        }
    }
}
