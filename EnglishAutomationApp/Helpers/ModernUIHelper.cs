using System;
using System.Drawing;
using System.Windows.Forms;

namespace EnglishAutomationApp.Helpers
{
    public static class ModernUIHelper
    {
        // Modern Color Palette
        public static class Colors
        {
            public static readonly Color Primary = Color.FromArgb(79, 70, 229);      // Indigo-600
            public static readonly Color PrimaryLight = Color.FromArgb(129, 140, 248); // Indigo-400
            public static readonly Color PrimaryDark = Color.FromArgb(67, 56, 202);   // Indigo-700
            
            public static readonly Color Secondary = Color.FromArgb(16, 185, 129);    // Emerald-500
            public static readonly Color SecondaryLight = Color.FromArgb(52, 211, 153); // Emerald-400
            
            public static readonly Color Background = Color.FromArgb(249, 250, 251);  // Gray-50
            public static readonly Color Surface = Color.White;
            public static readonly Color SurfaceVariant = Color.FromArgb(243, 244, 246); // Gray-100
            
            public static readonly Color TextPrimary = Color.FromArgb(17, 24, 39);    // Gray-900
            public static readonly Color TextSecondary = Color.FromArgb(107, 114, 128); // Gray-500
            public static readonly Color TextMuted = Color.FromArgb(156, 163, 175);   // Gray-400
            
            public static readonly Color Success = Color.FromArgb(34, 197, 94);       // Green-500
            public static readonly Color Warning = Color.FromArgb(245, 158, 11);      // Amber-500
            public static readonly Color Error = Color.FromArgb(239, 68, 68);         // Red-500
            
            public static readonly Color Border = Color.FromArgb(229, 231, 235);      // Gray-200
            public static readonly Color BorderLight = Color.FromArgb(243, 244, 246); // Gray-100
        }

        // Modern Typography
        public static class Fonts
        {
            public static readonly Font Heading1 = new Font("Segoe UI", 24, FontStyle.Bold);
            public static readonly Font Heading2 = new Font("Segoe UI", 20, FontStyle.Bold);
            public static readonly Font Heading3 = new Font("Segoe UI", 16, FontStyle.Bold);
            public static readonly Font Heading4 = new Font("Segoe UI", 14, FontStyle.Bold);
            
            public static readonly Font Body = new Font("Segoe UI", 12, FontStyle.Regular);
            public static readonly Font BodyBold = new Font("Segoe UI", 12, FontStyle.Bold);
            public static readonly Font Small = new Font("Segoe UI", 10, FontStyle.Regular);
            public static readonly Font Caption = new Font("Segoe UI", 9, FontStyle.Regular);
        }

        // Spacing Constants
        public static class Spacing
        {
            public const int XSmall = 4;
            public const int Small = 8;
            public const int Medium = 16;
            public const int Large = 24;
            public const int XLarge = 32;
            public const int XXLarge = 48;
        }

        // Border Radius
        public static class BorderRadius
        {
            public const int Small = 4;
            public const int Medium = 8;
            public const int Large = 12;
            public const int XLarge = 16;
        }

        // Helper Methods
        public static Button CreateModernButton(string text, Color? backgroundColor = null, Color? textColor = null)
        {
            var button = new Button
            {
                Text = text,
                Font = Fonts.Body,
                FlatStyle = FlatStyle.Flat,
                BackColor = backgroundColor ?? Colors.Primary,
                ForeColor = textColor ?? Color.White,
                Cursor = Cursors.Hand,
                Padding = new Padding(Spacing.Medium, Spacing.Small, Spacing.Medium, Spacing.Small),
                MinimumSize = new Size(100, 36)
            };

            button.FlatAppearance.BorderSize = 0;

            // Hover effects
            button.MouseEnter += (s, e) => {
                button.BackColor = backgroundColor == Colors.Primary ? Colors.PrimaryDark : 
                                 backgroundColor == Colors.Secondary ? Colors.SecondaryLight :
                                 ControlPaint.Light(backgroundColor ?? Colors.Primary, 0.1f);
            };

            button.MouseLeave += (s, e) => {
                button.BackColor = backgroundColor ?? Colors.Primary;
            };

            return button;
        }

        public static Panel CreateCard(int padding = 0)
        {
            var panel = new Panel
            {
                BackColor = Colors.Surface,
                Padding = new Padding(padding > 0 ? padding : Spacing.Large),
                Margin = new Padding(Spacing.Small)
            };

            // Add shadow effect simulation
            panel.Paint += (s, e) => {
                var rect = panel.ClientRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                
                using (var pen = new Pen(Colors.Border))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            };

            return panel;
        }

        public static Label CreateHeading(string text, int level = 1)
        {
            var label = new Label
            {
                Text = text,
                ForeColor = Colors.TextPrimary,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, Spacing.Medium)
            };

            label.Font = level switch
            {
                1 => Fonts.Heading1,
                2 => Fonts.Heading2,
                3 => Fonts.Heading3,
                4 => Fonts.Heading4,
                _ => Fonts.Heading1
            };

            return label;
        }

        public static Label CreateBodyText(string text, bool muted = false)
        {
            return new Label
            {
                Text = text,
                Font = Fonts.Body,
                ForeColor = muted ? Colors.TextMuted : Colors.TextPrimary,
                AutoSize = true
            };
        }

        public static TextBox CreateModernTextBox(string placeholder = "")
        {
            var textBox = new TextBox
            {
                Font = Fonts.Body,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Colors.Surface,
                ForeColor = Colors.TextPrimary,
                Padding = new Padding(Spacing.Small),
                Height = 36
            };

            if (!string.IsNullOrEmpty(placeholder))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = Colors.TextMuted;

                textBox.Enter += (s, e) => {
                    if (textBox.Text == placeholder)
                    {
                        textBox.Text = "";
                        textBox.ForeColor = Colors.TextPrimary;
                    }
                };

                textBox.Leave += (s, e) => {
                    if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Text = placeholder;
                        textBox.ForeColor = Colors.TextMuted;
                    }
                };
            }

            return textBox;
        }

        public static void ApplyModernFormStyle(Form form)
        {
            form.BackColor = Colors.Background;
            form.Font = Fonts.Body;
            form.ForeColor = Colors.TextPrimary;
        }

        public static void ApplyModernPanelStyle(Panel panel)
        {
            panel.BackColor = Colors.Surface;
            panel.Padding = new Padding(Spacing.Large);
        }
    }
}
