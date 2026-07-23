// SHIFT+STRG+ALT+D = Debug Mode in Settings Window
using System;
using System.Drawing;
using System.Windows.Forms;

namespace c2flux
{
    public sealed class DebugClassForm : Form
    {
        private readonly AppLayout _layout;
        private SymbolPreviewPanel symbolPreviewPanel;

        public DebugClassForm(AppLayout layout)
        {
            _layout = layout;

            InitializeComponent();
            AntdThemeService.Apply(this, _layout);
        }

        private void InitializeComponent()
        {
            Text = "DebugClassForm";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(520, 280);
            MinimumSize = new Size(520, 280);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            symbolPreviewPanel = new SymbolPreviewPanel
            {
                Name = "symbolPreviewPanel",
                Dock = DockStyle.Fill,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText
            };

            Controls.Add(symbolPreviewPanel);
        }

        private sealed class SymbolPreviewPanel : Panel
        {
            private const int RowHeight = 48;
            private const int IconCellLeft = 24;
            private const int IconCellWidth = 24;
            private const int SymbolBoxSize = StatusSymbolRenderer.DefaultSymbolSize;

            public SymbolPreviewPanel()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.Clear(BackColor);

                DrawHeader(e.Graphics);
                DrawSymbolRow(e.Graphics, 0, "Information", StatusSymbolKind.Information);
                DrawSymbolRow(e.Graphics, 1, "Warnung", StatusSymbolKind.Warning);
                DrawSymbolRow(e.Graphics, 2, "Fehler", StatusSymbolKind.Error);
                DrawSymbolRow(e.Graphics, 3, "Systemordner-Hinweis", StatusSymbolKind.SystemDirectory);
            }

            private void DrawHeader(Graphics graphics)
            {
                Rectangle textBounds = new Rectangle(16, 12, Width - 32, 24);

                TextRenderer.DrawText(
                    graphics,
                    "Symbol-Preview",
                    Font,
                    textBounds,
                    ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            private void DrawSymbolRow(Graphics graphics, int index, string label, StatusSymbolKind symbolKind)
            {
                int y = 48 + index * RowHeight;

                RectangleF iconCellBounds = new RectangleF(
                    IconCellLeft,
                    y,
                    IconCellWidth,
                    RowHeight);

                RectangleF symbolBox = GetCenteredSymbolBox(iconCellBounds);
                Rectangle textBounds = new Rectangle(72, y, Width - 88, RowHeight);

                using (Pen separatorPen = new Pen(SystemColors.ControlLight))
                {
                    graphics.DrawLine(separatorPen, 16, y + RowHeight - 1, Width - 16, y + RowHeight - 1);
                }

                StatusSymbolRenderer.DrawSymbol(graphics, symbolBox, symbolKind);

                TextRenderer.DrawText(
                    graphics,
                    label,
                    Font,
                    textBounds,
                    ForeColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            private RectangleF GetCenteredSymbolBox(RectangleF containerBounds)
            {
                float left = containerBounds.Left + (containerBounds.Width - SymbolBoxSize) / 2F;
                float top = containerBounds.Top + (containerBounds.Height - SymbolBoxSize) / 2F;

                return new RectangleF(
                    (float)Math.Round(left),
                    (float)Math.Round(top),
                    SymbolBoxSize,
                    SymbolBoxSize);
            }
        }
    }
}
