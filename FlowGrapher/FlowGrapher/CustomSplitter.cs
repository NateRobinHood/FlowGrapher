using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowGrapher
{
    public partial class CustomSplitter : Control
    {
        //Private Variables
        private Rectangle m_gripRectangle = Rectangle.Empty;
        private ContainerControl m_container = new ContainerControl();
        private int m_startingX = default(int);
        private int m_movingX = default(int);
        private int m_startingWidth = default(int);
        private int m_startingLeft = default(int);
        private bool m_isSplitterMove = false;
        private bool m_isCollapsed = false;
        private int m_preCollapseWidth = default(int);

        //Constructors
        public CustomSplitter()
        {
            this.Dock = DockStyle.Left;
            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
            this.MinimumWidth = 10;
            this.GripWidth = 4;
            this.BorderColor = Color.DarkGray;

            this.Controls.Add(m_container);

            InitializeComponent();

            DockChanged += CustomSplitter_DockChanged;
            Resize += CustomSplitter_Resize;
            MouseMove += CustomSplitter_MouseMove;
            MouseDown += CustomSplitter_MouseDown;
            MouseUp += CustomSplitter_MouseUp;

            CalculateGridRectangle();
        }

        //Public Properties
        [Browsable(true)]
        public int MinimumWidth
        {
            get;
            set;
        }

        [Browsable(true)]
        public int GripWidth
        {
            get;
            set;
        }

        [Browsable(true)]
        public Color BorderColor
        {
            get;
            set;
        }

        [Browsable(true)]
        public string SplitterText
        {
            get;
            set;
        }

        [Browsable(false)]
        public ContainerControl InnerContainer
        {
            get
            {
                return m_container;
            }
        }

        //Public Methods
        public void Collapse()
        {
            m_preCollapseWidth = this.Width;
            this.Width = MinimumWidth;
            m_isCollapsed = true;
        }

        public void Expand()
        {
            if (this.Width == MinimumWidth && m_preCollapseWidth != default(int))
            {
                this.Width = m_preCollapseWidth;
                m_preCollapseWidth = default(int);
                m_isCollapsed = false;
            }
        }

        //Private Methods
        private void CalculateGridRectangle()
        {
            switch (this.Dock)
            {
                case DockStyle.Right:
                    m_gripRectangle = new Rectangle(0, 0, GripWidth, this.Height);
                    m_container.Location = new Point(GripWidth, 0);
                    m_container.Size = new Size(this.Width - GripWidth, this.Height);
                    break;
                case DockStyle.Left:
                    m_gripRectangle = new Rectangle(this.Right - GripWidth, this.Top, GripWidth, this.Height);
                    m_container.Location = new Point(0, 0);
                    m_container.Size = new Size(this.Width - GripWidth, this.Height);
                    break;
                case DockStyle.Bottom:
                case DockStyle.Top:
                case DockStyle.Fill:
                case DockStyle.None:
                    throw new NotImplementedException("CustomSplitter only supports DockStyle.Right and DockStyle.Left");
            }
        }

        private int GateWidth(int width)
        {
            return (width > MinimumWidth ? width : MinimumWidth);
        }

        //Event Overrides
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            using (Pen borderPen = new Pen(BorderColor))
            {
                switch (this.Dock)
                {
                    case DockStyle.Right:
                        pe.Graphics.DrawLine(borderPen, new Point(0, 0), new Point(0, this.Height));
                        break;
                    case DockStyle.Left:
                        pe.Graphics.DrawLine(borderPen, new Point(this.Right - 1, 0), new Point(this.Right - 1, this.Height));
                        break;
                }
            }
        }

        //Event Handlers
        private void CustomSplitter_DockChanged(object sender, EventArgs e)
        {
            CalculateGridRectangle();
        }

        private void CustomSplitter_Resize(object sender, EventArgs e)
        {
            CalculateGridRectangle();
            if (this.Width != MinimumWidth)
                m_isCollapsed = false;
        }

        private void CustomSplitter_MouseUp(object sender, MouseEventArgs e)
        {
            m_isSplitterMove = false;
            m_movingX = 0;
            m_startingX = 0;
            m_startingWidth = 0;
            m_startingLeft = 0;
            Cursor = Cursors.Default;
        }

        private void CustomSplitter_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_gripRectangle.Contains(e.Location))
            {
                m_startingX = this.PointToScreen(e.Location).X;
                m_startingWidth = this.Width;
                m_startingLeft = this.Left;

                m_isSplitterMove = true;
            }
        }

        private void CustomSplitter_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isSplitterMove)
            {
                if (m_gripRectangle.Contains(e.Location))
                    Cursor = Cursors.VSplit;
                else
                    Cursor = Cursors.Default;
            }
            else
            {
                m_movingX = this.PointToScreen(e.Location).X - m_startingX;
                switch (this.Dock)
                {
                    case DockStyle.Right:
                        this.Width = GateWidth(m_startingWidth - m_movingX);
                        break;
                    case DockStyle.Left:
                        this.Width = GateWidth(m_startingWidth + m_movingX);
                        break;
                }

                this.Invalidate();
            }
        }
    }
}
