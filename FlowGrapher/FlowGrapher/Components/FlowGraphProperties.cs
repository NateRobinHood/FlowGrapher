using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowGrapher.Components
{
    public partial class FlowGraphProperties : UserControl
    {
        private static PropertyGrid propGridDisplay;

        //Contrsuctor
        public FlowGraphProperties()
        {
            InitializeComponent();
        }

        //Public Methods
        public static void ViewProperties(object obj)
        {
            propGridDisplay.SelectedObject = obj;
        }
    }
}
