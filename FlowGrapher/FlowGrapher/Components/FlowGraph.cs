using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlowGrapher.FlowGraphNodes;
using System.Xml.Linq;
using XSDFramework;
using XSDFramework.WizardConfigFramework;
using System.Reflection;
using System.IO;
using FlowGrapher.Resources;

namespace FlowGrapher.Components
{
    public partial class FlowGraph : UserControl
    {
        //Private Variables
        private static FlowGraphGrid m_grid;
        private FlowGraphNodeCollection m_flowGraphNodes = new FlowGraphNodeCollection();
        private Dictionary<FlowGraphNode, Point> m_panningLocations = new Dictionary<FlowGraphNode, Point>();
        private FlowGraphNode m_dragDropNode = null;
        private FlowGraphNode m_selectedNode = null;
        private FlowGraphLink m_selectedLink = null;
        private Point m_panStartingPoint = Point.Empty;
        private Point m_panMovingPoint = Point.Empty;
        private bool m_isPanning = false;
        private bool m_isMoving = false;
        private bool m_isLinking = false;
        private bool m_isDragDropping = false;

        //Constructors
        public FlowGraph()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.Paint += FlowGraph_Paint;
            this.ResizeRedraw = true;
            this.MouseWheel += FlowGraph_MouseWheel;
            this.MouseDown += FlowGraph_MouseDown;
            this.MouseUp += FlowGraph_MouseUp;
            this.MouseMove += FlowGraph_MouseMove;

            this.DragEnter += FlowGraph_DragEnter;
            this.DragLeave += FlowGraph_DragLeave;
            this.DragOver += FlowGraph_DragOver;
            this.DragDrop += FlowGraph_DragDrop;

            this.AllowDrop = true;

            //test code
            //m_flowGraphNodes.Add(new ConfigXmlNode(15,6));
            //m_flowGraphNodes.Add(new ProviderNode(8,4));
            //m_flowGraphNodes.Add(new ServiceNode(2,2));
            //m_flowGraphNodes.Add(new ServiceNode(2,5));
            //m_flowGraphNodes.Add(new ServiceNode(2,8));
        }

        //Public Properties
        public static FlowGraphGrid Grid
        {
            get
            {
                if (m_grid == null)
                    m_grid = new FlowGraphGrid();

                return m_grid;
            }
        }

        //Private Methods
        private void ValidateLinks()
        {

        }

        //Event Handlers
        private void FlowGraph_Paint(object sender, PaintEventArgs e)
        {
            FlowGraph.Grid.Draw(e.Graphics);
            foreach (FlowGraphNode node in m_flowGraphNodes)
            {
                if (node is IDrawable)
                {
                    ((IDrawable)node).Draw(e.Graphics);
                }
            }

            IEnumerable<FlowGraphLink> links = m_flowGraphNodes.SelectMany(c => c.NodeLinks).Distinct();
            foreach (FlowGraphLink link in links)
            {
                if (link is IDrawable)
                {
                    ((IDrawable)link).Draw(e.Graphics);
                }
            }

            if (m_selectedLink != null)
            {
                ((IDrawable)m_selectedLink).Draw(e.Graphics);
            }

            if (m_dragDropNode != null)
            {
                ((IDrawable)m_dragDropNode).Draw(e.Graphics);
            }
        }

        private void FlowGraph_MouseWheel(object sender, MouseEventArgs e)
        {
            FlowGraph.Grid.ZoomFactor += (e.Delta * .001f);
            this.Invalidate();
        }

        private void FlowGraph_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                m_selectedNode = m_flowGraphNodes.Where(c => c.Bounds.Contains(e.Location)).FirstOrDefault();
                if (m_selectedNode != null)
                {
                    FlowGraphPort selectedPort = m_selectedNode.PortGripHittest(e.Location);
                    if (selectedPort != null)
                    {
                        while (selectedPort.Links.Count() > 0)
                        {
                            selectedPort.Links.First().Remove();
                        }

                        return;
                    }
                }
            }

            m_selectedNode = m_flowGraphNodes.NodeHittest(e.Location);
            if (m_selectedNode != null)
            {
                //check if the mouse is on a port grip
                FlowGraphPort selectedPort = m_selectedNode.PortGripHittest(e.Location);
                if (selectedPort != null && selectedPort.Type == FlowGraphPort.PortType.OutPort)
                {
                    m_isLinking = true;
                    m_selectedLink = new FlowGraphLink();

                    m_selectedNode.RegisterLink(selectedPort, m_selectedLink);
                    m_selectedLink.LinkEnd = e.Location;
                }
                else
                {
                    //do panning
                    m_isMoving = true;

                    m_panStartingPoint = new Point(e.Location.X - m_selectedNode.Location.X,
                                        e.Location.Y - m_selectedNode.Location.Y);

                    this.Cursor = Cursors.SizeAll;
                }
            }
            else
            {
                m_isPanning = true;

                foreach (FlowGraphNode node in m_flowGraphNodes)
                {
                    m_panningLocations.Add(node, node.Location);
                }
                m_panStartingPoint = new Point(e.Location.X, e.Location.Y);

                this.Cursor = Cursors.SizeAll;
            }
        }

        private void FlowGraph_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_isLinking)
            {
                FlowGraphNode targetNode = m_flowGraphNodes.NodeHittest(e.Location);
                if (targetNode != null)
                {
                    FlowGraphPort selectedPort = targetNode.PortGripHittest(e.Location);
                    if (selectedPort != null && selectedPort.Type == FlowGraphPort.PortType.InPort)
                    {
                        targetNode.RegisterLink(selectedPort, m_selectedLink);
                    }
                    else
                    {
                        m_selectedLink.Remove();
                    }
                }
                else
                {
                    m_selectedLink.Remove();
                }
            }

            m_selectedNode = null;
            m_isMoving = false;

            m_panningLocations.Clear();
            m_isPanning = false;

            m_selectedLink = null;
            m_isLinking = false;
            this.Cursor = Cursors.Hand;

            this.Invalidate();
        }

        private void FlowGraph_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_isMoving)
            {
                m_panMovingPoint = new Point(e.Location.X - m_panStartingPoint.X,
                                        e.Location.Y - m_panStartingPoint.Y);
                if (m_selectedNode != null)
                {
                    //m_selectedNode.Location = m_panMovingPoint;
                    m_selectedNode.Location = FlowGraph.Grid.SnapToScaledGrid(m_panMovingPoint);
                    m_selectedNode.Cell = FlowGraph.Grid.GetCellByPoint(m_selectedNode.Location);
                }
                this.Invalidate();
            }
            if (m_isPanning)
            {
                //need a way to store original locationon the nodes
                m_panMovingPoint = new Point(e.Location.X - m_panStartingPoint.X,
                                        e.Location.Y - m_panStartingPoint.Y);

                foreach (FlowGraphNode node in m_flowGraphNodes)
                {
                    Point newLocation = m_panningLocations[node];
                    newLocation.Offset(m_panMovingPoint.X, m_panMovingPoint.Y);

                    node.Location = FlowGraph.Grid.SnapToScaledGrid(newLocation);
                    node.Cell = FlowGraph.Grid.GetCellByPoint(node.Location);
                }

                this.Invalidate();
            }
            if (m_isLinking)
            {
                if (m_selectedLink != null)
                {
                    m_selectedLink.LinkEnd = e.Location;
                }
                this.Invalidate();
            }
        }

        private void FlowGraph_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ToolboxListBox.ToolBoxNode)))
            {
                ToolboxListBox.ToolBoxNode dragDropInfo = e.Data.GetData(typeof(ToolboxListBox.ToolBoxNode)) as ToolboxListBox.ToolBoxNode;
                if (dragDropInfo != null)
                {
                    m_dragDropNode = Activator.CreateInstance(dragDropInfo.ItemType) as FlowGraphNode;
                    if (m_dragDropNode != null)
                    {
                        m_dragDropNode.SetTransparency(125);

                        e.Effect = DragDropEffects.Move;
                        m_isDragDropping = true;

                        this.Invalidate();

                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void FlowGraph_DragOver(object sender, DragEventArgs e)
        {
            if (m_isDragDropping)
            {
                if (m_dragDropNode != null)
                {
                    Point mousePoint = this.PointToClient(new Point(e.X, e.Y));
                    m_dragDropNode.Location = FlowGraph.Grid.SnapToScaledGrid(mousePoint);
                    m_dragDropNode.Cell = FlowGraph.Grid.GetCellByPoint(mousePoint);
                }
                this.Invalidate();
            }
        }

        private void FlowGraph_DragLeave(object sender, EventArgs e)
        {
            if (m_isDragDropping)
            {
                m_isDragDropping = false;
                m_dragDropNode = null;

                this.Invalidate();
            }
        }

        private void FlowGraph_DragDrop(object sender, DragEventArgs e)
        {
            if (m_isDragDropping && m_dragDropNode != null)
            {
                m_dragDropNode.SetTransparency(255);
                m_flowGraphNodes.Add(m_dragDropNode);
            }

            m_isDragDropping = false;
            m_dragDropNode = null;

            this.Invalidate();
        }
    }
}
