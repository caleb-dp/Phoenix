namespace CalExtension
{
  partial class CalebGlobalConfigSettings
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.tbxGlobalCalebConfig = new System.Windows.Forms.TextBox();

      this.groupBox1.SuspendLayout();
      this.SuspendLayout();

      this.groupBox1.Controls.Add(this.tbxGlobalCalebConfig);


      this.groupBox1.Location = new System.Drawing.Point(10, 10);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.AutoSize = true;
      this.groupBox1.TabIndex = 10;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Globalni XML konfigurace";

      this.tbxGlobalCalebConfig.Location = new System.Drawing.Point(10, 20);
      this.tbxGlobalCalebConfig.Name = "tbxLootItemTypes";
      this.tbxGlobalCalebConfig.Size = new System.Drawing.Size(800, 600);
      this.tbxGlobalCalebConfig.TabIndex = 5;
      this.tbxGlobalCalebConfig.Multiline = true;
      this.tbxGlobalCalebConfig.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbxGlobalCalebConfig.TextChanged += TbxGlobalCalebConfig_TextChanged; ;

      // 
      // CalebConfigSettings
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Name = "CalebConfigSettings";
      this.Size = new System.Drawing.Size(799, 376);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();

      this.ResumeLayout(false);

    }



    #endregion

    private System.Windows.Forms.GroupBox groupBox1;

    private System.Windows.Forms.TextBox tbxGlobalCalebConfig;

  }
}
