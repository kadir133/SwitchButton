using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms
{
    public class RoundedRectangleF
    {
        public readonly float Radius;
        GraphicsPath grPath;
        public RoundedRectangleF(float width, float height, float radius, float borderWidth, PointF point = default)
        {
            Radius = radius;
            float h = 2 * radius - borderWidth;
            float w = 2 * radius - borderWidth;
            float x1 = point.X;
            float y1 = point.Y;
            var x2 = x1 + width - (2 * radius);
            var y2 = y1 + height - (2 * radius);

            grPath = new GraphicsPath();
            if (radius <= 0)
            {
                grPath.AddRectangle(new RectangleF(x1, y1, width, height));
                return;
            }

            RectangleF upperLeftRect = new RectangleF(x1, y1, w, h);
            RectangleF lowerLeftRect = new RectangleF(x1, y2, w, h);
            RectangleF upperRightRect = new RectangleF(x2, y1, w, h);
            RectangleF lowerRightRect = new RectangleF(x2, y2, w, h);

            grPath.AddArc(upperLeftRect, 180, 90);
            grPath.AddArc(upperRightRect, 270, 90);
            grPath.AddArc(lowerRightRect, 0, 90);
            grPath.AddArc(lowerLeftRect, 90, 90);
            grPath.CloseAllFigures();
        }
        public GraphicsPath Path
        {
            get
            {
                return grPath;
            }
        }
    }
}
