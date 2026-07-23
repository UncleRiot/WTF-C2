using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace c2flux
{
    public enum StatusSymbolKind
    {
        Information,
        Warning,
        Error,
        SystemDirectory
    }

    public static class StatusSymbolRenderer
    {
        public const int DefaultSymbolSize = 14;

        public static Bitmap CreateBitmap(StatusSymbolKind symbolKind)
        {
            return CreateBitmap(symbolKind, DefaultSymbolSize);
        }

        public static Bitmap CreateBitmap(StatusSymbolKind symbolKind, int symbolSize)
        {
            int size = Math.Max(1, symbolSize);
            Bitmap bitmap = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                ConfigureGraphics(graphics);
                graphics.Clear(Color.Transparent);
                DrawSymbol(graphics, new RectangleF(0, 0, size, size), symbolKind);
            }

            return bitmap;
        }

        public static void DrawSymbol(Graphics graphics, RectangleF symbolBox, StatusSymbolKind symbolKind)
        {
            if (graphics == null)
                return;

            ConfigureGraphics(graphics);

            switch (symbolKind)
            {
                case StatusSymbolKind.Information:
                    DrawCircleSymbol(
                        graphics,
                        symbolBox,
                        Color.FromArgb(0, 120, 212),
                        Color.FromArgb(0, 90, 158),
                        Color.White,
                        "i",
                        0F);
                    break;

                case StatusSymbolKind.Warning:
                    DrawWarningTriangleSymbol(graphics, symbolBox);
                    break;

                case StatusSymbolKind.Error:
                    DrawSquareSymbol(
                        graphics,
                        symbolBox,
                        Color.FromArgb(196, 43, 28),
                        Color.FromArgb(135, 24, 15),
                        Color.White,
                        "×",
                        0F);
                    break;

                default:
                    DrawCircleSymbol(
                        graphics,
                        symbolBox,
                        Color.FromArgb(255, 140, 0),
                        Color.FromArgb(178, 92, 0),
                        Color.Black,
                        "!",
                        0F);
                    break;
            }
        }

        public static void DrawTreeExpandGlyph(Graphics graphics, RectangleF glyphBox, bool expanded)
        {
            if (graphics == null)
                return;

            ConfigureGraphics(graphics);

            RectangleF bounds = GetShapeBounds(glyphBox);

            using (SolidBrush fillBrush = new SolidBrush(SystemColors.Window))
            using (Pen borderPen = new Pen(SystemColors.ControlDark, 1F))
            using (Pen linePen = new Pen(SystemColors.WindowText, 1F))
            {
                graphics.FillRectangle(fillBrush, bounds);
                graphics.DrawRectangle(
                    borderPen,
                    bounds.Left,
                    bounds.Top,
                    bounds.Width,
                    bounds.Height);

                float centerX = bounds.Left + bounds.Width / 2F;
                float centerY = bounds.Top + bounds.Height / 2F;

                graphics.DrawLine(
                    linePen,
                    bounds.Left + 2F,
                    centerY,
                    bounds.Right - 2F,
                    centerY);

                if (!expanded)
                {
                    graphics.DrawLine(
                        linePen,
                        centerX,
                        bounds.Top + 2F,
                        centerX,
                        bounds.Bottom - 2F);
                }
            }
        }

        private static void ConfigureGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }

        private static void DrawCircleSymbol(Graphics graphics, RectangleF symbolBox, Color fillColor, Color borderColor, Color textColor, string symbolText, float textOffsetY)
        {
            RectangleF bounds = GetShapeBounds(symbolBox);

            using (GraphicsPath circlePath = new GraphicsPath())
            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            using (Pen borderPen = new Pen(borderColor, 1F))
            {
                circlePath.AddEllipse(bounds);
                graphics.FillPath(fillBrush, circlePath);
                graphics.DrawPath(borderPen, circlePath);
            }

            DrawCenteredSymbolText(graphics, symbolBox, textColor, symbolText, textOffsetY);
        }

        private static void DrawWarningTriangleSymbol(Graphics graphics, RectangleF symbolBox)
        {
            RectangleF bounds = GetShapeBounds(symbolBox);

            PointF topPoint = new PointF(
                bounds.Left + bounds.Width / 2F,
                bounds.Top);

            PointF rightPoint = new PointF(
                bounds.Right,
                bounds.Bottom);

            PointF leftPoint = new PointF(
                bounds.Left,
                bounds.Bottom);

            using (GraphicsPath trianglePath = new GraphicsPath())
            using (SolidBrush fillBrush = new SolidBrush(Color.FromArgb(255, 185, 0)))
            using (Pen borderPen = new Pen(Color.FromArgb(180, 125, 0), 1F))
            {
                trianglePath.AddPolygon(new[] { topPoint, rightPoint, leftPoint });
                graphics.FillPath(fillBrush, trianglePath);
                graphics.DrawPath(borderPen, trianglePath);
            }

            DrawCenteredSymbolText(graphics, symbolBox, Color.Black, "!", 1F);
        }

        private static void DrawSquareSymbol(Graphics graphics, RectangleF symbolBox, Color fillColor, Color borderColor, Color textColor, string symbolText, float textOffsetY)
        {
            RectangleF bounds = GetShapeBounds(symbolBox);

            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            using (Pen borderPen = new Pen(borderColor, 1F))
            {
                graphics.FillRectangle(fillBrush, bounds);
                graphics.DrawRectangle(
                    borderPen,
                    bounds.Left,
                    bounds.Top,
                    bounds.Width,
                    bounds.Height);
            }

            DrawCenteredSymbolText(graphics, symbolBox, textColor, symbolText, textOffsetY);
        }

        private static RectangleF GetShapeBounds(RectangleF symbolBox)
        {
            return new RectangleF(
                symbolBox.Left + 0.5F,
                symbolBox.Top + 0.5F,
                symbolBox.Width - 1F,
                symbolBox.Height - 1F);
        }

        private static void DrawCenteredSymbolText(Graphics graphics, RectangleF symbolBox, Color textColor, string symbolText, float offsetY)
        {
            using (GraphicsPath textPath = new GraphicsPath())
            using (FontFamily fontFamily = new FontFamily("Segoe UI"))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                textPath.AddString(
                    symbolText,
                    fontFamily,
                    (int)FontStyle.Bold,
                    9F,
                    Point.Empty,
                    StringFormat.GenericTypographic);

                RectangleF textBounds = textPath.GetBounds();

                float targetCenterX = symbolBox.Left + symbolBox.Width / 2F;
                float targetCenterY = symbolBox.Top + symbolBox.Height / 2F + offsetY;
                float textCenterX = textBounds.Left + textBounds.Width / 2F;
                float textCenterY = textBounds.Top + textBounds.Height / 2F;

                using (Matrix transformMatrix = new Matrix())
                {
                    transformMatrix.Translate(
                        targetCenterX - textCenterX,
                        targetCenterY - textCenterY);

                    textPath.Transform(transformMatrix);
                }

                graphics.FillPath(textBrush, textPath);
            }
        }
    }
}
