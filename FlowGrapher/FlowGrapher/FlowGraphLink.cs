using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher
{
    public class FlowGraphLink : IDrawable
    {
        public event EventHandler<EventArgs> OnRemoved;

        //Constants
        private Pen LinkPen = new Pen(Color.LightBlue, 2);
        private Brush LinkGripBrush = Brushes.LightBlue;
        private const bool Diagnostics = false;

        //Private Variables
        private Point m_linkStart = Point.Empty;
        private Point m_linkEnd = Point.Empty;

        //Constructors
        public FlowGraphLink()
        {
        }

        //Public Properties
        public Point LinkStart
        {
            get
            {
                return m_linkStart;
            }
            set
            {
                m_linkStart = value;
            }
        }

        public Point LinkEnd
        {
            get
            {
                return m_linkEnd;
            }
            set
            {
                m_linkEnd = value;
            }
        }

        public Point LinkMidPoint
        {
            get
            {
                return GetMidPoint(LinkStart, LinkEnd);
            }
        }

        public Point LinkBezier1
        {
            get
            {
                bool vertexUp = LinkStart.Y < LinkEnd.Y;
                int quad = GetQuadrant(LinkStart, LinkEnd);
                return GetSimpleVertexPoint(LinkStart, LinkMidPoint, vertexUp, quad);
                //return GetVertexPoint(LinkStart, LinkMidPoint, vertexUp);
            }
        }

        public Point LinkBezier2
        {
            get
            {
                bool vertexUp = LinkStart.Y < LinkEnd.Y;
                int quad = GetQuadrant(LinkStart, LinkEnd);
                return GetSimpleVertexPoint(LinkMidPoint, LinkEnd, !vertexUp, quad);
                //return GetVertexPoint(LinkMidPoint, LinkEnd, !vertexUp);
            }
        }

        public double LinkBezierDeflectionAngle
        {
            get
            {
                float xDiff = LinkEnd.X - LinkStart.X;
                float yDiff = LinkEnd.Y - LinkStart.Y;
                double angle = Math.Abs(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
                return angle;
            }
        }

        //Public Methods
        public void Remove()
        {
            OnRemoved?.Invoke(this, EventArgs.Empty);
        }

        //Private Methods
        private double GetDistance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        private Point GetMidPoint(Point p1, Point p2)
        {
            return new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        }

        private int GetQuadrant(Point orgin, Point p1)
        {
            if (p1.X > orgin.X && p1.Y > orgin.Y)
                return 2;
            else if (p1.X < orgin.X && p1.Y > orgin.Y)
                return 3;
            else if (p1.X > orgin.X && p1.Y < orgin.Y)
                return 1;
            else
                return 4;
        }

        private bool PointInValid(Point p1)
        {
            if (Math.Abs(p1.X) == int.MaxValue || Math.Abs(p1.Y) == int.MaxValue)
                return false;

            return true;
        }

        private double DegreeGate(double degree)
        {
            if (degree > 85 && degree <= 90)
                return 85;

            if (degree >= 90 && degree < 95)
                return 95;

            if (degree > 175 && degree <= 180)
                return 175;

            return degree;
        }

        private GraphicsPath GetLinkArrow(float x, float y, float width, float height)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(x, y, x + width, y + (height / 2));
            gp.AddLine(x + width, y + (height / 2), x, y + height);
            gp.AddLine(x, y + height, x, y);
            gp.CloseFigure();

            return gp;
        }

        private Point GetSimpleVertexPoint(Point p1, Point p2, bool additive, int quadrant)
        {
            double baseLength = GetDistance(p1, p2);
            double coordsTheda = LinkBezierDeflectionAngle * (Math.PI / 180.0); //the angle from the coordinate plane

            double baseDegrees = DegreeGate(LinkBezierDeflectionAngle);
            double baseTheda = (baseDegrees) * (Math.PI / 180.0); //put in terms of radians
            //gate the basetheda at 85 degrees

            double calcVertexHeight = (baseLength / 2) * Math.Tan(baseTheda); //calculates the vertext height of the isosceles triangle

            double calcAddRatioX = Math.Sin(coordsTheda);
            double calcAddRatioY = Math.Cos(coordsTheda);

            Point vertexBaseIntersect = GetMidPoint(p1, p2);

            switch (quadrant)
            {
                case 1:
                    if(additive)
                        return new Point(vertexBaseIntersect.X - (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y - (int)(calcVertexHeight * calcAddRatioY));
                    else
                        return new Point(vertexBaseIntersect.X + (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y + (int)(calcVertexHeight * calcAddRatioY));
                case 2:
                    if(additive)
                        return new Point(vertexBaseIntersect.X + (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y - (int)(calcVertexHeight * calcAddRatioY));
                    else
                        return new Point(vertexBaseIntersect.X - (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y + (int)(calcVertexHeight * calcAddRatioY));
                case 3:
                    if(additive)
                        return new Point(vertexBaseIntersect.X + (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y - (int)(calcVertexHeight * calcAddRatioY));
                    else
                        return new Point(vertexBaseIntersect.X - (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y + (int)(calcVertexHeight * calcAddRatioY));
                case 4:
                    if(additive)
                        return new Point(vertexBaseIntersect.X + (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y + (int)(calcVertexHeight * calcAddRatioY));
                    else
                        return new Point(vertexBaseIntersect.X - (int)(calcVertexHeight * calcAddRatioX), vertexBaseIntersect.Y - (int)(calcVertexHeight * calcAddRatioY));
            }

            Console.WriteLine(string.Format("calcVertexHeight({0}) calcAddRatioX({1}) calcAddRatioY({2})", calcVertexHeight, calcAddRatioX, calcAddRatioY));

            return Point.Empty;
        }

        #region IDrawable

        public void Draw(Graphics e)
        {
            e.DrawBezier(LinkPen, LinkStart, LinkBezier1, LinkBezier2, LinkEnd);
            e.FillEllipse(LinkGripBrush, new Rectangle(LinkMidPoint.X - 3, LinkMidPoint.Y - 3, 6, 6));
            e.FillPath(LinkGripBrush, GetLinkArrow(LinkEnd.X - 4, LinkEnd.Y - 4, 8, 8));

            if (Diagnostics)
            {
                //draw test points
                e.DrawEllipse(Pens.Red, LinkBezier1.X - 1, LinkBezier1.Y - 1, 2, 2);
                e.DrawEllipse(Pens.Red, LinkBezier2.X - 1, LinkBezier2.Y - 1, 2, 2);

                e.DrawEllipse(Pens.Purple, LinkStart.X - 1, LinkStart.Y - 1, 2, 2);
                e.DrawEllipse(Pens.Purple, LinkMidPoint.X - 1, LinkMidPoint.Y - 1, 2, 2);
                e.DrawEllipse(Pens.Purple, LinkEnd.X - 1, LinkEnd.Y - 1, 2, 2);

                e.DrawLine(Pens.OrangeRed, LinkStart, LinkBezier1);
                e.DrawLine(Pens.OrangeRed, LinkBezier1, LinkMidPoint);
                e.DrawLine(Pens.OrangeRed, LinkMidPoint, LinkStart);

                e.DrawLine(Pens.OrangeRed, LinkMidPoint, LinkBezier2);
                e.DrawLine(Pens.OrangeRed, LinkBezier2, LinkEnd);
                e.DrawLine(Pens.OrangeRed, LinkEnd, LinkMidPoint);
                e.DrawString(LinkBezierDeflectionAngle.ToString(), new Font(FontFamily.GenericSerif, 12), Brushes.Black, LinkBezier2);
            }
        }

        #endregion
    }
}
