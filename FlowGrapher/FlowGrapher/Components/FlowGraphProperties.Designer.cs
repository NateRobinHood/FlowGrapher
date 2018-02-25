namespace FlowGrapher.Components
{
    partial class FlowGraphProperties
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            propGridDisplay = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propGridDisplay
            // 
            //propGridDisplay.CategorySplitterColor = System.Drawing.Color.Silver;
            propGridDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            //propGridDisplay.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //propGridDisplay.HelpForeColor = System.Drawing.Color.Silver;
            //propGridDisplay.LineColor = System.Drawing.Color.Silver;
            propGridDisplay.Location = new System.Drawing.Point(0, 0);
            propGridDisplay.Name = "propGridDisplay";
            propGridDisplay.Size = new System.Drawing.Size(219, 466);
            propGridDisplay.TabIndex = 0;
            //propGridDisplay.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            // 
            // FlowGraphProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(propGridDisplay);
            this.Name = "FlowGraphProperties";
            this.Size = new System.Drawing.Size(219, 466);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
