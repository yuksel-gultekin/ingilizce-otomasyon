using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private Label subtitleLabel;
        private Label emailLabel;
        private Label passwordLabel;
        private Panel mainPanel;
        private Panel inputPanel;
        private PictureBox logoBox;
        private CheckBox rememberMeCheckBox;
        private LinkLabel forgotPasswordLabel;
        private Timer fadeTimer;
        private float opacity = 0f;

        public LoginForm()
        {
            InitializeComponent();
            SetupModernStyling();
            SetupFadeInAnimation();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties - Modern styling
            this.Text = "English Automation Platform";
            this.Size = new Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate-900
            this.AllowTransparency = true;
            this.DoubleBuffered = true;

            // Logo/Icon placeholder
            logoBox = new PictureBox();
            logoBox.Size = new Size(80, 80);
            logoBox.Location = new Point((this.Width - logoBox.Width) / 2, 60);
            logoBox.BackColor = Color.FromArgb(99, 102, 241); // Indigo-500
            logoBox.Paint += LogoBox_Paint;

            // Title Label - Modern typography
            titleLabel = new Label();
            titleLabel.Text = "Welcome back";
            titleLabel.Font = new Font("Segoe UI", 28, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(50, 160);
            titleLabel.Size = new Size(400, 50);

            // Subtitle Label
            subtitleLabel = new Label();
            subtitleLabel.Text = "Sign in to your account to continue";
            subtitleLabel.Font = new Font("Segoe UI", 11);
            subtitleLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.Location = new Point(50, 210);
            subtitleLabel.Size = new Size(400, 25);

            // Main Panel - Card-like container
            mainPanel = new Panel();
            mainPanel.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
            mainPanel.Location = new Point(60, 260);
            mainPanel.Size = new Size(380, 380);
            mainPanel.Paint += MainPanel_Paint;

            // Input Panel - Container for form fields
            inputPanel = new Panel();
            inputPanel.BackColor = Color.Transparent;
            inputPanel.Location = new Point(40, 40);
            inputPanel.Size = new Size(300, 280);

            // Email Label
            emailLabel = new Label();
            emailLabel.Text = "EMAIL ADDRESS";
            emailLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            emailLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            emailLabel.Location = new Point(0, 0);
            emailLabel.Size = new Size(300, 20);

            // Email TextBox - Modern input styling
            emailTextBox = new TextBox();
            emailTextBox.Font = new Font("Segoe UI", 12);
            emailTextBox.Location = new Point(0, 25);
            emailTextBox.Size = new Size(300, 35);
            emailTextBox.BorderStyle = BorderStyle.None;
            emailTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            emailTextBox.ForeColor = Color.White;
            emailTextBox.Padding = new Padding(15, 10, 15, 10);
            emailTextBox.PlaceholderText = "Enter your email";

            // Password Label
            passwordLabel = new Label();
            passwordLabel.Text = "PASSWORD";
            passwordLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            passwordLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            passwordLabel.Location = new Point(0, 80);
            passwordLabel.Size = new Size(300, 20);

            // Password TextBox
            passwordTextBox = new TextBox();
            passwordTextBox.Font = new Font("Segoe UI", 12);
            passwordTextBox.Location = new Point(0, 105);
            passwordTextBox.Size = new Size(300, 35);
            passwordTextBox.BorderStyle = BorderStyle.None;
            passwordTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            passwordTextBox.ForeColor = Color.White;
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.Padding = new Padding(15, 10, 15, 10);
            passwordTextBox.PlaceholderText = "Enter your password";

            // Remember Me Checkbox
            rememberMeCheckBox = new CheckBox();
            rememberMeCheckBox.Text = "Remember me";
            rememberMeCheckBox.Font = new Font("Segoe UI", 9);
            rememberMeCheckBox.ForeColor = Color.FromArgb(148, 163, 184);
            rememberMeCheckBox.Location = new Point(0, 160);
            rememberMeCheckBox.Size = new Size(120, 20);
            rememberMeCheckBox.FlatStyle = FlatStyle.Flat;

            // Forgot Password Link
            forgotPasswordLabel = new LinkLabel();
            forgotPasswordLabel.Text = "Forgot password?";
            forgotPasswordLabel.Font = new Font("Segoe UI", 9);
            forgotPasswordLabel.LinkColor = Color.FromArgb(99, 102, 241); // Indigo-500
            forgotPasswordLabel.ActiveLinkColor = Color.FromArgb(79, 70, 229); // Indigo-600
            forgotPasswordLabel.Location = new Point(180, 160);
            forgotPasswordLabel.Size = new Size(120, 20);
            forgotPasswordLabel.TextAlign = ContentAlignment.MiddleRight;

            // Login Button - Modern gradient button
            loginButton = new Button();
            loginButton.Text = "Sign in";
            loginButton.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            loginButton.ForeColor = Color.White;
            loginButton.FlatStyle = FlatStyle.Flat;
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Location = new Point(0, 200);
            loginButton.Size = new Size(300, 45);
            loginButton.Cursor = Cursors.Hand;
            loginButton.Click += LoginButton_Click;
            loginButton.Paint += LoginButton_Paint;
            loginButton.MouseEnter += LoginButton_MouseEnter;
            loginButton.MouseLeave += LoginButton_MouseLeave;

            // Register Button - Outline style
            registerButton = new Button();
            registerButton.Text = "Create new account";
            registerButton.Font = new Font("Segoe UI", 11);
            registerButton.ForeColor = Color.FromArgb(99, 102, 241); // Indigo-500
            registerButton.BackColor = Color.Transparent;
            registerButton.FlatStyle = FlatStyle.Flat;
            registerButton.FlatAppearance.BorderSize = 1;
            registerButton.FlatAppearance.BorderColor = Color.FromArgb(99, 102, 241);
            registerButton.Location = new Point(60, 660);
            registerButton.Size = new Size(380, 35);
            registerButton.Cursor = Cursors.Hand;
            registerButton.Click += RegisterButton_Click;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.Location = new Point(0, 255);
            messageLabel.Size = new Size(300, 40);
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;

            // Add controls to input panel
            inputPanel.Controls.Add(emailLabel);
            inputPanel.Controls.Add(emailTextBox);
            inputPanel.Controls.Add(passwordLabel);
            inputPanel.Controls.Add(passwordTextBox);
            inputPanel.Controls.Add(rememberMeCheckBox);
            inputPanel.Controls.Add(forgotPasswordLabel);
            inputPanel.Controls.Add(loginButton);
            inputPanel.Controls.Add(messageLabel);

            // Add input panel to main panel
            mainPanel.Controls.Add(inputPanel);

            // Add all controls to form
            this.Controls.Add(logoBox);
            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
            this.Controls.Add(mainPanel);
            this.Controls.Add(registerButton);

            this.ResumeLayout(false);
        }

        private void SetupModernStyling()
        {
            // Setup custom input styling
            SetupTextBoxModernStyle(emailTextBox);
            SetupTextBoxModernStyle(passwordTextBox);
        }

        private void SetupTextBoxModernStyle(TextBox textBox)
        {
            textBox.Paint += (s, e) =>
            {
                var tb = s as TextBox;
                using (var pen = new Pen(Color.FromArgb(71, 85, 105), 1)) // Slate-600
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, tb.Width - 1, tb.Height - 1);
                }
            };

            textBox.Enter += (s, e) =>
            {
                var tb = s as TextBox;
                tb.BackColor = Color.FromArgb(71, 85, 105); // Slate-600
                tb.Invalidate();
            };

            textBox.Leave += (s, e) =>
            {
                var tb = s as TextBox;
                tb.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
                tb.Invalidate();
            };
        }

        private void SetupFadeInAnimation()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 20;
            fadeTimer.Tick += (s, e) =>
            {
                opacity += 0.05f;
                if (opacity >= 1.0f)
                {
                    opacity = 1.0f;
                    fadeTimer.Stop();
                }
                this.Opacity = opacity;
            };
            this.Shown += (s, e) => fadeTimer.Start();
        }

        private void LogoBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            using (var brush = new LinearGradientBrush(
                logoBox.ClientRectangle,
                Color.FromArgb(99, 102, 241), // Indigo-500
                Color.FromArgb(139, 92, 246), // Violet-500
                LinearGradientMode.Diagonal))
            {
                g.FillEllipse(brush, 0, 0, logoBox.Width, logoBox.Height);
            }

            // Draw a simple icon (graduation cap or book symbol)
            using (var pen = new Pen(Color.White, 3))
            {
                var center = new Point(logoBox.Width / 2, logoBox.Height / 2);
                g.DrawEllipse(pen, center.X - 15, center.Y - 10, 30, 20);
                g.DrawLine(pen, center.X - 20, center.Y - 15, center.X + 20, center.Y - 15);
                g.DrawLine(pen, center.X, center.Y - 25, center.X, center.Y - 5);
            }
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Create rounded rectangle
            using (var path = GetRoundedRectangle(mainPanel.ClientRectangle, 12))
            {
                using (var brush = new SolidBrush(Color.FromArgb(30, 41, 59))) // Slate-800
                {
                    g.FillPath(brush, path);
                }
                
                // Add subtle border
                using (var pen = new Pen(Color.FromArgb(51, 65, 85), 1)) // Slate-700
                {
                    g.DrawPath(pen, path);
                }
            }
        }

        private void LoginButton_Paint(object sender, PaintEventArgs e)
        {
            var button = sender as Button;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = button.ClientRectangle;
            using (var path = GetRoundedRectangle(rect, 8))
            {
                Color startColor, endColor;
                if (button.ClientRectangle.Contains(button.PointToClient(Cursor.Position)))
                {
                    startColor = Color.FromArgb(79, 70, 229); // Indigo-600
                    endColor = Color.FromArgb(67, 56, 202);   // Indigo-700
                }
                else
                {
                    startColor = Color.FromArgb(99, 102, 241); // Indigo-500
                    endColor = Color.FromArgb(79, 70, 229);    // Indigo-600
                }

                using (var brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical))
                {
                    g.FillPath(brush, path);
                }
            }

            // Draw text
            var textRect = button.ClientRectangle;
            TextRenderer.DrawText(g, button.Text, button.Font, textRect, button.ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void LoginButton_MouseEnter(object sender, EventArgs e)
        {
            loginButton.Invalidate();
        }

        private void LoginButton_MouseLeave(object sender, EventArgs e)
        {
            loginButton.Invalidate();
        }

        private GraphicsPath GetRoundedRectangle(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, cornerRadius, cornerRadius, 180, 90);
            path.AddArc(rect.Right - cornerRadius, rect.Y, cornerRadius, cornerRadius, 270, 90);
            path.AddArc(rect.Right - cornerRadius, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - cornerRadius, cornerRadius, cornerRadius, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Add subtle background gradient
            using (var brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(15, 23, 42),  // Slate-900
                Color.FromArgb(30, 41, 59),  // Slate-800
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailTextBox.Text) || 
                string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                ShowMessage("Please fill in all required fields.", Color.FromArgb(248, 113, 113)); // Red-400
                return;
            }

            loginButton.Enabled = false;
            loginButton.Text = "Signing in...";
            loginButton.Invalidate();

            try
            {
                var result = await AuthenticationService.LoginAsync(
                    emailTextBox.Text.Trim(), 
                    passwordTextBox.Text);

                if (result.Success)
                {
                    ShowMessage(result.Message, Color.FromArgb(34, 197, 94)); // Green-500
                    
                    // Fade out animation before opening main form
                    var fadeOutTimer = new Timer();
                    fadeOutTimer.Interval = 20;
                    fadeOutTimer.Tick += (s, args) =>
                    {
                        this.Opacity -= 0.1;
                        if (this.Opacity <= 0)
                        {
                            fadeOutTimer.Stop();
                            var mainForm = new MainForm();
                            mainForm.Show();
                            this.Hide();
                        }
                    };
                    fadeOutTimer.Start();
                }
                else
                {
                    ShowMessage(result.Message, Color.FromArgb(248, 113, 113)); // Red-400
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Connection error. Please try again.", Color.FromArgb(248, 113, 113)); // Red-400
            }
            finally
            {
                loginButton.Enabled = true;
                loginButton.Text = "Sign in";
                loginButton.Invalidate();
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
            
            // Add fade-in animation for messages
            var messageTimer = new Timer();
            messageTimer.Interval = 3000;
            messageTimer.Tick += (s, e) =>
            {
                messageLabel.Text = "";
                messageTimer.Stop();
            };
            messageTimer.Start();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }
    }
}