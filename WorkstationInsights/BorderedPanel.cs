using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WorkstationInsights
{
    public class BorderedPanel : Panel
    {
        private Color _borderColor = Color.FromArgb(225, 225, 230);
        private int _borderWidth = 1;
        private int _borderRadius = 0;

        public BorderedPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                this.Invalidate();
            }
        }

        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                _borderWidth = value;
                this.Invalidate();
            }
        }

        public int BorderRadius
        {
            get { return _borderRadius; }
            set
            {
                _borderRadius = value;
                this.Invalidate();
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            using (var path = GetRoundedRect(this.ClientRectangle, _borderRadius))
            using (var borderPen = new Pen(_borderColor, _borderWidth))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                // Draw the border
                if (_borderWidth > 0)
                {
                    var borderRect = new Rectangle(
                        _borderWidth / 2,
                        _borderWidth / 2,
                        this.Width - _borderWidth,
                        this.Height - _borderWidth);
                    
                    using (var borderPath = GetRoundedRect(borderRect, _borderRadius))
                    {
                        e.Graphics.DrawPath(borderPen, borderPath);
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

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
    }
}
