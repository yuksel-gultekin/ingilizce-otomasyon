using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Data;
using EnglishAutomationApp.Helpers;
using EnglishAutomationApp.Models;

namespace EnglishAutomationApp.Views.Pages
{
    public partial class AdminUserControl : UserControl
    {
        private Panel headerPanel = null!;
        private Panel toolbarPanel = null!;
        private Panel contentPanel = null!;
        private DataGridView usersDataGridView = null!;
        private Button addUserButton = null!;
        private Button refreshButton = null!;
        private TextBox searchTextBox = null!;
        private Label statsLabel = null!;

        private List<User> allUsers = new List<User>();
        private bool isEnglish = true;

        public AdminUserControl()
        {
            InitializeComponent();
            LoadUsersAsync();
        }

        public void SetLanguage(bool english)
        {
            isEnglish = english;
            UpdateLanguage();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // UserControl properties
            this.BackColor = ModernUIHelper.Colors.Background;
            this.Dock = DockStyle.Fill;

            CreateHeaderPanel();
            CreateToolbarPanel();
            CreateContentPanel();

            // Add controls to UserControl
            this.Controls.Add(contentPanel);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);

            this.ResumeLayout(false);
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                BackColor = ModernUIHelper.Colors.Background,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            var titleLabel = ModernUIHelper.CreateHeading("👥 User Management", 2);
            titleLabel.Location = new Point(ModernUIHelper.Spacing.Large, ModernUIHelper.Spacing.Medium);

            statsLabel = ModernUIHelper.CreateBodyText("Loading users...", true);
            statsLabel.Location = new Point(ModernUIHelper.Spacing.Large, 50);

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(statsLabel);
        }

        private void CreateToolbarPanel()
        {
            toolbarPanel = new Panel
            {
                BackColor = ModernUIHelper.Colors.Surface,
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            // Search box
            searchTextBox = new TextBox
            {
                Font = ModernUIHelper.Fonts.Body,
                Location = new Point(ModernUIHelper.Spacing.Large, 20),
                Size = new Size(300, 30),
                BackColor = ModernUIHelper.Colors.SurfaceVariant,
                ForeColor = ModernUIHelper.Colors.OnSurface,
                BorderStyle = BorderStyle.FixedSingle
            };
            searchTextBox.PlaceholderText = "Search users by name or email...";
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            // Add user button
            addUserButton = ModernUIHelper.CreateIconButton("+ Add User", "👤", ModernUIHelper.Colors.Primary, 130);
            addUserButton.Location = new Point(350, 20);
            addUserButton.Click += AddUserButton_Click;

            // Refresh button
            refreshButton = ModernUIHelper.CreateIconButton("🔄 Refresh", "↻", ModernUIHelper.Colors.Secondary, 120);
            refreshButton.Location = new Point(500, 20);
            refreshButton.Click += RefreshButton_Click;

            toolbarPanel.Controls.Add(searchTextBox);
            toolbarPanel.Controls.Add(addUserButton);
            toolbarPanel.Controls.Add(refreshButton);
        }

        private void CreateContentPanel()
        {
            contentPanel = new Panel
            {
                BackColor = ModernUIHelper.Colors.Background,
                Dock = DockStyle.Fill,
                Padding = new Padding(ModernUIHelper.Spacing.Large)
            };

            // Users DataGridView
            usersDataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = ModernUIHelper.Colors.Surface,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                GridColor = ModernUIHelper.Colors.SurfaceVariant,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false
            };

            // Style the DataGridView
            usersDataGridView.DefaultCellStyle.BackColor = ModernUIHelper.Colors.Surface;
            usersDataGridView.DefaultCellStyle.ForeColor = ModernUIHelper.Colors.OnSurface;
            usersDataGridView.DefaultCellStyle.SelectionBackColor = ModernUIHelper.Colors.Primary;
            usersDataGridView.DefaultCellStyle.SelectionForeColor = Color.White;
            usersDataGridView.DefaultCellStyle.Font = ModernUIHelper.Fonts.Body;

            usersDataGridView.ColumnHeadersDefaultCellStyle.BackColor = ModernUIHelper.Colors.SurfaceVariant;
            usersDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = ModernUIHelper.Colors.OnSurface;
            usersDataGridView.ColumnHeadersDefaultCellStyle.Font = ModernUIHelper.Fonts.BodyBold;

            contentPanel.Controls.Add(usersDataGridView);
        }

        private async void LoadUsersAsync()
        {
            try
            {
                allUsers = await AccessDatabaseHelper.GetAllUsersAsync();
                DisplayUsers();
                UpdateStatsLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayUsers()
        {
            usersDataGridView.DataSource = null;
            usersDataGridView.Columns.Clear();

            if (!allUsers.Any())
            {
                statsLabel.Text = "No users found.";
                return;
            }

            // Create columns
            usersDataGridView.Columns.Add("Id", "ID");
            usersDataGridView.Columns.Add("Email", "Email");
            usersDataGridView.Columns.Add("FirstName", "First Name");
            usersDataGridView.Columns.Add("LastName", "Last Name");
            usersDataGridView.Columns.Add("Role", "Role");
            usersDataGridView.Columns.Add("IsActive", "Active");
            usersDataGridView.Columns.Add("IsAdmin", "Admin");
            usersDataGridView.Columns.Add("CreatedDate", "Created Date");
            usersDataGridView.Columns.Add("LastLoginDate", "Last Login");

            // Add action columns
            var editColumn = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Edit",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            usersDataGridView.Columns.Add(editColumn);

            var deleteColumn = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "Delete",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            usersDataGridView.Columns.Add(deleteColumn);

            // Populate data
            foreach (var user in allUsers)
            {
                usersDataGridView.Rows.Add(
                    user.Id,
                    user.Email,
                    user.FirstName ?? "",
                    user.LastName ?? "",
                    user.Role,
                    user.IsActive ? "Yes" : "No",
                    user.IsAdmin ? "Yes" : "No",
                    user.CreatedDate.ToString("yyyy-MM-dd"),
                    user.LastLoginDate?.ToString("yyyy-MM-dd") ?? "Never"
                );
            }

            // Handle button clicks
            usersDataGridView.CellClick += UsersDataGridView_CellClick;
        }

        private void UpdateStatsLabel()
        {
            var totalUsers = allUsers.Count;
            var activeUsers = allUsers.Count(u => u.IsActive);
            var adminUsers = allUsers.Count(u => u.IsAdmin);

            if (isEnglish)
            {
                statsLabel.Text = $"Total: {totalUsers} users | Active: {activeUsers} | Admins: {adminUsers}";
            }
            else
            {
                statsLabel.Text = $"Toplam: {totalUsers} kullanıcı | Aktif: {activeUsers} | Yönetici: {adminUsers}";
            }
        }

        private void SearchTextBox_TextChanged(object? sender, EventArgs e)
        {
            var searchTerm = searchTextBox.Text.ToLower();
            var filteredUsers = allUsers.Where(u =>
                u.Email.ToLower().Contains(searchTerm) ||
                (u.FirstName?.ToLower().Contains(searchTerm) ?? false) ||
                (u.LastName?.ToLower().Contains(searchTerm) ?? false)
            ).ToList();

            // Update display with filtered users
            DisplayFilteredUsers(filteredUsers);
        }

        private void DisplayFilteredUsers(List<User> users)
        {
            usersDataGridView.Rows.Clear();
            foreach (var user in users)
            {
                usersDataGridView.Rows.Add(
                    user.Id,
                    user.Email,
                    user.FirstName ?? "",
                    user.LastName ?? "",
                    user.Role,
                    user.IsActive ? "Yes" : "No",
                    user.IsAdmin ? "Yes" : "No",
                    user.CreatedDate.ToString("yyyy-MM-dd"),
                    user.LastLoginDate?.ToString("yyyy-MM-dd") ?? "Never"
                );
            }
        }

        private void AddUserButton_Click(object? sender, EventArgs e)
        {
            var addUserForm = new AddUserForm(isEnglish);
            if (addUserForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsersAsync(); // Refresh the list
            }
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadUsersAsync();
        }

        private async void UsersDataGridView_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var userIdValue = usersDataGridView.Rows[e.RowIndex].Cells["Id"].Value;
            if (userIdValue == null) return;
            var userId = (int)userIdValue;
            var user = allUsers.FirstOrDefault(u => u.Id == userId);

            if (user == null) return;

            if (usersDataGridView.Columns[e.ColumnIndex].Name == "Edit")
            {
                await EditUser(user);
            }
            else if (usersDataGridView.Columns[e.ColumnIndex].Name == "Delete")
            {
                await DeleteUser(user);
            }
        }

        private Task EditUser(User user)
        {
            try
            {
                var editUserForm = new EditUserForm(user, isEnglish);
                if (editUserForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUsersAsync(); // Refresh the list
                }
            }
            catch (Exception ex)
            {
                var message = isEnglish ? $"Error opening edit form: {ex.Message}" : $"Düzenleme formu açılırken hata: {ex.Message}";
                MessageBox.Show(message, isEnglish ? "Error" : "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return Task.CompletedTask;
        }

        private async Task DeleteUser(User user)
        {
            try
            {
                // Prevent deleting the current admin user
                if (user.IsAdmin && allUsers.Count(u => u.IsAdmin) <= 1)
                {
                    var message = isEnglish ?
                        "Cannot delete the last admin user. At least one admin must remain." :
                        "Son yönetici kullanıcı silinemez. En az bir yönetici kalmalıdır.";
                    MessageBox.Show(message, isEnglish ? "Cannot Delete" : "Silinemez",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirmMessage = isEnglish ?
                    $"Are you sure you want to delete user '{user.Email}'?\n\nThis action cannot be undone." :
                    $"'{user.Email}' kullanıcısını silmek istediğinizden emin misiniz?\n\nBu işlem geri alınamaz.";

                var confirmTitle = isEnglish ? "Confirm Delete" : "Silme Onayı";

                var result = MessageBox.Show(confirmMessage, confirmTitle,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    var success = await AccessDatabaseHelper.DeleteUserAsync(user.Id);

                    if (success)
                    {
                        var successMessage = isEnglish ?
                            "User deleted successfully." :
                            "Kullanıcı başarıyla silindi.";
                        MessageBox.Show(successMessage, isEnglish ? "Success" : "Başarılı",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUsersAsync(); // Refresh the list
                    }
                    else
                    {
                        var errorMessage = isEnglish ?
                            "Failed to delete user. Please try again." :
                            "Kullanıcı silinemedi. Lütfen tekrar deneyin.";
                        MessageBox.Show(errorMessage, isEnglish ? "Error" : "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                var message = isEnglish ? $"Error deleting user: {ex.Message}" : $"Kullanıcı silinirken hata: {ex.Message}";
                MessageBox.Show(message, isEnglish ? "Error" : "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                // English
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                if (titleLabel != null) titleLabel.Text = "👥 User Management";

                statsLabel.Text = statsLabel.Text.Replace("Toplam:", "Total:")
                                                 .Replace("Aktif:", "Active:")
                                                 .Replace("Yönetici:", "Admins:");

                searchTextBox.PlaceholderText = "Search users by name or email...";
                addUserButton.Text = "+ Add User";
                refreshButton.Text = "🔄 Refresh";

                var columns = usersDataGridView?.Columns;

                if (columns != null)
                {
                    if (columns.Contains("Email"))
                        columns["Email"].HeaderText = "Email";

                    if (columns.Contains("FirstName"))
                        columns["FirstName"].HeaderText = "First Name";

                    if (columns.Contains("LastName"))
                        columns["LastName"].HeaderText = "Last Name";

                    if (columns.Contains("Role"))
                        columns["Role"].HeaderText = "Role";

                    if (columns.Contains("IsActive"))
                        columns["IsActive"].HeaderText = "Active";

                    if (columns.Contains("IsAdmin"))
                        columns["IsAdmin"].HeaderText = "Admin";

                    if (columns.Contains("CreatedDate"))
                        columns["CreatedDate"].HeaderText = "Created Date";

                    if (columns.Contains("LastLoginDate"))
                        columns["LastLoginDate"].HeaderText = "Last Login";

                    if (columns.Contains("Edit"))
                        columns["Edit"].HeaderText = "Edit";

                    if (columns.Contains("Delete"))
                        columns["Delete"].HeaderText = "Delete";
                }

            }
            else
            {
                // Turkish
                var titleLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
                if (titleLabel != null) titleLabel.Text = "👥 Kullanıcı Yönetimi";

                statsLabel.Text = statsLabel.Text.Replace("Total:", "Toplam:")
                                                 .Replace("Active:", "Aktif:")
                                                 .Replace("Admins:", "Yönetici:");

                searchTextBox.PlaceholderText = "Kullanıcıları ad veya e-posta ile arayın...";
                addUserButton.Text = "+ Kullanıcı Ekle";
                refreshButton.Text = "🔄 Yenile";

                // Update DataGridView headers
                if (usersDataGridView.Columns.Count > 0)
                {
                    if (usersDataGridView.Columns["Email"] != null) usersDataGridView.Columns["Email"].HeaderText = "E-posta";
                    if (usersDataGridView.Columns["FirstName"] != null) usersDataGridView.Columns["FirstName"].HeaderText = "Ad";
                    if (usersDataGridView.Columns["LastName"] != null) usersDataGridView.Columns["LastName"].HeaderText = "Soyad";
                    if (usersDataGridView.Columns["Role"] != null) usersDataGridView.Columns["Role"].HeaderText = "Rol";
                    if (usersDataGridView.Columns["IsActive"] != null) usersDataGridView.Columns["IsActive"].HeaderText = "Aktif";
                    if (usersDataGridView.Columns["IsAdmin"] != null) usersDataGridView.Columns["IsAdmin"].HeaderText = "Yönetici";
                    if (usersDataGridView.Columns["CreatedDate"] != null) usersDataGridView.Columns["CreatedDate"].HeaderText = "Oluşturma Tarihi";
                    if (usersDataGridView.Columns["LastLoginDate"] != null) usersDataGridView.Columns["LastLoginDate"].HeaderText = "Son Giriş";
                    if (usersDataGridView.Columns["Edit"] != null) usersDataGridView.Columns["Edit"].HeaderText = "Düzenle";
                    if (usersDataGridView.Columns["Delete"] != null) usersDataGridView.Columns["Delete"].HeaderText = "Sil";
                }
            }
        }
    }
}
