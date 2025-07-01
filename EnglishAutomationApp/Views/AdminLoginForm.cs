using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views
{
    public partial class AdminLoginForm : Form
    {
        private TextBox emailTextBox = null!;
        private TextBox passwordTextBox = null!;
        private Button loginButton = null!;
        private Button cancelButton = null!;
        private Label messageLabel = null!;
        private Label titleLabel = null!;

        public AdminLoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();


            this.Text = "Admin Login - English Automation Platform";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate-900

            // Title Label
            titleLabel = new Label();
            titleLabel.Text = "🔧 Admin Panel Access";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(20, 30);
            titleLabel.Size = new Size(360, 40);

            // Main Panel
            var mainPanel = new Panel();
            mainPanel.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
            mainPanel.Location = new Point(30, 80);
            mainPanel.Size = new Size(340, 220);

            // Email Label
            var emailLabel = new Label();
            emailLabel.Text = "ADMIN EMAIL";
            emailLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            emailLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            emailLabel.Location = new Point(30, 30);
            emailLabel.Size = new Size(280, 20);

            // Email TextBox
            emailTextBox = new TextBox();
            emailTextBox.Font = new Font("Segoe UI", 12);
            emailTextBox.Location = new Point(30, 50);
            emailTextBox.Size = new Size(280, 30);
            emailTextBox.BorderStyle = BorderStyle.FixedSingle;
            emailTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            emailTextBox.ForeColor = Color.White;

            // Password Label
            var passwordLabel = new Label();
            passwordLabel.Text = "ADMIN PASSWORD";
            passwordLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            passwordLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            passwordLabel.Location = new Point(30, 90);
            passwordLabel.Size = new Size(280, 20);

            // Password TextBox
            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 12);
            passwordTextBox.Location = new Point(30, 110);
            passwordTextBox.Size = new Size(280, 30);
            passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
            passwordTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            passwordTextBox.ForeColor = Color.White;
            passwordTextBox.UseSystemPasswordChar = true;

            // Login Button
            loginButton = new Button();
            loginButton.Text = "Access Admin Panel";
            loginButton.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            loginButton.BackColor = Color.FromArgb(245, 158, 11); // Amber-500
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Location = new Point(30, 160);
            loginButton.Size = new Size(130, 40);
            loginButton.Cursor = Cursors.Hand;
            loginButton.Click += LoginButton_Click;

            // Cancel Button
            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Font = new Font("Segoe UI", 11);
            cancelButton.BackColor = Color.Transparent;
            cancelButton.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 1;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
            cancelButton.Location = new Point(180, 160);
            cancelButton.Size = new Size(130, 40);
            cancelButton.Cursor = Cursors.Hand;
            cancelButton.Click += CancelButton_Click;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.Location = new Point(30, 210);
            messageLabel.Size = new Size(280, 30);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to main panel
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(loginButton);
            mainPanel.Controls.Add(cancelButton);
            mainPanel.Controls.Add(messageLabel);

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        private async void LoginButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailTextBox.Text) || string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                ShowMessage("Please enter both email and password.", Color.FromArgb(248, 113, 113));
                return;
            }

            loginButton.Enabled = false;
            loginButton.Text = "Checking...";

            try
            {
                var result = await AuthenticationService.LoginAsync(emailTextBox.Text.Trim(), passwordTextBox.Text);

                if (result.Success && AuthenticationService.CurrentUser?.IsAdmin == true)
                {
                    ShowMessage("Admin access granted!", Color.FromArgb(34, 197, 94));
                    await Task.Delay(1000);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (result.Success)
                {
                    ShowMessage("Access denied. Admin privileges required.", Color.FromArgb(248, 113, 113));
                    AuthenticationService.Logout();
                }
                else
                {
                    ShowMessage(result.Message, Color.FromArgb(248, 113, 113));
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Login error: {ex.Message}", Color.FromArgb(248, 113, 113));
            }
            finally
            {
                loginButton.Enabled = true;
                loginButton.Text = "Access Admin Panel";
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ShowMessage(string message, Color color)
        {
            messageLabel.Text = message;
            messageLabel.ForeColor = color;
        }
    }
}
