using FlowGrapher.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher.FlowGraphNodes
{
    [FlowGraphNode("Provider", "A collection of services")]
    public class ProviderNode : FlowGraphNode, IDrawable
    {
        private const string HeaderText = "Provider";

        public ProviderNode()
        {
            InitPorts();
            NodeHeaderText = HeaderText;
        }

        public ProviderNode(int cellX = 0, int cellY = 0) : base()
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
            base.InPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.InPort, "Service"));

            //has no outports
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
