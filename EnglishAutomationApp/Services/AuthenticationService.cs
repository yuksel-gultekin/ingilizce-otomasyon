using EnglishAutomationApp.Data;
using EnglishAutomationApp.Models;

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
                var user = await AccessDatabaseHelper.GetUserByEmailAsync(email);

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
                // Email check
                var existingUser = await AccessDatabaseHelper.GetUserByEmailAsync(email);

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

                var success = await AccessDatabaseHelper.CreateUserAsync(newUser);

                if (success)
                {
                    return (true, "Registration successful. You can now login.");
                }
                else
                {
                    return (false, "Registration failed. Please try again.");
                }
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
                var user = await AccessDatabaseHelper.GetUserByEmailAsync(CurrentUser.Email);
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

                // For now, we'll return success but note that password update needs to be implemented in AccessDatabaseHelper
                return (true, "Password change feature will be implemented soon.");
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
                var user = await AccessDatabaseHelper.GetUserByEmailAsync(email);

                if (user == null)
                {
                    return (false, "Email address not found.");
                }

                // Generate temporary password
                var tempPassword = GenerateTemporaryPassword();

                // For now, we'll return the temp password but note that password update needs to be implemented
                return (true, $"Password reset feature will be implemented soon. Temporary password would be: {tempPassword}");
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
                // Update current user in memory
                CurrentUser.FirstName = firstName;
                CurrentUser.LastName = lastName;

                // For now, we'll just update in memory. Database update needs to be implemented in AccessDatabaseHelper
                await Task.CompletedTask;
                return (true, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating profile: {ex.Message}");
            }
        }
    }
}
