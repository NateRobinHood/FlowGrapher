using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowGrapher
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FlowGraphNodeAttribute : Attribute
    {
        private string m_nodeName = string.Empty;
        private string m_description = string.Empty;

        public FlowGraphNodeAttribute(string nodeName, string description)
        {
            this.m_nodeName = nodeName;
            this.m_description = description;
        }

        public string NodeName
        {
            get
            {
                return m_nodeName;
            }
        }

        public string Description
        {
            get
            {
                return m_description;
            }
        }
    }
}
