using FlowGrapher.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher
{
    public class FlowGraphGrid : IDrawable
    {
        public event EventHandler<EventArgs> OnCellSizeChanged;

        //Const
        private const int CellWidth = 10;
        private const int CellHeight = 10;
        private const float MinimumZoomFactor = 0.5f; //when multiplied with CellWidth or CellHeight, must result in a minimum of 1 
        private Brush GridBackgroundColor = Brushes.LightSlateGray;
        private Pen GridLinesColor = Pens.SlateGray;

        //Private Variables
        private float m_zoomFactor = 1;
        //**********************************************
        // this might not go here, I think it belongs in
        // a class higher up, who ever owns the drawing
        // location and captures mouse events
        //**********************************************
        //private Point m_panStartingPoint = Point.Empty;
        //private Point m_panMovingPoint = Point.Empty;
        //private bool m_isPanning = false;
        //**********************************************

        //Constructors
        public FlowGraphGrid()
        {
        }

        //Public Properties
        public Size CurrentCellSize
        {
            get
            {
                return new Size(Convert.ToInt32(CellWidth * m_zoomFactor), Convert.ToInt32(CellHeight * m_zoomFactor));
            }
        }

        public float ZoomFactor
        {
            get
            {
                return m_zoomFactor;
            }
            set
            {
                if (value < MinimumZoomFactor && m_zoomFactor != MinimumZoomFactor)
                {
                    m_zoomFactor = MinimumZoomFactor;
                    OnCellSizeChanged?.Invoke(this, EventArgs.Empty);
                }
                else if(m_zoomFactor != value)
                {
                    m_zoomFactor = value;
                    OnCellSizeChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        //Public Methods
        //Snaps the point to top left of a cell
        public Point SnapToScaledGrid(Point point)
        {
            double x = Math.Round((double)point.X / CurrentCellSize.Width) * CurrentCellSize.Width;
            double y = Math.Round((double)point.Y / CurrentCellSize.Height) * CurrentCellSize.Height;
            return new Point((int)x, (int)y);
        }

        public GridCell GetCellByPoint(Point point)
        {
            int cellX = point.X / CurrentCellSize.Width;
            int cellY = point.Y / CurrentCellSize.Height;

            return new GridCell(cellX, cellY);
        }

        public Point GetPointByCell(GridCell cell)
        {
            int pointX = cell.X * CurrentCellSize.Width;
            int pointY = cell.Y * CurrentCellSize.Height;

            Point cellPoint = new Point(pointX, pointY);

            return SnapToScaledGrid(cellPoint);
        }

        #region IDrawable

        public void Draw(Graphics e)
        {
            //make sure the clip was set by the creator of the graphics object
            //if the graphics object was created by a control's paint event
            //the clipping should be set
            if (!e.Clip.IsInfinite(e))
            {
                RectangleF bounds = e.ClipBounds;

                e.FillRectangle(GridBackgroundColor, bounds);

                int rows = Convert.ToInt32(bounds.Height / CurrentCellSize.Height);
                int columns = Convert.ToInt32(bounds.Width / CurrentCellSize.Width);

                //draw horizontal grid lines
                for (int i = 0; i < rows; i++)
                {
                    Point rowStart = new Point(0, i * CurrentCellSize.Height);
                    Point rowEnd = new Point(Convert.ToInt32(bounds.Width), i * CurrentCellSize.Height);
                    e.DrawLine(GridLinesColor, rowStart, rowEnd);
                }

                //draw vertical grid lines
                for (int i = 0; i < columns; i++)
                {
                    Point columnStart = new Point(i * CurrentCellSize.Width, 0);
                    Point columnEnd = new Point(i * CurrentCellSize.Width, Convert.ToInt32(bounds.Height));
                    e.DrawLine(GridLinesColor, columnStart, columnEnd);
                }
            }
        }

        #endregion
    }

    public class GridCell
    {
        //Private Variables
        private int m_x = 0;
        private int m_y = 0;

        //Constructors
        public GridCell(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        //Public Properties
        public int X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }

        public int Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }

        public Point GridPoint
        {
            get
            {
                return FlowGraph.Grid.GetPointByCell(this);
            }
        }

        //Static Properties
        public static GridCell Empty
        {
            get
            {
                return new GridCell(0, 0);
            }
        }
    }
}
