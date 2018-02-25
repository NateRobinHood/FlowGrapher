using FlowGrapher.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher
{
    public abstract class FlowGraphNode
    {
        //Constants
        private Brush NodeHeaderBrush = Brushes.Black;
        private Color BeginHeaderGradient = Color.FromArgb(221, 221, 221);
        private Color EndHeaderGradient = Color.FromArgb(57, 193, 57);
        private Brush NodeBackgroundBrush = Brushes.Silver;
        private Pen NodeBorderPen = new Pen(Brushes.DarkGray, 2);
        private Brush PortGripBrushIn = new SolidBrush(Color.Green);
        private Brush PortGripBrushOut = new SolidBrush(Color.Blue);
        private FontFamily PortFontFamily = new FontFamily("Comic Sans MS");

        //Private Variables
        private Point m_nodeLocation = Point.Empty;
        private RectangleF m_nodeBounds = RectangleF.Empty;
        private GridCell m_gridCell = GridCell.Empty;
        //******************************
        //used for the painting process
        private Size paint_cellSize;
        private Font paint_portFont;
        private Font paint_headerFont;
        private SizeF paint_inPortSize;
        private SizeF paint_outPortSize;
        private SizeF paint_nodeSize;
        private Rectangle paint_clip;
        private bool paint_initialized = false;
        private bool paint_cellSizeChanged = false;
        private bool paint_portsChanged = false;
        private bool paint_locationChanged = false;

        //Protected Variables
        protected string NodeHeaderText = string.Empty;

        //Constructors
        public FlowGraphNode()
        {
            InPorts = new FlowGraphPortCollection();
            OutPorts = new FlowGraphPortCollection();

            FlowGraph.Grid.OnCellSizeChanged += FlowGraphGrid_OnCellSizeChanged;
            InPorts.PortPaintChange += Ports_PortPaintChange;
            OutPorts.PortPaintChange += Ports_PortPaintChange;
        }

        //Public Properties
        public FlowGraphPortCollection InPorts
        {
            get;
            private set;
        }

        public FlowGraphPortCollection OutPorts
        {
            get;
            private set;
        }

        public IEnumerable<FlowGraphLink> NodeLinks
        {
            get
            {
                List<FlowGraphLink> links = new List<FlowGraphLink>();
                links.AddRange(InPorts.PortLinks);
                links.AddRange(OutPorts.PortLinks);

                return links;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(m_nodeLocation, new Size((int)paint_nodeSize.Width, (int)paint_nodeSize.Height));
            }
        }

        public Size SizeInCells
        {
            get
            {
                if (paint_initialized)
                    return new Size(Convert.ToInt32(paint_nodeSize.Width / paint_cellSize.Width), Convert.ToInt32(paint_nodeSize.Height / paint_cellSize.Height));
                else
                    return Size.Empty;
            }
        }

        public Point Location
        {
            get
            {
                return m_nodeLocation;
            }
            set
            {
                m_nodeLocation = value;
                paint_locationChanged = true;
            }
        }

        public GridCell Cell
        {
            get
            {
                return m_gridCell;
            }
            set
            {
                m_gridCell = value;
            }
        }

        //Private Methods
        private GraphicsPath GetRoundRect(float x, float y, float width, float height, float radius)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(x + radius, y, x + width - (radius * 2), y); // Line
            gp.AddArc(x + width - (radius * 2), y, radius * 2, radius * 2, 270, 90); // Corner
            gp.AddLine(x + width, y + radius, x + width, y + height - (radius * 2)); // Line
            gp.AddArc(x + width - (radius * 2), y + height - (radius * 2), radius * 2, radius * 2, 0, 90); // Corner
            gp.AddLine(x + width - (radius * 2), y + height, x + radius, y + height); // Line
            gp.AddArc(x, y + height - (radius * 2), radius * 2, radius * 2, 90, 90); // Corner
            gp.AddLine(x, y + height - (radius * 2), x, y + radius); // Line
            gp.AddArc(x, y, radius * 2, radius * 2, 180, 90); // Corner
            gp.CloseFigure();

            return gp;
        }

        private void UpdatePaintVars(Graphics e)
        {
            if(paint_cellSizeChanged || !paint_initialized)
                paint_cellSize = FlowGraph.Grid.CurrentCellSize;

            if (paint_cellSizeChanged || !paint_initialized)
            {
                paint_portFont = new Font(PortFontFamily, 20);
                while (paint_portFont.GetHeight() > paint_cellSize.Height)
                {
                    paint_portFont = new Font(PortFontFamily, paint_portFont.Size - 0.5f);
                }
            }

            if (paint_cellSizeChanged || paint_portsChanged || paint_locationChanged || !paint_initialized)
            {
                string longestInPortName = InPorts.Select(c => c.PortName).Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
                string longestOutPortName = OutPorts.Select(c => c.PortName).Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);

                float portGripSize = paint_cellSize.Height / 3;

                paint_inPortSize = e.MeasureString(longestInPortName, paint_portFont);
                paint_outPortSize = e.MeasureString(longestOutPortName, paint_portFont);

                float nodeWidth = (float)Math.Ceiling((paint_inPortSize.Width + paint_outPortSize.Width + (portGripSize * 2)) / paint_cellSize.Width) * paint_cellSize.Width;
                float nodeHeight = ((InPorts.Count > OutPorts.Count ? InPorts.Count : OutPorts.Count) + 1) * paint_cellSize.Height; //add two for the header and footer
                paint_nodeSize = new SizeF(nodeWidth, nodeHeight);

                for (int i = 0; i < InPorts.Count; i++)
                {
                    InPorts[i].PortBounds = new RectangleF(0 + portGripSize, (i + 1) * paint_cellSize.Height, paint_inPortSize.Width, paint_inPortSize.Height);
                }

                for (int i = 0; i < OutPorts.Count; i++)
                {
                    float portTextOffset = e.MeasureString(OutPorts[i].PortName, paint_portFont).Width + portGripSize;
                    OutPorts[i].PortBounds = new RectangleF(paint_nodeSize.Width - portTextOffset, (i + 1) * paint_cellSize.Height, paint_outPortSize.Width, paint_outPortSize.Height);
                }

                UpdateLinks();
            }

            if (paint_cellSizeChanged || !paint_initialized)
            {
                paint_headerFont = new Font(PortFontFamily, 20);
                while (paint_headerFont.GetHeight() > paint_cellSize.Height || e.MeasureString(NodeHeaderText, paint_headerFont).Width > paint_nodeSize.Width)
                {
                    paint_headerFont = new Font(PortFontFamily, paint_headerFont.Size - 0.5f);
                }
            }

                if (paint_cellSizeChanged || !paint_initialized)
            {
                paint_clip = new Rectangle(0, 0, (int)paint_nodeSize.Width, (int)paint_nodeSize.Height);
            }

            paint_initialized = true;
            paint_cellSizeChanged = false;
            paint_portsChanged = false;
            paint_locationChanged = false;
        }

        private void UpdateLinks()
        {
            foreach (FlowGraphPort inPort in InPorts)
            {
                foreach (FlowGraphLink link in inPort.Links)
                {
                    Rectangle portBounds = this.TranslatePortGrip(inPort);
                    link.LinkEnd = new Point((portBounds.Left + (portBounds.Width / 2)),
                        (portBounds.Top + (portBounds.Height / 2)));
                }
            }

            foreach (FlowGraphPort outPort in OutPorts)
            {
                foreach (FlowGraphLink link in outPort.Links)
                {
                    Rectangle portBounds = this.TranslatePortGrip(outPort);
                    link.LinkStart = new Point((portBounds.Left + (portBounds.Width / 2)),
                    (portBounds.Top + (portBounds.Height / 2)));
                }
            }
        }

        //Public Methods
        public FlowGraphPort PortGripHittest(Point location)
        {
            foreach (FlowGraphPort port in InPorts)
            {
                RectangleF portGripTranslate = new RectangleF(port.PortGripBounds.Location, port.PortGripBounds.Size);
                portGripTranslate.Offset(m_nodeLocation.X, m_nodeLocation.Y);

                if (portGripTranslate.Contains(location))
                {
                    return port;
                }
            }

            foreach (FlowGraphPort port in OutPorts)
            {
                RectangleF portGripTranslate = new RectangleF(port.PortGripBounds.Location, port.PortGripBounds.Size);
                portGripTranslate.Offset(m_nodeLocation.X, m_nodeLocation.Y);

                if (portGripTranslate.Contains(location))
                {
                    return port;
                }
            }

            return null;
        }

        public Rectangle TranslatePortGrip(FlowGraphPort port)
        {
            Rectangle portGripTranslate = new Rectangle((int)port.PortGripBounds.X, (int)port.PortGripBounds.Y, (int)port.PortGripBounds.Size.Width, (int)port.PortGripBounds.Size.Height);
            portGripTranslate.Offset(m_nodeLocation.X, m_nodeLocation.Y);

            return portGripTranslate;
        }

        public void RegisterLink(FlowGraphPort port, FlowGraphLink link)
        {
            FlowGraphPort.AssignLink(port, link);
            switch (port.Type)
            {
                case FlowGraphPort.PortType.InPort:
                    {
                        Rectangle portBounds = this.TranslatePortGrip(port);
                        link.LinkEnd = new Point((portBounds.Left + (portBounds.Width / 2)),
                            (portBounds.Top + (portBounds.Height / 2)));
                    }
                    break;
                case FlowGraphPort.PortType.OutPort:
                    {
                        Rectangle portBounds = this.TranslatePortGrip(port);
                        link.LinkStart = new Point((portBounds.Left + (portBounds.Width / 2)),
                        (portBounds.Top + (portBounds.Height / 2)));
                    }
                    break;
            }
        }

        public void SetTransparency(int transparency)
        {
            //Background Brushes
            NodeBackgroundBrush = new SolidBrush(Color.FromArgb(transparency, Color.Silver));
            NodeBorderPen = new Pen(Color.FromArgb(transparency, Color.DarkGray), 2);

            //Header Brushes
            BeginHeaderGradient = Color.FromArgb(transparency, 221, 221, 221);
            EndHeaderGradient = Color.FromArgb(transparency, 57, 193, 57);
            NodeHeaderBrush = new SolidBrush(Color.FromArgb(transparency, Color.Black));

            //Port Brushes
            PortGripBrushIn = new SolidBrush(Color.FromArgb(transparency, Color.Green));
            PortGripBrushOut = new SolidBrush(Color.FromArgb(transparency, Color.Blue));
            for (int i = 0; i < InPorts.Count; i++)
            {
                InPorts[i].SetTransparency(transparency);
            }
            for (int i = 0; i < OutPorts.Count; i++)
            {
                OutPorts[i].SetTransparency(transparency);
            }
        }

        public void DrawBackground(Graphics e)
        {
            UpdatePaintVars(e);

            //create a new graphics container for painting the node
            GraphicsContainer nodeContainer = e.BeginContainer();
            //set this new container's orgin to the node's location
            e.TranslateTransform(m_nodeLocation.X, m_nodeLocation.Y);

            Rectangle nodeShape = new Rectangle(0, 0, (int)paint_nodeSize.Width, (int)paint_nodeSize.Height);

            e.FillRectangle(NodeBackgroundBrush, nodeShape);
            e.DrawRectangle(NodeBorderPen, nodeShape);

            //end node graphics container
            e.EndContainer(nodeContainer);
        }

        public void DrawHeader(Graphics e)
        {
            UpdatePaintVars(e);

            //create a new graphics container for painting the node
            GraphicsContainer nodeContainer = e.BeginContainer();
            //set this new container's orgin to the node's location
            e.TranslateTransform(m_nodeLocation.X, m_nodeLocation.Y);

            Rectangle headerRect = new Rectangle(0, 0, (int)paint_nodeSize.Width, paint_cellSize.Height);
            LinearGradientMode mode = LinearGradientMode.Vertical;

            using (LinearGradientBrush b = new LinearGradientBrush(headerRect, BeginHeaderGradient, EndHeaderGradient, mode))
            {
                e.FillRectangle(b, headerRect);
            }

            e.DrawString(NodeHeaderText, paint_headerFont, NodeHeaderBrush, new PointF(3, 0));

            //end node graphics container
            e.EndContainer(nodeContainer);
        }

        public void DrawPorts(Graphics e)
        {
            UpdatePaintVars(e);

            //create a new graphics container for painting the node
            GraphicsContainer nodeContainer = e.BeginContainer();
            //set this new container's orgin to the node's location
            e.TranslateTransform(m_nodeLocation.X, m_nodeLocation.Y);
            e.SetClip(paint_clip, CombineMode.Replace);

            for (int i = 0; i < InPorts.Count; i++)
            {
                e.DrawString(InPorts[i].PortName, paint_portFont, InPorts[i].PortBrush, InPorts[i].PortBounds);
                e.FillPath(PortGripBrushIn, InPorts[i].PortGrip);
            }
            //e.DrawRectangles(PortGripPen, InPorts.Select(c => c.PortGrip).ToArray());

            for (int i = 0; i < OutPorts.Count; i++)
            {
                e.DrawString(OutPorts[i].PortName, paint_portFont, OutPorts[i].PortBrush, OutPorts[i].PortBounds);
                e.FillPath(PortGripBrushOut, OutPorts[i].PortGrip);
            }
            //e.DrawRectangles(PortGripPen, OutPorts.Select(c => c.PortGrip).ToArray());

            //end node graphics container
            e.EndContainer(nodeContainer);
        }

        //Event Handlers
        private void Ports_PortPaintChange(object sender, EventArgs e)
        {
            paint_portsChanged = true;
        }

        private void FlowGraphGrid_OnCellSizeChanged(object sender, EventArgs e)
        {
            paint_cellSizeChanged = true;
            Location = m_gridCell.GridPoint;
        }
    }
}
