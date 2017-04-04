namespace GTA_V_RTM_By_BISOON
{
    partial class Blipfrm
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
            this.blip_blipsComboBox = new DevExpress.XtraEditors.ComboBoxEdit();
            this.blip_colorComboBox = new DevExpress.XtraEditors.ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)(this.blip_blipsComboBox.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blip_colorComboBox.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // blip_blipsComboBox
            // 
            this.blip_blipsComboBox.Location = new System.Drawing.Point(12, 34);
            this.blip_blipsComboBox.Name = "blip_blipsComboBox";
            this.blip_blipsComboBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.blip_blipsComboBox.Size = new System.Drawing.Size(177, 20);
            this.blip_blipsComboBox.TabIndex = 19;
            // 
            // blip_colorComboBox
            // 
            this.blip_colorComboBox.Location = new System.Drawing.Point(12, 8);
            this.blip_colorComboBox.Name = "blip_colorComboBox";
            this.blip_colorComboBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.blip_colorComboBox.Properties.Items.AddRange(new object[] {
            "White",
            "Red",
            "Green",
            "Blue",
            "Yellow"});
            this.blip_colorComboBox.Size = new System.Drawing.Size(177, 20);
            this.blip_colorComboBox.TabIndex = 18;
            // 
            // Blipfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 63);
            this.ControlBox = false;
            this.Controls.Add(this.blip_blipsComboBox);
            this.Controls.Add(this.blip_colorComboBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Blipfrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Blip";
            ((System.ComponentModel.ISupportInitialize)(this.blip_blipsComboBox.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blip_colorComboBox.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public DevExpress.XtraEditors.ComboBoxEdit blip_blipsComboBox;
        public DevExpress.XtraEditors.ComboBoxEdit blip_colorComboBox;
    }
}