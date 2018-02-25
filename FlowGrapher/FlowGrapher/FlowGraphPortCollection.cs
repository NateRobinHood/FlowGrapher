using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher
{
    public class FlowGraphPortCollection: List<FlowGraphPort>
    {
        public event EventHandler<EventArgs> PortPaintChange;

        public FlowGraphPortCollection() : base()
        {

        }

        public new void Add(FlowGraphPort item)
        {
            item.OnPortPaintChange += Item_OnPortPaintChange;
            base.Add(item);
            SetPortIndexs();
        }

        public new void Remove(FlowGraphPort item)
        {
            item.OnPortPaintChange -= Item_OnPortPaintChange;
            base.Remove(item);
            SetPortIndexs();
        }

        //Public Properties
        public IEnumerable<FlowGraphLink> PortLinks
        {
            get
            {
                return this.SelectMany(c => c.Links);
            }
        }

        //Private Methods
        private void SetPortIndexs()
        {
            for (int i = 0; i < this.Count; i++)
            {
                FlowGraphPort.SetPortIndex(this[i], i);
            }
        }

        //Event Handlers
        private void Item_OnPortPaintChange(object sender, EventArgs e)
        {
            PortPaintChange?.Invoke(sender, e);
        }
    }
}
