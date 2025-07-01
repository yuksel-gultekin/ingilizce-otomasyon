using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views
{
    public partial class RegisterForm : Form
    {
        private TextBox firstNameTextBox = null!;
        private TextBox lastNameTextBox = null!;
        private TextBox emailTextBox = null!;
        private TextBox passwordTextBox = null!;
        private TextBox confirmPasswordTextBox = null!;
        private Button registerButton = null!;
        private Button cancelButton = null!;
        private Label messageLabel = null!;
        private Label titleLabel = null!;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Register - English Automation Platform";
            this.Size = new Size(500, 700);
            this.MinimumSize = new Size(480, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate-900

            // Title Label
            titleLabel = new Label();
            titleLabel.Text = "Create New Account";
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(50, 30);
            titleLabel.Size = new Size(400, 50);
            titleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Main Panel
            var mainPanel = new Panel();
            mainPanel.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
            mainPanel.Location = new Point(50, 100);
            mainPanel.Size = new Size(400, 540);
            mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // First Name
            var firstNameLabel = new Label();
            firstNameLabel.Text = "FIRST NAME";
            firstNameLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            firstNameLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            firstNameLabel.Location = new Point(40, 40);
            firstNameLabel.Size = new Size(320, 20);

            firstNameTextBox = new TextBox();
            firstNameTextBox.Font = new Font("Segoe UI", 12);
            firstNameTextBox.Location = new Point(40, 65);
            firstNameTextBox.Size = new Size(320, 30);
            firstNameTextBox.BorderStyle = BorderStyle.FixedSingle;
            firstNameTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            firstNameTextBox.ForeColor = Color.White;
            firstNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Last Name
            var lastNameLabel = new Label();
            lastNameLabel.Text = "LAST NAME";
            lastNameLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lastNameLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            lastNameLabel.Location = new Point(40, 100);
            lastNameLabel.Size = new Size(320, 20);

            lastNameTextBox = new TextBox();
            lastNameTextBox.Font = new Font("Segoe UI", 12);
            lastNameTextBox.Location = new Point(40, 120);
            lastNameTextBox.Size = new Size(320, 30);
            lastNameTextBox.BorderStyle = BorderStyle.FixedSingle;
            lastNameTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            lastNameTextBox.ForeColor = Color.White;
            lastNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Email
            var emailLabel = new Label();
            emailLabel.Text = "EMAIL ADDRESS";
            emailLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            emailLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            emailLabel.Location = new Point(40, 160);
            emailLabel.Size = new Size(320, 20);

            emailTextBox = new TextBox();
            emailTextBox.Font = new Font("Segoe UI", 12);
            emailTextBox.Location = new Point(40, 180);
            emailTextBox.Size = new Size(320, 30);
            emailTextBox.BorderStyle = BorderStyle.FixedSingle;
            emailTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            emailTextBox.ForeColor = Color.White;
            emailTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Password
            var passwordLabel = new Label();
            passwordLabel.Text = "PASSWORD";
            passwordLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            passwordLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            passwordLabel.Location = new Point(40, 220);
            passwordLabel.Size = new Size(320, 20);

            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 12);
            passwordTextBox.Location = new Point(40, 240);
            passwordTextBox.Size = new Size(320, 30);
            passwordTextBox.BorderStyle = BorderStyle.FixedSingle;
            passwordTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            passwordTextBox.ForeColor = Color.White;
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Confirm Password
            var confirmPasswordLabel = new Label();
            confirmPasswordLabel.Text = "CONFIRM PASSWORD";
            confirmPasswordLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            confirmPasswordLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            confirmPasswordLabel.Location = new Point(40, 280);
            confirmPasswordLabel.Size = new Size(320, 20);

            confirmPasswordTextBox = new TextBox();
            confirmPasswordTextBox.Font = new Font("Segoe UI", 12);
            confirmPasswordTextBox.Location = new Point(40, 300);
            confirmPasswordTextBox.Size = new Size(320, 30);
            confirmPasswordTextBox.BorderStyle = BorderStyle.FixedSingle;
            confirmPasswordTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            confirmPasswordTextBox.ForeColor = Color.White;
            confirmPasswordTextBox.UseSystemPasswordChar = true;
            confirmPasswordTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Register Button
            registerButton = new Button();
            registerButton.Text = "Create Account";
            registerButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            registerButton.BackColor = Color.FromArgb(99, 102, 241); // Indigo-500
            registerButton.ForeColor = Color.White;
            registerButton.FlatStyle = FlatStyle.Flat;
            registerButton.FlatAppearance.BorderSize = 0;
            registerButton.Location = new Point(40, 360);
            registerButton.Size = new Size(150, 45);
            registerButton.Cursor = Cursors.Hand;
            registerButton.Click += RegisterButton_Click;

            // Cancel Button
            cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Font = new Font("Segoe UI", 11);
            cancelButton.BackColor = Color.Transparent;
            cancelButton.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.FlatAppearance.BorderSize = 1;
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
            cancelButton.Location = new Point(210, 360);
            cancelButton.Size = new Size(150, 45);
            cancelButton.Cursor = Cursors.Hand;
            cancelButton.Click += CancelButton_Click;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.Location = new Point(40, 420);
            messageLabel.Size = new Size(320, 60);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Add controls to main panel
            mainPanel.Controls.Add(firstNameLabel);
            mainPanel.Controls.Add(firstNameTextBox);
            mainPanel.Controls.Add(lastNameLabel);
            mainPanel.Controls.Add(lastNameTextBox);
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(confirmPasswordLabel);
            mainPanel.Controls.Add(confirmPasswordTextBox);
            mainPanel.Controls.Add(registerButton);
            mainPanel.Controls.Add(cancelButton);
            mainPanel.Controls.Add(messageLabel);

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        private async void RegisterButton_Click(object? sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            registerButton.Enabled = false;
            registerButton.Text = "Creating account...";

            try
            {
                var result = await AuthenticationService.RegisterAsync(
                    emailTextBox.Text.Trim(),
                    passwordTextBox.Text,
                    firstNameTextBox.Text.Trim(),
                    lastNameTextBox.Text.Trim());

                if (result.Success)
                {
                    ShowMessage(result.Message, Color.FromArgb(34, 197, 94)); // Green-500

                    // Wait 2 seconds and close form
                    await Task.Delay(2000);
                    this.Close();
                }
                else
                {
                    ShowMessage(result.Message, Color.FromArgb(248, 113, 113)); // Red-400
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Unexpected error: {ex.Message}", Color.FromArgb(248, 113, 113)); // Red-400
            }
            finally
            {
                registerButton.Enabled = true;
                registerButton.Text = "Create Account";
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateForm()
        {
            // First name check
            if (string.IsNullOrWhiteSpace(firstNameTextBox.Text))
            {
                ShowMessage("Please enter your first name.", Color.FromArgb(248, 113, 113)); // Red-400
                firstNameTextBox.Focus();
                return false;
            }

            // Last name check
            if (string.IsNullOrWhiteSpace(lastNameTextBox.Text))
            {
                ShowMessage("Please enter your last name.", Color.FromArgb(248, 113, 113)); // Red-400
                lastNameTextBox.Focus();
                return false;
            }

            // Email check
            if (string.IsNullOrWhiteSpace(emailTextBox.Text))
            {
                ShowMessage("Please enter your email address.", Color.FromArgb(248, 113, 113)); // Red-400
                emailTextBox.Focus();
                return false;
            }

            if (!IsValidEmail(emailTextBox.Text))
            {
                ShowMessage("Please enter a valid email address.", Color.FromArgb(248, 113, 113)); // Red-400
                emailTextBox.Focus();
                return false;
            }

            // Password check
            if (string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                ShowMessage("Please enter your password.", Color.FromArgb(248, 113, 113)); // Red-400
                passwordTextBox.Focus();
                return false;
            }

            if (passwordTextBox.Text.Length < 6)
            {
                ShowMessage("Password must be at least 6 characters long.", Color.FromArgb(248, 113, 113)); // Red-400
                passwordTextBox.Focus();
                return false;
            }

            // Confirm password check
            if (passwordTextBox.Text != confirmPasswordTextBox.Text)
            {
                ShowMessage("Passwords do not match.", Color.FromArgb(248, 113, 113)); // Red-400
                confirmPasswordTextBox.Focus();
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void ShowMessage(string message, Color color)
        {
            messageLabel.Text = message;
            messageLabel.ForeColor = color;
        }
    }
}
