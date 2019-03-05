using System.Xml;

namespace CalExtension
{
  partial class CalebConfigSettings
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
      this.label1 = new System.Windows.Forms.Label();
      this.chbxAutoHeal = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.chbxHealMoby = new System.Windows.Forms.CheckBox();
      this.label3 = new System.Windows.Forms.Label();
      this.chbxDestroySpiderWeb = new System.Windows.Forms.CheckBox();
      this.chbxHandleFrozen = new System.Windows.Forms.CheckBox();
      this.label4 = new System.Windows.Forms.Label();
      this.renameCbx = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.cbxAutoHealType = new System.Windows.Forms.ComboBox();
      this.label7 = new System.Windows.Forms.Label();
      this.tbxHealMinDmg = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.cbxAutoLootType = new System.Windows.Forms.ComboBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.chbxAutoTrainPoison = new System.Windows.Forms.CheckBox();




      this.label10 = new System.Windows.Forms.Label();
      this.tbxAttackDelay = new System.Windows.Forms.TextBox();

      this.label11 = new System.Windows.Forms.Label();
      this.chbxUseWatcher = new System.Windows.Forms.CheckBox();

      this.label12 = new System.Windows.Forms.Label();
      this.cbxCastMessageType = new System.Windows.Forms.ComboBox();
      this.label13 = new System.Windows.Forms.Label();
      this.cbxEnemyHitsMessageType = new System.Windows.Forms.ComboBox();
      this.label14 = new System.Windows.Forms.Label();
      this.cbxFriendHitsMessageType = new System.Windows.Forms.ComboBox();

      this.label15 = new System.Windows.Forms.Label();
      this.chbxUseWallTime = new System.Windows.Forms.CheckBox();


      this.label16 = new System.Windows.Forms.Label();
      this.tbxLootItemTypes = new System.Windows.Forms.TextBox();

      this.label17 = new System.Windows.Forms.Label();
      this.cbxPlayerHitsMessageType = new System.Windows.Forms.ComboBox();

      this.label18 = new System.Windows.Forms.Label();
      this.tbxPlayerAliases = new System.Windows.Forms.TextBox();
      

      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 21);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "AutoHeal:";
      // 
      // chbxAutoHeal
      // 
      this.chbxAutoHeal.AutoSize = true;
      this.chbxAutoHeal.Location = new System.Drawing.Point(135, 21);
      this.chbxAutoHeal.Name = "chbxAutoHeal";
      this.chbxAutoHeal.Size = new System.Drawing.Size(15, 14);
      this.chbxAutoHeal.TabIndex = 1;
      this.chbxAutoHeal.UseVisualStyleBackColor = true;
      this.chbxAutoHeal.CheckedChanged += new System.EventHandler(this.chbxAutoHeal_CheckedChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 38);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(56, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Heal alies:";
      // 
      // chbxHealMoby
      // 
      this.chbxHealMoby.AutoSize = true;
      this.chbxHealMoby.Location = new System.Drawing.Point(135, 37);
      this.chbxHealMoby.Name = "chbxHealMoby";
      this.chbxHealMoby.Size = new System.Drawing.Size(15, 14);
      this.chbxHealMoby.TabIndex = 3;
      this.chbxHealMoby.UseVisualStyleBackColor = true;
      this.chbxHealMoby.CheckedChanged += new System.EventHandler(this.chbxHealMoby_CheckedChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 16);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(97, 13);
      this.label3.TabIndex = 4;
      this.label3.Text = "Destroy spiderweb:";
      // 
      // chbxDestroySpiderWeb
      // 
      this.chbxDestroySpiderWeb.AutoSize = true;
      this.chbxDestroySpiderWeb.Location = new System.Drawing.Point(135, 16);
      this.chbxDestroySpiderWeb.Name = "chbxDestroySpiderWeb";
      this.chbxDestroySpiderWeb.Size = new System.Drawing.Size(15, 14);
      this.chbxDestroySpiderWeb.TabIndex = 5;
      this.chbxDestroySpiderWeb.UseVisualStyleBackColor = true;
      this.chbxDestroySpiderWeb.CheckedChanged += new System.EventHandler(this.chbxDestroySpiderWeb_CheckedChanged);
      // 
      // chbxHandleFrozen
      // 
      this.chbxHandleFrozen.AutoSize = true;
      this.chbxHandleFrozen.Location = new System.Drawing.Point(135, 36);
      this.chbxHandleFrozen.Name = "chbxHandleFrozen";
      this.chbxHandleFrozen.Size = new System.Drawing.Size(15, 14);
      this.chbxHandleFrozen.TabIndex = 6;
      this.chbxHandleFrozen.UseVisualStyleBackColor = true;
      this.chbxHandleFrozen.CheckedChanged += new System.EventHandler(this.chbxHandleFrozen_CheckedChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 36);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(53, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Sipkuj se:";
      // 
      // renameCbx
      // 
      this.renameCbx.DisplayMember = "Value";
      this.renameCbx.FormattingEnabled = true;
      this.renameCbx.Location = new System.Drawing.Point(135, 56);
      this.renameCbx.Name = "renameCbx";
      this.renameCbx.Size = new System.Drawing.Size(121, 21);
      this.renameCbx.TabIndex = 8;
      this.renameCbx.ValueMember = "Key";

      renameCbx.DataSource = CalebConfig.Of<RenameType>();

      this.renameCbx.SelectedValueChanged += new System.EventHandler(this.renameCbx_SelectedValueChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 64);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(74, 13);
      this.label5.TabIndex = 9;
      this.label5.Text = "Prejmenovani:";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.cbxAutoHealType);
      this.groupBox1.Controls.Add(this.label7);
      this.groupBox1.Controls.Add(this.tbxHealMinDmg);
      this.groupBox1.Controls.Add(this.label6);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.chbxAutoHeal);
      this.groupBox1.Controls.Add(this.chbxHealMoby);

      this.groupBox1.Controls.Add(this.label18);
      this.groupBox1.Controls.Add(this.tbxPlayerAliases);

      this.groupBox1.Location = new System.Drawing.Point(376, 10);
      this.groupBox1.Name = "groupBox1";
      //this.groupBox1.Size = new System.Drawing.Size(420, 600);
      this.groupBox1.AutoSize = true;
      this.groupBox1.TabIndex = 10;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Heal / Players";
      // 
      // cbxAutoHealType
      // 
      this.cbxAutoHealType.DisplayMember = "Value";
      this.cbxAutoHealType.FormattingEnabled = true;
      this.cbxAutoHealType.Location = new System.Drawing.Point(135, 79);
      this.cbxAutoHealType.Name = "cbxAutoHealType";
      this.cbxAutoHealType.Size = new System.Drawing.Size(121, 21);
      this.cbxAutoHealType.TabIndex = 7;
      this.cbxAutoHealType.ValueMember = "Key";
      cbxAutoHealType.DataSource = CalebConfig.Of<AutoHealType>();
      this.cbxAutoHealType.SelectedValueChanged += new System.EventHandler(this.cbxAutoHealType_SelectedValueChanged);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 82);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(78, 13);
      this.label7.TabIndex = 6;
      this.label7.Text = "Auto heal type:";
      // 
      // tbxHealMinDmg
      // 
      this.tbxHealMinDmg.Location = new System.Drawing.Point(135, 53);
      this.tbxHealMinDmg.Name = "tbxHealMinDmg";
      this.tbxHealMinDmg.Size = new System.Drawing.Size(54, 20);
      this.tbxHealMinDmg.TabIndex = 5;
      this.tbxHealMinDmg.TextChanged+= new System.EventHandler(this.TbxHealMinDmg_TextChanged);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 56);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(63, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "Min Dmg %:";

      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(6, 110);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label18.Text = "Aliasy hracu:";

      // 
      // label16
      // 
      this.tbxPlayerAliases.Location = new System.Drawing.Point(6, 135);
      this.tbxPlayerAliases.Name = "tbxPlayerAliases";
      this.tbxPlayerAliases.Size = new System.Drawing.Size(350, 350);
      this.tbxPlayerAliases.TabIndex = 5;
      this.tbxPlayerAliases.Multiline = true;
      this.tbxPlayerAliases.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.tbxPlayerAliases.TextChanged += new System.EventHandler(this.tbxPlayerAliases_TextChanged);


      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.chbxAutoTrainPoison);
      this.groupBox2.Controls.Add(this.label9);
      this.groupBox2.Controls.Add(this.cbxAutoLootType);
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Controls.Add(this.label3);
      this.groupBox2.Controls.Add(this.chbxDestroySpiderWeb);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.chbxHandleFrozen);
      this.groupBox2.Controls.Add(this.renameCbx);
      this.groupBox2.Controls.Add(this.label4);

      this.groupBox2.Controls.Add(this.label10);
      this.groupBox2.Controls.Add(this.label11);

      this.groupBox2.Controls.Add(this.label12);
      this.groupBox2.Controls.Add(this.label13);
      this.groupBox2.Controls.Add(this.label14);
      this.groupBox2.Controls.Add(this.label15);
      this.groupBox2.Controls.Add(this.label16);
      this.groupBox2.Controls.Add(this.label17);

      this.groupBox2.Controls.Add(this.cbxCastMessageType);
      this.groupBox2.Controls.Add(this.cbxEnemyHitsMessageType);
      this.groupBox2.Controls.Add(this.cbxFriendHitsMessageType);
      this.groupBox2.Controls.Add(this.chbxUseWallTime);
      this.groupBox2.Controls.Add(this.tbxLootItemTypes);
      this.groupBox2.Controls.Add(this.cbxPlayerHitsMessageType);

      this.groupBox2.Controls.Add(this.tbxAttackDelay);
      this.groupBox2.Controls.Add(this.chbxUseWatcher);

      this.groupBox2.Location = new System.Drawing.Point(0, 10);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(364, 500);
      this.groupBox2.TabIndex = 11;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Nastaveni automatiky";
      // 
      // cbxAutoLootType
      // 
      this.cbxAutoLootType.DisplayMember = "Value";
      this.cbxAutoLootType.FormattingEnabled = true;
      this.cbxAutoLootType.Location = new System.Drawing.Point(135, 82);
      this.cbxAutoLootType.Name = "cbxAutoLootType";
      this.cbxAutoLootType.Size = new System.Drawing.Size(121, 21);
      this.cbxAutoLootType.TabIndex = 11;
      this.cbxAutoLootType.ValueMember = "Key";
      cbxAutoLootType.DataSource = CalebConfig.Of<LootType>();
      this.cbxAutoLootType.SelectedValueChanged += new System.EventHandler(this.cbxAutoLootType_SelectedValueChanged);

      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 85);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(75, 13);
      this.label8.TabIndex = 10;
      this.label8.Text = "Auto loot type:";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(6, 110);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(89, 13);
      this.label9.TabIndex = 12;
      this.label9.Text = "Auto train poison:";
      // 
      // chbxAutoTrainPoison
      // 
      this.chbxAutoTrainPoison.AutoSize = true;
      this.chbxAutoTrainPoison.Location = new System.Drawing.Point(135, 110);
      this.chbxAutoTrainPoison.Name = "chbxAutoTrainPoison";
      this.chbxAutoTrainPoison.Size = new System.Drawing.Size(15, 14);
      this.chbxAutoTrainPoison.TabIndex = 13;
      this.chbxAutoTrainPoison.UseVisualStyleBackColor = true;
      this.chbxAutoTrainPoison.CheckedChanged += new System.EventHandler(this.chbxAutoTrainPoison_CheckedChanged);

      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(6, 135);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label10.Text = "Pauza healu pri utoku (ms):";

      this.tbxAttackDelay.Location = new System.Drawing.Point(135, 135);
      this.tbxAttackDelay.Name = "tbxAttackDelay";
      this.tbxAttackDelay.Size = new System.Drawing.Size(54, 20);
     // this.tbxAttackDelay.TabIndex = 5;
      this.tbxAttackDelay.TextChanged += new System.EventHandler(this.TbxAttackDelay_TextChanged);

      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(6, 160);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label11.Text = "Pouzit listu Watcher:";

      // 
      // chbxUseWatcher
      // 
      this.chbxUseWatcher.AutoSize = true;
      this.chbxUseWatcher.Location = new System.Drawing.Point(135, 160);
      this.chbxUseWatcher.Name = "chbxAutoTrainPoison";
      this.chbxUseWatcher.Size = new System.Drawing.Size(15, 14);
      //this.chbxAutoTrainPoison.TabIndex = 13;
      this.chbxUseWatcher.UseVisualStyleBackColor = true;
      this.chbxUseWatcher.CheckedChanged += new System.EventHandler(this.chbxUseWatcher_CheckedChanged);


      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(6, 185);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label12.Text = "Kouzlici hlasky:";

      // 
      // cbxCastMessageType
      // 
      this.cbxCastMessageType.DisplayMember = "Value";
      this.cbxCastMessageType.FormattingEnabled = true;
      this.cbxCastMessageType.Location = new System.Drawing.Point(135, 185);
      this.cbxCastMessageType.Name = "cbxCastMessageType";
      this.cbxCastMessageType.Size = new System.Drawing.Size(121, 21);
      this.cbxCastMessageType.TabIndex = 8;
      this.cbxCastMessageType.ValueMember = "Key";

      cbxCastMessageType.DataSource = CalebConfig.Of<MessagePrintType>();

      this.cbxCastMessageType.SelectedValueChanged += new System.EventHandler(this.cbxCastMessageType_SelectedValueChanged);

      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(6, 210);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label13.Text = "Enemy hits:";

      // 
      // cbxEnemyHitsMessageType
      // 
      this.cbxEnemyHitsMessageType.DisplayMember = "Value";
      this.cbxEnemyHitsMessageType.FormattingEnabled = true;
      this.cbxEnemyHitsMessageType.Location = new System.Drawing.Point(135, 210);
      this.cbxEnemyHitsMessageType.Name = "cbxEnemyHitsMessageType";
      this.cbxEnemyHitsMessageType.Size = new System.Drawing.Size(121, 21);
      this.cbxEnemyHitsMessageType.TabIndex = 8;
      this.cbxEnemyHitsMessageType.ValueMember = "Key";

      cbxEnemyHitsMessageType.DataSource = CalebConfig.Of<MessagePrintType>();

      this.cbxEnemyHitsMessageType.SelectedValueChanged += new System.EventHandler(this.cbxEnemyHitsMessageType_SelectedValueChanged);

      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(6, 235);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label14.Text = "Friend hits:";

      // 
      // cbxFriendHitsMessageType
      // 
      this.cbxFriendHitsMessageType.DisplayMember = "Value";
      this.cbxFriendHitsMessageType.FormattingEnabled = true;
      this.cbxFriendHitsMessageType.Location = new System.Drawing.Point(135, 235);
      this.cbxFriendHitsMessageType.Name = "cbxFriendHitsMessageType";
      this.cbxFriendHitsMessageType.Size = new System.Drawing.Size(121, 21);
      this.cbxFriendHitsMessageType.TabIndex = 8;
      this.cbxFriendHitsMessageType.ValueMember = "Key";

      cbxFriendHitsMessageType.DataSource = CalebConfig.Of<MessagePrintType>();

      this.cbxFriendHitsMessageType.SelectedValueChanged += new System.EventHandler(this.cbxFriendHitsMessageType_SelectedValueChanged);


      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(6, 260);
      this.label17.Name = "label14";
      this.label17.Size = new System.Drawing.Size(89, 13);
      this.label17.Text = "Player hits:";

      // 
      // cbxPlayerHitsMessageType
      // 
      this.cbxPlayerHitsMessageType.DisplayMember = "Value";
      this.cbxPlayerHitsMessageType.FormattingEnabled = true;
      this.cbxPlayerHitsMessageType.Location = new System.Drawing.Point(135, 260);
      this.cbxPlayerHitsMessageType.Name = "cbxPlayerHitsMessageType";
      this.cbxPlayerHitsMessageType.Size = new System.Drawing.Size(121, 21);
      this.cbxPlayerHitsMessageType.TabIndex = 8;
      this.cbxPlayerHitsMessageType.ValueMember = "Key";

      cbxPlayerHitsMessageType.DataSource = CalebConfig.Of<MessagePrintType>();

      this.cbxPlayerHitsMessageType.SelectedValueChanged += new System.EventHandler(this.cbxPlayerHitsMessageType_SelectedValueChanged);


      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(6, 285);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label15.Text = "Odpocitavani sten:";

      // 
      // chbxUseWallTime
      // 
      this.chbxUseWallTime.AutoSize = true;
      this.chbxUseWallTime.Location = new System.Drawing.Point(135, 285);
      this.chbxUseWallTime.Name = "chbxUseWallTime";
      this.chbxUseWallTime.Size = new System.Drawing.Size(15, 14);
      //this.chbxAutoTrainPoison.TabIndex = 13;
      this.chbxUseWallTime.UseVisualStyleBackColor = true;
      this.chbxUseWallTime.CheckedChanged += new System.EventHandler(this.chbxUseWallTime_CheckedChanged);


      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(6, 310);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(89, 13);
      //this.label10.TabIndex = 12;
      this.label16.Text = "Typy itemu na loot:";

      // 
      // label16
      // 
      this.tbxLootItemTypes.Location = new System.Drawing.Point(135, 310);
      this.tbxLootItemTypes.Name = "tbxLootItemTypes";
      this.tbxLootItemTypes.Size = new System.Drawing.Size(220, 195);
      this.tbxLootItemTypes.TabIndex = 5;
      this.tbxLootItemTypes.Multiline = true;
      this.tbxLootItemTypes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.tbxLootItemTypes.TextChanged += new System.EventHandler(this.tbxLootItemTypes_TextChanged);

      XmlDocument doc = CalebConfig.GlobalDocument;

      
      if (doc != null)
      {
        XmlElement itemsEl = doc.DocumentElement.SelectSingleNode("Loot/Items") as XmlElement;
        if (itemsEl != null)
          this.label16.Visible = this.tbxLootItemTypes.Visible = false;
      }
        // 
        // CalebConfigSettings
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "CalebConfigSettings";
      this.Size = new System.Drawing.Size(799, 376);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }






    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox chbxAutoHeal;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckBox chbxHealMoby;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.CheckBox chbxDestroySpiderWeb;
    private System.Windows.Forms.CheckBox chbxHandleFrozen;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ComboBox renameCbx;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.TextBox tbxHealMinDmg;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.ComboBox cbxAutoHealType;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox cbxAutoLootType;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.CheckBox chbxAutoTrainPoison;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox tbxAttackDelay;

    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.CheckBox chbxUseWatcher;

    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.ComboBox cbxCastMessageType;

    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.ComboBox cbxEnemyHitsMessageType;

    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.ComboBox cbxFriendHitsMessageType;

    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.CheckBox chbxUseWallTime;

    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.TextBox tbxLootItemTypes;

    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.ComboBox cbxPlayerHitsMessageType;

    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.TextBox tbxPlayerAliases;

  }
}
