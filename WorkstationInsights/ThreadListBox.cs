using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WorkstationInsights
{
    public class ThreadListBox : ListBox
    {
        private Color _selectedItemBackColor = Color.FromArgb(225, 243, 254);
        private Color _selectedItemBorderColor = Color.FromArgb(0, 120, 212);
        private Color _hoverItemBackColor = Color.FromArgb(243, 243, 243);
        private Color _itemTextColor = Color.FromArgb(30, 30, 30);
        private Color _selectedItemTextColor = Color.FromArgb(0, 90, 158);
        private int _itemHeight = 36;
        private int _leftPadding = 10;
        private int _cornerRadius = 4;

        public ThreadListBox()
        {
            DrawMode = DrawMode.OwnerDrawVariable;
            BorderStyle = BorderStyle.None;
            ItemHeight = _itemHeight;
            IntegralHeight = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                    ControlStyles.AllPaintingInWmPaint | 
                    ControlStyles.UserPaint | 
                    ControlStyles.ResizeRedraw, true);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool isHovered = ClientRectangle.Contains(PointToClient(Cursor.Position)) && 
                           e.Bounds.Contains(PointToClient(Cursor.Position));

            // Set up colors
            Color backColor = isSelected ? _selectedItemBackColor : 
                                 isHovered ? _hoverItemBackColor : BackColor;
            Color textColor = isSelected ? _selectedItemTextColor : _itemTextColor;
            Color borderColor = isSelected ? _selectedItemBorderColor : Color.Transparent;

            // Draw background
            using (var backBrush = new SolidBrush(backColor))
            using (var textBrush = new SolidBrush(textColor))
            using (var borderPen = new Pen(borderColor, 1.5f))
            {
                // Create a rounded rectangle for the item background
                Rectangle itemRect = new Rectangle(
                    e.Bounds.X + 2, 
                    e.Bounds.Y + 1, 
                    e.Bounds.Width - 4, 
                    e.Bounds.Height - 2);

                // Draw background
                using (var path = GetRoundedRect(itemRect, _cornerRadius))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(backBrush, path);
                    
                    // Draw border if selected
                    if (isSelected)
                    {
                        e.Graphics.DrawPath(borderPen, path);
                    }
                }


                // Draw text
                if (Items.Count > 0 && e.Index >= 0)
                {
                    string text = Items[e.Index].ToString();
                    var textRect = new Rectangle(
                        e.Bounds.X + _leftPadding,
                        e.Bounds.Y + (e.Bounds.Height - (int)e.Graphics.MeasureString(text, Font).Height) / 2,
                        e.Bounds.Width - (_leftPadding * 2),
                        e.Bounds.Height);

                    // Truncate text with ellipsis if needed
                    text = TruncateText(e.Graphics, text, textRect.Width, Font);

                    TextRenderer.DrawText(
                        e.Graphics,
                        text,
                        Font,
                        textRect,
                        textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
            }
        }


        private string TruncateText(Graphics g, string text, int maxWidth, Font font)
        {
            string truncated = text;
            var size = TextRenderer.MeasureText(g, truncated, font);
            
            if (size.Width > maxWidth)
            {
                // Start with the full string and remove characters until it fits
                int maxLength = text.Length;
                int minLength = 1;
                int mid = 0;
                
                while (minLength <= maxLength)
                {
                    mid = (minLength + maxLength) / 2;
                    string test = text.Substring(0, mid) + "...";
                    size = TextRenderer.MeasureText(g, test, font);
                    
                    if (size.Width < maxWidth)
                    {
                        minLength = mid + 1;
                        truncated = test;
                    }
                    else
                    {
                        maxLength = mid - 1;
                    }
                }
            }
            
            return truncated;
        }

        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            // Top left arc  
            path.AddArc(arc, 180, 90);

            // Top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);
            e.ItemHeight = _itemHeight;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            Invalidate();
        }
    }
}
