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
        private TextBox emailTextBox = null!;
        private TextBox passwordTextBox = null!;
        private Button loginButton = null!;
        private Button registerButton = null!;
        private Button adminPanelButton = null!;
        private ComboBox languageComboBox = null!;
        private Label messageLabel = null!;
        private Label titleLabel = null!;
        private Label subtitleLabel = null!;
        private Label emailLabel = null!;
        private Label passwordLabel = null!;
        private Panel mainPanel = null!;
        private Panel inputPanel = null!;
        private PictureBox logoBox = null!;
        private CheckBox rememberMeCheckBox = null!;
        private LinkLabel forgotPasswordLabel = null!;
        private System.Windows.Forms.Timer fadeTimer = null!;
        private float opacity = 0f;
        private bool isEnglish = true;

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
            this.Size = new Size(500, 650);
            this.MinimumSize = new Size(480, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate-900
            this.DoubleBuffered = true;

            // Logo/Icon placeholder
            logoBox = new PictureBox();
            logoBox.Size = new Size(80, 80);
            logoBox.Location = new Point(210, 40);
            logoBox.BackColor = Color.FromArgb(99, 102, 241); // Indigo-500
            logoBox.Anchor = AnchorStyles.Top;
            logoBox.Paint += LogoBox_Paint;

            // Language ComboBox
            languageComboBox = new ComboBox();
            languageComboBox.Items.AddRange(new[] { "🇺🇸 English", "🇹🇷 Türkçe" });
            languageComboBox.SelectedIndex = 0;
            languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageComboBox.Font = new Font("Segoe UI", 9);
            languageComboBox.BackColor = Color.FromArgb(51, 65, 85);
            languageComboBox.ForeColor = Color.White;
            languageComboBox.Location = new Point(350, 20);
            languageComboBox.Size = new Size(120, 25);
            languageComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            // Title Label - Modern typography
            titleLabel = new Label();
            titleLabel.Text = "Welcome back";
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Location = new Point(50, 140);
            titleLabel.Size = new Size(400, 40);
            titleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Subtitle Label
            subtitleLabel = new Label();
            subtitleLabel.Text = "Sign in to your account to continue";
            subtitleLabel.Font = new Font("Segoe UI", 11);
            subtitleLabel.ForeColor = Color.FromArgb(148, 163, 184); // Slate-400
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.Location = new Point(50, 180);
            subtitleLabel.Size = new Size(400, 25);
            subtitleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Main Panel - Card-like container
            mainPanel = new Panel();
            mainPanel.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
            mainPanel.Location = new Point(50, 220);
            mainPanel.Size = new Size(400, 320);
            mainPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            mainPanel.Paint += MainPanel_Paint;

            // Input Panel - Container for form fields
            inputPanel = new Panel();
            inputPanel.BackColor = Color.Transparent;
            inputPanel.Location = new Point(50, 30);
            inputPanel.Size = new Size(300, 290);
            inputPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

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
            emailTextBox.Size = new Size(300, 32);
            emailTextBox.BorderStyle = BorderStyle.None;
            emailTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            emailTextBox.ForeColor = Color.White;
            emailTextBox.Padding = new Padding(12, 8, 12, 8);
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
            passwordTextBox.Size = new Size(300, 32);
            passwordTextBox.BorderStyle = BorderStyle.None;
            passwordTextBox.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
            passwordTextBox.ForeColor = Color.White;
            passwordTextBox.UseSystemPasswordChar = true;
            passwordTextBox.Padding = new Padding(12, 8, 12, 8);
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

            // Register Button - Outline style (smaller)
            registerButton = new Button();
            registerButton.Text = "Create new account";
            registerButton.Font = new Font("Segoe UI", 10);
            registerButton.ForeColor = Color.FromArgb(99, 102, 241); // Indigo-500
            registerButton.BackColor = Color.Transparent;
            registerButton.FlatStyle = FlatStyle.Flat;
            registerButton.FlatAppearance.BorderSize = 1;
            registerButton.FlatAppearance.BorderColor = Color.FromArgb(99, 102, 241);
            registerButton.Location = new Point(50, 555);
            registerButton.Size = new Size(190, 35);
            registerButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            registerButton.Cursor = Cursors.Hand;
            registerButton.Click += RegisterButton_Click;

            // Admin Panel Button
            adminPanelButton = new Button();
            adminPanelButton.Text = "🔧 Admin Panel";
            adminPanelButton.Font = new Font("Segoe UI", 10);
            adminPanelButton.ForeColor = Color.FromArgb(245, 158, 11); // Amber-500
            adminPanelButton.BackColor = Color.Transparent;
            adminPanelButton.FlatStyle = FlatStyle.Flat;
            adminPanelButton.FlatAppearance.BorderSize = 1;
            adminPanelButton.FlatAppearance.BorderColor = Color.FromArgb(245, 158, 11);
            adminPanelButton.Location = new Point(260, 555);
            adminPanelButton.Size = new Size(190, 35);
            adminPanelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            adminPanelButton.Cursor = Cursors.Hand;
            adminPanelButton.Click += AdminPanelButton_Click;

            // Message Label
            messageLabel = new Label();
            messageLabel.Font = new Font("Segoe UI", 10);
            messageLabel.Location = new Point(0, 250);
            messageLabel.Size = new Size(300, 30);
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
            this.Controls.Add(languageComboBox);
            this.Controls.Add(titleLabel);
            this.Controls.Add(subtitleLabel);
            this.Controls.Add(mainPanel);
            this.Controls.Add(registerButton);
            this.Controls.Add(adminPanelButton);

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
            // Modern rounded corners style
            textBox.Paint += (s, e) =>
            {
                var tb = s as TextBox;
                if (tb != null)
                {
                    // Create rounded rectangle background
                    using (var path = GetRoundedRectangle(new Rectangle(0, 0, tb.Width, tb.Height), 6))
                    {
                        using (var brush = new SolidBrush(tb.BackColor))
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.FillPath(brush, path);
                        }
                    }
                }
            };

            textBox.Enter += (s, e) =>
            {
                var tb = s as TextBox;
                if (tb != null)
                {
                    tb.BackColor = Color.FromArgb(71, 85, 105); // Slate-600
                    tb.Invalidate();
                }
            };

            textBox.Leave += (s, e) =>
            {
                var tb = s as TextBox;
                if (tb != null)
                {
                    tb.BackColor = Color.FromArgb(51, 65, 85); // Slate-700
                    tb.Invalidate();
                }
            };
        }

        private void SetupFadeInAnimation()
        {
            fadeTimer = new System.Windows.Forms.Timer();
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

        private void LogoBox_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            using (var brush = new LinearGradientBrush(
                logoBox.ClientRectangle,
                Color.FromArgb(99, 102, 241), // Indigo-500
                Color.FromArgb(139, 92, 246), // Violet-500
                LinearGradientMode.ForwardDiagonal))
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

        private void MainPanel_Paint(object? sender, PaintEventArgs e)
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

        private void LoginButton_Paint(object? sender, PaintEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

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

        private void LoginButton_MouseEnter(object? sender, EventArgs e)
        {
            loginButton.Invalidate();
        }

        private void LoginButton_MouseLeave(object? sender, EventArgs e)
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

        private async void LoginButton_Click(object? sender, EventArgs e)
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
                    var fadeOutTimer = new System.Windows.Forms.Timer();
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
            catch (Exception)
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

        private void RegisterButton_Click(object? sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog(this);
        }

        private void AdminPanelButton_Click(object? sender, EventArgs e)
        {
            // Admin login dialog
            var adminLoginForm = new AdminLoginForm();
            if (adminLoginForm.ShowDialog(this) == DialogResult.OK)
            {
                // Open admin panel
                var adminForm = new AdminPanelForm();
                adminForm.ShowDialog(this);
            }
        }

        private void LanguageComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            isEnglish = languageComboBox.SelectedIndex == 0;
            UpdateLanguage();
        }

        private void UpdateLanguage()
        {
            if (isEnglish)
            {
                titleLabel.Text = "Welcome back";
                subtitleLabel.Text = "Sign in to your account to continue";
                emailLabel.Text = "EMAIL ADDRESS";
                passwordLabel.Text = "PASSWORD";
                rememberMeCheckBox.Text = "Remember me";
                forgotPasswordLabel.Text = "Forgot password?";
                loginButton.Text = "Sign in";
                registerButton.Text = "Create new account";
                adminPanelButton.Text = "🔧 Admin Panel";
            }
            else
            {
                titleLabel.Text = "Tekrar hoş geldiniz";
                subtitleLabel.Text = "Devam etmek için hesabınıza giriş yapın";
                emailLabel.Text = "E-POSTA ADRESİ";
                passwordLabel.Text = "ŞİFRE";
                rememberMeCheckBox.Text = "Beni hatırla";
                forgotPasswordLabel.Text = "Şifremi unuttum?";
                loginButton.Text = "Giriş yap";
                registerButton.Text = "Yeni hesap oluştur";
                adminPanelButton.Text = "🔧 Yönetici Paneli";
            }
        }

        private void ShowMessage(string message, Color color)
        {
            messageLabel.Text = message;
            messageLabel.ForeColor = color;
            
            // Add fade-in animation for messages
            var messageTimer = new System.Windows.Forms.Timer();
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