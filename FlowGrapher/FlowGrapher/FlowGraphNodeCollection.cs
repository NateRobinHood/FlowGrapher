using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher
{
    public class FlowGraphNodeCollection : List<FlowGraphNode>
    {
        //Constructor
        public FlowGraphNodeCollection()
        {

        }

        //Public Methods
        public FlowGraphNode NodeHittest(Point location)
        {
            FlowGraphNode targetNode = this.Where(c => c.Bounds.Contains(location)).FirstOrDefault();

            return targetNode;
        }
    }
}
