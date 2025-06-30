using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnglishAutomationApp.Services;

namespace EnglishAutomationApp.Views
{
    public partial class LoginForm : Form
    {
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Button registerButton;
        private Label messageLabel;
        private Label titleLabel;
        private Label emailLabel;
        private Label passwordLabel;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "English Automation Platform - Login";
            this.Size = new Size(450, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(102, 126, 234);

            // Title Label
            titleLabel = new Label();
            titleLabel.Text = "🌟 English Automation Platform";
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(50, 50);
            titleLabel.Size = new Size(350, 40);

            // Main Panel
            var mainPanel = new Panel();
            mainPanel.BackColor = Color.White;
            mainPanel.Location = new Point(50, 120);
            mainPanel.Size = new Size(350, 400);

            // Email Label
            emailLabel = new Label();
            emailLabel.Text = "Email:";
            emailLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            emailLabel.Location = new Point(30, 50);
            emailLabel.Size = new Size(100, 25);

            // Email TextBox
            emailTextBox = new TextBox();
            emailTextBox.Font = new Font("Segoe UI", 12);
            emailTextBox.Location = new Point(30, 75);
            emailTextBox.Size = new Size(290, 30);

            // Password Label
            passwordLabel = new Label();
            passwordLabel.Text = "Password:";
            passwordLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            passwordLabel.Location = new Point(30, 120);
            passwordLabel.Size = new Size(100, 25);

            // Password TextBox
            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 12);
            passwordTextBox.Location = new Point(30, 145);
            passwordTextBox.Size = new Size(290, 30);
            passwordTextBox.UseSystemPasswordChar = true;

            // Login Button
            loginButton = new Button();
            loginButton.Text = "🚀 Sign In";
            loginButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            loginButton.BackColor = Color.FromArgb(102, 126, 234);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Location = new Point(30, 200);
            loginButton.Size = new Size(290, 45);
            loginButton.Click += LoginButton_Click;

            // Register Button
            registerButton = new Button();
            registerButton.Text = "Create New Account";
            registerButton.Font = new Font("Segoe UI", 10);
            registerButton.BackColor = Color.Transparent;
            registerButton.ForeColor = Color.FromArgb(102, 126, 234);
            registerButton.FlatStyle = FlatStyle.Flat;
            registerButton.FlatAppearance.BorderSize = 1;
            registerButton.FlatAppearance.BorderColor = Color.FromArgb(102, 126, 234);
            registerButton.Location = new Point(30, 260);
            registerButton.Size = new Size(290, 35);
            registerButton.Click += RegisterButton_Click;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 9);
            messageLabel.Location = new Point(30, 310);
            messageLabel.Size = new Size(290, 50);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to main panel
            mainPanel.Controls.Add(emailLabel);
            mainPanel.Controls.Add(emailTextBox);
            mainPanel.Controls.Add(passwordLabel);
            mainPanel.Controls.Add(passwordTextBox);
            mainPanel.Controls.Add(loginButton);
            mainPanel.Controls.Add(registerButton);
            mainPanel.Controls.Add(messageLabel);

            // Add controls to form
            this.Controls.Add(titleLabel);
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailTextBox.Text) || 
                string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                ShowMessage("Please fill in email and password fields.", Color.Red);
                return;
            }

            loginButton.Enabled = false;
            loginButton.Text = "Logging in...";

            try
            {
                var result = await AuthenticationService.LoginAsync(
                    emailTextBox.Text.Trim(), 
                    passwordTextBox.Text);

                if (result.Success)
                {
                    ShowMessage(result.Message, Color.Green);
                    
                    // Open main form
                    var mainForm = new MainForm();
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    ShowMessage(result.Message, Color.Red);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Unexpected error: {ex.Message}", Color.Red);
            }
            finally
            {
                loginButton.Enabled = true;
                loginButton.Text = "🚀 Sign In";
            }
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog(this);
        }

        private void ShowMessage(string message, Color color)
        {
            messageLabel.Text = message;
            messageLabel.ForeColor = color;
        }
    }
}
