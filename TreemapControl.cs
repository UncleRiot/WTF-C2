using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace c2flux
{
    public sealed class TreemapControl : Control
    {
        private FileSystemEntry _entry;

        public TreemapControl()
        {
            DoubleBuffered = true;
        }

        public void SetEntry(FileSystemEntry entry)
        {
            _entry = entry;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_entry == null || _entry.Children.Count == 0)
                return;

            List<FileSystemEntry> entries = _entry.Children
                .Where(item => item.SizeBytes > 0)
                .OrderByDescending(item => item.SizeBytes)
                .Take(100)
                .ToList();

            long total = entries.Sum(item => item.SizeBytes);
            if (total <= 0)
                return;

            Rectangle bounds = ClientRectangle;
            int x = bounds.Left;
            int y = bounds.Top;
            int width = bounds.Width;
            int height = bounds.Height;

            for (int index = 0; index < entries.Count; index++)
            {
                FileSystemEntry item = entries[index];
                bool horizontal = width >= height;
                double share = (double)item.SizeBytes / total;
                int length = horizontal
                    ? Math.Max(1, (int)Math.Round(width * share))
                    : Math.Max(1, (int)Math.Round(height * share));

                Rectangle rectangle = horizontal
                    ? new Rectangle(x, y, Math.Min(length, width), height)
                    : new Rectangle(x, y, width, Math.Min(length, height));

                int hash = Math.Abs(item.FullPath?.GetHashCode() ?? index);
                using SolidBrush brush = new SolidBrush(Color.FromArgb(80 + hash % 120, 80 + (hash / 7) % 120, 80 + (hash / 13) % 120));
                e.Graphics.FillRectangle(brush, rectangle);
                e.Graphics.DrawRectangle(Pens.Black, rectangle);

                string text = item.Name + Environment.NewLine + SizeFormatter.Format(item.SizeBytes);
                TextRenderer.DrawText(e.Graphics, text, Font, rectangle, Color.White, TextFormatFlags.EndEllipsis | TextFormatFlags.WordBreak);

                if (horizontal)
                {
                    x += rectangle.Width;
                    width -= rectangle.Width;
                }
                else
                {
                    y += rectangle.Height;
                    height -= rectangle.Height;
                }

                total -= item.SizeBytes;
                if (width <= 0 || height <= 0 || total <= 0)
                    break;
            }
        }
    }
}
