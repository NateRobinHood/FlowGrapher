using FlowGrapher.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher.FlowGraphNodes
{
    [FlowGraphNode("Service", "Defines a service configuration xml element")]
    public class ServiceNode : FlowGraphNode, IDrawable
    {
        private const string HeaderText = "Service";

        public ServiceNode()
        {
            InitPorts();
            NodeHeaderText = HeaderText;
        }

        public ServiceNode(int cellX = 0, int cellY = 0) : base()
        {
            InitPorts();
            NodeHeaderText = HeaderText;

            if (cellX != 0 || cellY != 0)
            {
                this.Cell = new GridCell(cellX, cellY);
                this.Location = FlowGraph.Grid.GetPointByCell(this.Cell);
            }
        }

        //Private Methods
        private void InitPorts()
        {
            //has no in ports

            base.OutPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.OutPort, "Out"));
        }

        #region IDrawable

        public void Draw(Graphics e)
        {
            base.DrawBackground(e);
            base.DrawHeader(e);
            base.DrawPorts(e);
        }

        #endregion
    }
}
