namespace FlowGrapher
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scToolbox = new System.Windows.Forms.SplitContainer();
            this.nodeToolbox = new FlowGrapher.Components.FlowGraphToolbox();
            this.scProperties = new System.Windows.Forms.SplitContainer();
            this.flowGraphWindow = new FlowGrapher.Components.FlowGraph();
            this.flowGraphProperties = new FlowGrapher.Components.FlowGraphProperties();
            ((System.ComponentModel.ISupportInitialize)(this.scToolbox)).BeginInit();
            this.scToolbox.Panel1.SuspendLayout();
            this.scToolbox.Panel2.SuspendLayout();
            this.scToolbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scProperties)).BeginInit();
            this.scProperties.Panel1.SuspendLayout();
            this.scProperties.Panel2.SuspendLayout();
            this.scProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // scToolbox
            // 
            this.scToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scToolbox.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scToolbox.Location = new System.Drawing.Point(0, 0);
            this.scToolbox.Margin = new System.Windows.Forms.Padding(0);
            this.scToolbox.Name = "scToolbox";
            // 
            // scToolbox.Panel1
            // 
            this.scToolbox.Panel1.Controls.Add(this.nodeToolbox);
            // 
            // scToolbox.Panel2
            // 
            this.scToolbox.Panel2.Controls.Add(this.scProperties);
            this.scToolbox.Size = new System.Drawing.Size(1104, 593);
            this.scToolbox.SplitterDistance = 200;
            this.scToolbox.TabIndex = 1;
            // 
            // nodeToolbox
            // 
            this.nodeToolbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.nodeToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeToolbox.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nodeToolbox.Location = new System.Drawing.Point(0, 0);
            this.nodeToolbox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.nodeToolbox.Name = "nodeToolbox";
            this.nodeToolbox.Size = new System.Drawing.Size(200, 593);
            this.nodeToolbox.TabIndex = 0;
            // 
            // scProperties
            // 
            this.scProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scProperties.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.scProperties.Location = new System.Drawing.Point(0, 0);
            this.scProperties.Name = "scProperties";
            // 
            // scProperties.Panel1
            // 
            this.scProperties.Panel1.Controls.Add(this.flowGraphWindow);
            // 
            // scProperties.Panel2
            // 
            this.scProperties.Panel2.Controls.Add(this.flowGraphProperties);
            this.scProperties.Size = new System.Drawing.Size(900, 593);
            this.scProperties.SplitterDistance = 700;
            this.scProperties.TabIndex = 0;
            // 
            // flowGraphWindow
            // 
            this.flowGraphWindow.AllowDrop = true;
            this.flowGraphWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowGraphWindow.Location = new System.Drawing.Point(0, 0);
            this.flowGraphWindow.Margin = new System.Windows.Forms.Padding(0);
            this.flowGraphWindow.Name = "flowGraphWindow";
            this.flowGraphWindow.Size = new System.Drawing.Size(700, 593);
            this.flowGraphWindow.TabIndex = 0;
            // 
            // flowGraphProperties
            // 
            this.flowGraphProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flowGraphProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowGraphProperties.Location = new System.Drawing.Point(0, 0);
            this.flowGraphProperties.Name = "flowGraphProperties";
            this.flowGraphProperties.Size = new System.Drawing.Size(196, 593);
            this.flowGraphProperties.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(1104, 593);
            this.Controls.Add(this.scToolbox);
            this.Name = "MainForm";
            this.Text = "FlowGrapher";
            this.scToolbox.Panel1.ResumeLayout(false);
            this.scToolbox.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scToolbox)).EndInit();
            this.scToolbox.ResumeLayout(false);
            this.scProperties.Panel1.ResumeLayout(false);
            this.scProperties.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scProperties)).EndInit();
            this.scProperties.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Components.FlowGraph flowGraphWindow;
        private System.Windows.Forms.SplitContainer scToolbox;
        private System.Windows.Forms.SplitContainer scProperties;
        private Components.FlowGraphToolbox nodeToolbox;
        private Components.FlowGraphProperties flowGraphProperties;
    }
}

