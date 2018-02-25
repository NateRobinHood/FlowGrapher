using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using FlowGrapher.Resources;
using XSDFramework.WizardConfigFramework;
using XSDFramework;
using System.Xml.Linq;
using FlowGrapher.FlowGraphNodes;

namespace FlowGrapher.Components
{
    public partial class FlowGraphToolbox : UserControl
    {
        //Private Variables
        private List<ToolboxListBox.ToolBoxNode> m_flowGrahpNodeTypes = new List<ToolboxListBox.ToolBoxNode>();
        private ToolboxListBox listBoxTools;

        //Constructors
        public FlowGraphToolbox()
        {
            InitializeComponent();

            listBoxTools = new ToolboxListBox();
            listBoxTools.Dock = DockStyle.Fill;
            listBoxTools.BackColor = this.BackColor;
            listBoxTools.BorderStyle = BorderStyle.None;
            this.Controls.Add(listBoxTools);

            InitNodeTypeList();

            this.FontChanged += FlowGraphToolbox_FontChanged;
        }

        //Private Methods
        private void InitNodeTypeList()
        {
            //string appPath = Application.ExecutablePath;

            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            Stream flowGraphIconStream = thisAssembly.GetManifestResourceStream(ResourceManager.FlowGraphNodeIconImage);
            Image flowGraphIconImage = Image.FromStream(flowGraphIconStream);

            IEnumerable<ToolboxListBox.ToolBoxNode> thisAssemblyTypes = thisAssembly.GetTypes()
                .Where(c => c.GetCustomAttribute(typeof(FlowGraphNodeAttribute)) != null)
                .Select(c => new ToolboxListBox.ToolBoxNode(c, (FlowGraphNodeAttribute)c.GetCustomAttribute(typeof(FlowGraphNodeAttribute)), flowGraphIconImage));

            m_flowGrahpNodeTypes.AddRange(thisAssemblyTypes);

            listBoxTools.Items.AddRange(m_flowGrahpNodeTypes.ToArray());
        }

        //Event Handlers
        private void FlowGraphToolbox_FontChanged(object sender, EventArgs e)
        {
            listBoxTools.Font = this.Font;
        }
    }
}
