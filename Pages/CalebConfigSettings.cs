using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Phoenix.Configuration;

namespace CalExtension
{
  [Phoenix.SettingsCategory("Caleb config")]
  public partial class CalebConfigSettings : UserControl
  {
    public CalebConfigSettings()
    {
      InitializeComponent();


      CalebConfig.PropertyChanged += CalebConfig_PropertyChanged;
      Disposed += new EventHandler(CalebConfig_Disposed);

      chbxAutoHeal.Checked = CalebConfig.AutoHeal;
      chbxHealMoby.Checked = CalebConfig.HealMoby;
      chbxDestroySpiderWeb.Checked = CalebConfig.DestroySpiderWeb;
      chbxHandleFrozen.Checked = CalebConfig.HandleFrozen;
      renameCbx.SelectedValue = (int)CalebConfig.Rename;
      tbxHealMinDmg.Text = CalebConfig.HealMinDamagePerc.ToString();

      tbxAttackDelay.Text = CalebConfig.AttackDelay.ToString();
      chbxUseWatcher.Checked = CalebConfig.UseWatcher;

      cbxAutoHealType.SelectedValue = (int)CalebConfig.HealType;
      cbxAutoLootType.SelectedValue = (int)CalebConfig.Loot;

      chbxAutoTrainPoison.Checked = CalebConfig.TrainPoison;

      cbxCastMessageType.SelectedValue = (int)CalebConfig.CastMessageType;
      cbxEnemyHitsMessageType.SelectedValue = (int)CalebConfig.EnemyHitsMessageType;
      cbxFriendHitsMessageType.SelectedValue = (int)CalebConfig.FriendHitsMessageType;
      cbxPlayerHitsMessageType.SelectedValue = (int)CalebConfig.PlayerHitsMessageType;

      chbxUseWallTime.Checked = CalebConfig.UseWallTime;

      tbxLootItemTypes.Text = CalebConfig.LootItemTypes;
      tbxPlayerAliases.Text = CalebConfig.PlayerAliases;
    }

    private void CalebConfig_PropertyChanged(object sender, EventArgs e)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new EventHandler(CalebConfig_PropertyChanged), sender, e);
        return;
      }

      chbxAutoHeal.Checked = CalebConfig.AutoHeal;
      chbxHealMoby.Checked = CalebConfig.HealMoby;
      chbxDestroySpiderWeb.Checked = CalebConfig.DestroySpiderWeb;
      chbxHandleFrozen.Checked = CalebConfig.HandleFrozen;

      tbxHealMinDmg.Text = CalebConfig.HealMinDamagePerc.ToString();
      tbxAttackDelay.Text = CalebConfig.AttackDelay.ToString();
      chbxUseWatcher.Checked = CalebConfig.UseWatcher;

      cbxAutoHealType.SelectedValue = (int)CalebConfig.HealType;
      cbxAutoLootType.SelectedValue = (int)CalebConfig.Loot;
      renameCbx.SelectedValue = (int)CalebConfig.Rename;

      chbxAutoTrainPoison.Checked = CalebConfig.TrainPoison;

      cbxCastMessageType.SelectedValue = (int)CalebConfig.CastMessageType;
      cbxEnemyHitsMessageType.SelectedValue = (int)CalebConfig.EnemyHitsMessageType;
      cbxFriendHitsMessageType.SelectedValue = (int)CalebConfig.FriendHitsMessageType;
      cbxPlayerHitsMessageType.SelectedValue = (int)CalebConfig.PlayerHitsMessageType;

      chbxUseWallTime.Checked = CalebConfig.UseWallTime;

      tbxLootItemTypes.Text = CalebConfig.LootItemTypes;
      tbxPlayerAliases.Text = CalebConfig.PlayerAliases;
      
    }


    void CalebConfig_Disposed(object sender, EventArgs e)
    {
      CalebConfig.PropertyChanged -= new EventHandler(CalebConfig_PropertyChanged);
    }


    private void chbxDestroySpiderWeb_CheckedChanged(object sender, System.EventArgs e)
    {
      CalebConfig.DestroySpiderWeb = chbxDestroySpiderWeb.Checked;
    }


    private void chbxHealMoby_CheckedChanged(object sender, System.EventArgs e)
    {
      CalebConfig.HealMoby = chbxHealMoby.Checked;
    }


    private void chbxAutoHeal_CheckedChanged(object sender, System.EventArgs e)
    {
      CalebConfig.AutoHeal = chbxAutoHeal.Checked;
      Game.PrintMessage("AutoHeal: " + CalebConfig.AutoHeal);
    }

    private void chbxHandleFrozen_CheckedChanged(object sender, EventArgs e)
    {
      CalebConfig.HandleFrozen = chbxHandleFrozen.Checked;
    }


    private void renameCbx_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (renameCbx.SelectedValue != null)
        CalebConfig.Rename = (RenameType)((int)renameCbx.SelectedValue);
    }


    private void cbxAutoHealType_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (cbxAutoHealType.SelectedValue != null)
        CalebConfig.HealType = (AutoHealType)((int)cbxAutoHealType.SelectedValue);
    }

    private void cbxAutoLootType_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (cbxAutoLootType.SelectedValue != null)
        CalebConfig.Loot = (LootType)((int)cbxAutoLootType.SelectedValue);
    }

    private void TbxHealMinDmg_TextChanged(object sender, System.EventArgs e)
    {
      int val = 0;
      if (Int32.TryParse(tbxHealMinDmg.Text, out val))
        CalebConfig.HealMinDamagePerc = val;
    }

    private void TbxAttackDelay_TextChanged(object sender, System.EventArgs e)
    {
      int val = 0;
      if (Int32.TryParse(tbxAttackDelay.Text, out val))
        CalebConfig.AttackDelay = val;
    }

    private void chbxAutoTrainPoison_CheckedChanged(object sender, System.EventArgs e)
    {
      CalebConfig.TrainPoison = chbxAutoTrainPoison.Checked;
    }

    private void chbxUseWatcher_CheckedChanged(object sender, System.EventArgs e)
    {
      CalebConfig.UseWatcher = chbxUseWatcher.Checked;
    }

    private void cbxCastMessageType_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (cbxCastMessageType.SelectedValue != null)
        CalebConfig.CastMessageType = (MessagePrintType)((int)cbxCastMessageType.SelectedValue);
    }

    private void cbxEnemyHitsMessageType_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (cbxEnemyHitsMessageType.SelectedValue != null)
        CalebConfig.EnemyHitsMessageType = (MessagePrintType)((int)cbxEnemyHitsMessageType.SelectedValue);
    }

    private void cbxFriendHitsMessageType_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (cbxFriendHitsMessageType.SelectedValue != null)
        CalebConfig.FriendHitsMessageType = (MessagePrintType)((int)cbxFriendHitsMessageType.SelectedValue);
    }


    private void cbxPlayerHitsMessageType_SelectedValueChanged(object sender, System.EventArgs e)
    {
      if (cbxPlayerHitsMessageType.SelectedValue != null)
        CalebConfig.PlayerHitsMessageType = (MessagePrintType)((int)cbxPlayerHitsMessageType.SelectedValue);
    }

    private void chbxUseWallTime_CheckedChanged(object sender, System.EventArgs e)
    {
      CalebConfig.UseWallTime = chbxUseWallTime.Checked;
    }

    private void tbxLootItemTypes_TextChanged(object sender, System.EventArgs e)
    {
      CalebConfig.LootItemTypes = tbxLootItemTypes.Text;
    }

    private void tbxPlayerAliases_TextChanged(object sender, System.EventArgs e)
    {
      CalebConfig.PlayerAliases = tbxPlayerAliases.Text;
    }

  }
}
