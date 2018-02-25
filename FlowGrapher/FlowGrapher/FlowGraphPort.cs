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
    public class FlowGraphPort
    {
        //Events
        public event EventHandler<EventArgs> OnPortPaintChange;

        //Private Variables
        private string m_portName = string.Empty;
        private Brush m_portBrush = Brushes.White;
        private GraphicsPath m_portGrip = null;
        private RectangleF m_portGripBounds = RectangleF.Empty;
        private RectangleF m_portBounds = RectangleF.Empty; //is set by the owning FlowGraphNode during paint operation
        private int m_portIndex = -1;
        private PortType m_portType;
        private List<FlowGraphLink> m_assignedLinks = new List<FlowGraphLink>();

        //Constructors
        public FlowGraphPort(PortType type)
        {
            m_portType = type;
        }

        public FlowGraphPort(PortType type, string portName)
        {
            m_portType = type;
            m_portName = portName;
        }

        //Static Methods
        public static void SetPortIndex(FlowGraphPort item, int portIndex)
        {
            item.PortIndex = portIndex;
        }

        public static void AssignLink(FlowGraphPort item, FlowGraphLink link)
        {
            item.AddLink(link);
        }

        //Public Properties
        public int PortIndex
        {
            get
            {
                return m_portIndex;
            }
            private set
            {
                m_portIndex = value;
                OnPortPaintChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public PortType Type
        {
            get
            {
                return m_portType;
            }
        }

        public IEnumerable<FlowGraphLink> Links
        {
            get
            {
                return m_assignedLinks;
            }
        }

        public GraphicsPath PortGrip
        {
            get
            {
                return m_portGrip;
            }
            //set
            //{
            //    m_portGrip = value;
            //}
        }

        public RectangleF PortGripBounds
        {
            get
            {
                return m_portGripBounds;
            }
        }

        public RectangleF PortBounds
        {
            get
            {
                return m_portBounds;
            }
            set
            {
                m_portBounds = value;
                SetGrip();
            }
        }

        public string PortName
        {
            get
            {
                return m_portName;
            }
            set
            {
                if (m_portName != value)
                {
                    m_portName = value;
                    OnPortPaintChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Brush PortBrush
        {
            get
            {
                return m_portBrush;
            }
            set
            {
                if (m_portBrush != value)
                {
                    m_portBrush = value;
                    OnPortPaintChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        //Public Methods
        public void SetTransparency(int transparency)
        {
            m_portBrush = new SolidBrush(Color.FromArgb(transparency, Color.White));
        }

        //Private Methods
        private void SetGrip()
        {
            float gripSize = m_portBounds.Height / 3;
            float gripY = ((m_portIndex + 1) * FlowGraph.Grid.CurrentCellSize.Height) + gripSize;
            switch (m_portType)
            {
                case PortType.InPort:
                    m_portGripBounds = new RectangleF(0, gripY, gripSize, gripSize);
                    m_portGrip = GetPortGripArrow(0, gripY, gripSize, gripSize);
                    break;
                case PortType.OutPort:
                    m_portGripBounds = new RectangleF(m_portBounds.Right, gripY, gripSize, gripSize);
                    m_portGrip = GetPortGripArrow(m_portBounds.Right, gripY, gripSize, gripSize);
                    break;
            }
        }

        private void AddLink(FlowGraphLink link)
        {
            if (!m_assignedLinks.Contains(link))
            {
                m_assignedLinks.Add(link);
                link.OnRemoved += Link_OnRemoved;
            }
        }

        //Event Handlers
        private void Link_OnRemoved(object sender, EventArgs e)
        {
            FlowGraphLink link = sender as FlowGraphLink;
            if (link != null)
            {
                if (m_assignedLinks.Contains(link))
                {
                    link.OnRemoved -= Link_OnRemoved;
                    m_assignedLinks.Remove(link);
                }
            }
        }

        private GraphicsPath GetPortGripArrow(float x, float y, float width, float height)
        {
            GraphicsPath gp = new GraphicsPath();

            gp.AddLine(x, y, x + width, y + (height / 2));
            gp.AddLine(x + width, y + (height / 2), x, y + height);
            gp.AddLine(x, y + height, x, y);
            gp.CloseFigure();

            return gp;
        }

        //Enum Definitions
        public enum PortType
        {
            InPort = 0,
            OutPort
        }
    }
}
