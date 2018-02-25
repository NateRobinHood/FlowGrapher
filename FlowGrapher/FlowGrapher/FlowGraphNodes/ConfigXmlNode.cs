using FlowGrapher.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher.FlowGraphNodes
{
    [FlowGraphNode("ConfigXml", "Container node for ICW configuration")]
    public class ConfigXmlNode : FlowGraphNode, IDrawable
    {
        private const string HeaderText = "Config Xml";

        public ConfigXmlNode()
        {
            InitPorts();
            NodeHeaderText = HeaderText;
        }

        public ConfigXmlNode(int cellX = 0, int cellY = 0) : base()
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
            base.InPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.InPort, "interfaceproviders"));
            base.InPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.InPort, "database"));
            base.InPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.InPort, "application"));
            base.InPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.InPort, "trace"));
            base.InPorts.Add(new FlowGraphPort(FlowGraphPort.PortType.InPort, "overrides"));

            //has no outports
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
