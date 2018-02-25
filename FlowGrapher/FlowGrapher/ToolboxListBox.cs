using FlowGrapher.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XSDFramework.WizardConfigFramework;

namespace FlowGrapher
{
    public partial class ToolboxListBox : ListBox
    {
        private int m_mouseIndex = -1;

        public ToolboxListBox()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.ItemHeight = 25;
            this.DrawItem += CustomListBox_DrawItem;

            this.MouseMove += CustomListBox_MouseMove;
            this.MouseLeave += CustomListBox_MouseLeave;
            this.MouseClick += ToolboxListBox_MouseClick;
            this.MouseDown += ToolboxListBox_MouseDown;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        //Event Hanlders
        private void CustomListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (this.Items.Count == 0)
                return;

            ToolBoxNode item = this.Items[e.Index] as ToolBoxNode;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.LightGray);
            }
            else if (e.Index == m_mouseIndex)
            {
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State,
                                          e.ForeColor,
                                          Color.DarkGray);
            }

            e.DrawBackground();

            Rectangle iconRect = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + ((e.Bounds.Height - item.Icon.Height) / 2), item.Icon.Width, item.Icon.Height);
            e.Graphics.DrawImage(item.Icon, iconRect);

            Rectangle fontRectangle = new Rectangle(e.Bounds.X + iconRect.Right + 2, e.Bounds.Y, e.Bounds.Width - iconRect.Width, e.Bounds.Height);
            StringFormat stringFormat = new StringFormat();
            //stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(item.ToString(), e.Font, new SolidBrush(e.ForeColor), fontRectangle, stringFormat);
        }

        private void CustomListBox_MouseMove(object sender, MouseEventArgs e)
        {
            int itemIndex = this.IndexFromPoint(e.Location);

            if (itemIndex == -1)
            {
                if (m_mouseIndex != -1)
                {
                    m_mouseIndex = -1;
                    this.Invalidate();
                }
                return;
            }

            if (m_mouseIndex != itemIndex)
            {
                m_mouseIndex = itemIndex;
                this.Invalidate();
            }
        }

        private void CustomListBox_MouseLeave(object sender, EventArgs e)
        {
            if (m_mouseIndex > -1)
            {
                m_mouseIndex = -1;
                this.Invalidate();
            }
        }

        private void ToolboxListBox_MouseClick(object sender, MouseEventArgs e)
        {
            int itemIndex = this.IndexFromPoint(e.Location);
            if (itemIndex == -1)
            {
                this.ClearSelected();
            }
        }

        private void ToolboxListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int itemIndex = this.IndexFromPoint(e.Location);
            if (itemIndex != -1)
            {
                ToolBoxNode node = Items[itemIndex] as ToolBoxNode;
                if (node != null)
                {
                    DoDragDrop(node, DragDropEffects.Move);
                }
            }
        }

        //Nested Classes
        public class ToolBoxNode
        {
            private Type m_itemType;
            private FlowGraphNodeAttribute m_flowGraphAttribute;
            private GenerateWizardControls.XSDElement m_xsdElement;
            private List<string> m_xmlValuePath;
            private Image m_icon;

            public ToolBoxNode(Type itemType, FlowGraphNodeAttribute itemAttribute, Image icon)
            {
                m_itemType = itemType;
                m_flowGraphAttribute = itemAttribute;
                m_icon = icon;
            }

            public ToolBoxNode(Type itemType, GenerateWizardControls.XSDElement xsdElement, Image icon)
            {
                m_itemType = itemType;
                m_xsdElement = xsdElement;
                m_icon = icon;
            }

            public ToolBoxNode(Type itemType, GenerateWizardControls.XSDElement xsdElement, List<string> xmlValuePath, Image icon)
            {
                m_itemType = itemType;
                m_xmlValuePath = xmlValuePath;
                m_xsdElement = xsdElement;
                m_icon = icon;
            }

            public Type ItemType
            {
                get
                {
                    return m_itemType;
                }
            }

            public GenerateWizardControls.XSDElement XsdElement
            {
                get
                {
                    return m_xsdElement;
                }
            }

            public List<string> XmlValuePath
            {
                get
                {
                    return m_xmlValuePath;
                }
            }

            public Image Icon
            {
                get
                {
                    return m_icon;
                }
            }

            public string NodeName
            {
                get
                {
                    if(m_flowGraphAttribute != null)
                        return m_flowGraphAttribute.NodeName;
                    if (m_xsdElement != null)
                        return m_xsdElement.ElementName;
                    return string.Empty;
                }
            }

            public string NodeDescription
            {
                get
                {
                    if(m_flowGraphAttribute != null)
                        return m_flowGraphAttribute.Description;
                    if (m_xsdElement != null)
                        return m_xsdElement.Description;
                    return string.Empty;
                }
            }

            public override string ToString()
            {
                return NodeName;
            }
        }
    }
}
