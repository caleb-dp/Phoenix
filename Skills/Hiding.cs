using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;

namespace CalExtension.Skills
{
  public class Hiding : Skill
  {
    //---------------------------------------------------------------------------------------------
    public static bool HideRunning = false;
    public void HideDruid(int wait)
    {
      Journal.Clear();
      Game.CurrentGame.CurrentPlayer.SwitchWarmode();

      StandardSkill usedSkill = StandardSkill.Hiding;

      SkillValue hidingSV = SkillsHelper.GetSkillValue("Hiding");
      SkillValue stealthSV = SkillsHelper.GetSkillValue("Stealth");

      if (stealthSV.RealValue > hidingSV.RealValue)
        usedSkill = StandardSkill.Stealth;

  
      Game.RunScript(3000);


      UO.UseSkill(usedSkill);


      AsyncCounter counter = new AsyncCounter();
      counter.PrefixText = "Vracecka: ";
      counter.HighlightTime = 1500;
      counter.Step = 400;
      counter.StopMessage = "You can't seem to hide here,You have hidden yourself well,You can't do much in your";
      counter.StopMethod = IsHidden;
      counter.Run();

      if (World.Player.Backpack.AllItems.FindType(0x1B17, 0x0493).Exist)
      {
        Game.Wait(wait);
        UO.UseType(0x1B17, 0x0493);
      }
      else
        World.Player.PrintMessage("Nemas artefakt!");

    }

    //---------------------------------------------------------------------------------------------
    private static Graphic DropItemGraphic = Graphic.Invariant;
    private static UOColor DropItemColor = UOColor.Invariant;


    public void Hide()
    {
      Hide(1500, 0, 400, Graphic.Invariant, UOColor.Invariant);
    }

    public void Hide(int highlightTime, ushort highlightColor, int counterStep, Graphic dropItemGraphic, UOColor dropItemColor)
    {
      Journal.Clear();
      Game.CurrentGame.CurrentPlayer.SwitchWarmode();

      StandardSkill usedSkill = StandardSkill.Hiding;

      SkillValue hidingSV = SkillsHelper.GetSkillValue("Hiding");
      SkillValue stealthSV = SkillsHelper.GetSkillValue("Stealth");

      if (stealthSV.RealValue > hidingSV.RealValue)
        usedSkill = StandardSkill.Stealth;

      if (World.Player.Layers[Layer.LeftHand].Graphic == 0x0A15)//lantern
      {
        UOItem shield = new UOItem(Serial.Invalid);

        List<UOItem> items = new List<UOItem>();
        items.AddRange(World.Player.Backpack.Items);

        foreach (UOItem item in items)
        {
          foreach (Graphic g in ItemLibrary.Shields.GraphicArray)
          {
            if (item.Graphic == g && (!shield.Exist || shield.Graphic != 0x1B76))
              shield = item;
          }
        }

        if (shield.Exist)
          shield.Use();
        else
          World.Player.Layers[Layer.LeftHand].Move(1, World.Player.Backpack, 100, 30);

        Game.Wait(150);
      }


      Game.RunScript(3000);

      AsyncCounter counter = new AsyncCounter();
      counter.PrefixText = "";
      if (highlightTime > 0)
        counter.HighlightTime = highlightTime;

      if (highlightColor > 0)
        counter.HighlightColor = highlightColor;


      if (!dropItemGraphic.IsInvariant)
      {
        int origDistance = World.FindDistance;
        World.FindDistance = 3;
        UOItem item = World.Ground.FindType(dropItemGraphic, dropItemColor);
        if (item.Exist && item.Distance <= 3)
        {
          item.Move(item.Amount, World.Player.Backpack);
        }
        World.FindDistance = origDistance;
      }

      Hiding.HideRunning = true;
      UO.UseSkill(usedSkill);
      Game.Wait(50);

      if (!dropItemGraphic.IsInvariant && World.Player.Backpack.AllItems.FindType(dropItemGraphic, dropItemColor).Exist)
      {
        DropItemGraphic = dropItemGraphic;
        DropItemColor = dropItemColor;

        counter.RunComplete += Counter_RunComplete;
      }


      counter.Step = counterStep;
      counter.StopMessage = "You can't seem to hide here,You have hidden yourself well";
      counter.StopMethod = IsHidden;
      counter.Run();
    }

    //---------------------------------------------------------------------------------------------

    private void Counter_RunComplete(object sender, EventArgs e)
    {
      ((AsyncCounter)sender).RunComplete -= Counter_RunComplete;

      //Game.PrintMessage("Counter_RunComplete");

      if (World.Player.Hidden && !DropItemGraphic.IsInvariant)
      {
        UOItem item = World.Player.Backpack.Items.FindType(DropItemGraphic, DropItemColor);
        if (item.Exist)
        {
          item.Move(1, World.Player.X, World.Player.Y, World.Player.Z);
        }
      }
      Game.RunScript(5);

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void DropPickItem(Graphic dropItemGraphic, UOColor dropItemColor)
    {
      int origDistance = World.FindDistance;
      World.FindDistance = 6;

      UOItem item = World.Ground.FindType(dropItemGraphic, dropItemColor);
      if (item.Exist)
      {
        if (item.Move(item.Amount, World.Player.Backpack))
          World.Player.PrintMessage("[ pickitem ]");
        else
          World.Player.PrintMessage("[ pickitem ]", MessageType.Error);
      }
      else
      {
        item = World.Player.Backpack.Items.FindType(dropItemGraphic, dropItemColor);
        if (item.Exist)
        {
          if (item.Move(item.Amount, World.Player.X, World.Player.Y, World.Player.Z))
            World.Player.PrintMessage("[ dropitem ]");
          else
            World.Player.PrintMessage("[ dropitem ]", MessageType.Error);
        }
      }


      World.FindDistance = origDistance;
    }

    //---------------------------------------------------------------------------------------------

    public void HideRay()
    {
      HideRay(1500, 0, 400);
    }

    public void HideRay(int highlightTime, ushort highlightColor, int counterStep)
    {
      Journal.Clear();

      //   SkillValue hidingSV = SkillsHelper.GetSkillValue("Hiding");
      // SkillValue stealthSV = SkillsHelper.GetSkillValue("Stealth");
      StandardSkill usedSkill = StandardSkill.Hiding;
      UO.UseSkill(usedSkill);

      bool needAction = false;
      if (Journal.WaitForText(true, 200, "You are preoccupied with thoughts of battle"))
      {
        Game.CurrentGame.CurrentPlayer.SwitchWarmode();
        needAction = true;

      }
      // if (stealthSV.RealValue > hidingSV.RealValue)
      // usedSkill = StandardSkill.Stealth;


      //You are preoccupied with thoughts of battle






      if (World.Player.Layers[Layer.LeftHand].Graphic == 0x0A15)//lantern
      {

        UOItem shield = new UOItem(Serial.Invalid);

        List<UOItem> items = new List<UOItem>();
        items.AddRange(World.Player.Backpack.Items);

        foreach (UOItem item in items)
        {
          foreach (Graphic g in ItemLibrary.Shields.GraphicArray)
          {
            if (item.Graphic == g && (!shield.Exist || shield.Graphic != 0x1B76))
              shield = item;
          }
        }

        if (shield.Exist)
          shield.Use();
        else
          World.Player.Layers[Layer.LeftHand].Move(1, World.Player.Backpack, 100, 30);

        Game.Wait(250);
      }


      Game.RunScript(3000);

      AsyncCounter counter = new AsyncCounter();
      counter.PrefixText = "";
      if (highlightTime > 0)
        counter.HighlightTime = highlightTime;

      if (highlightColor > 0)
        counter.HighlightColor = highlightColor;

      Hiding.HideRunning = true;

      if (needAction)
      {
        World.Player.PrintMessage("[ HID Action ]", MessageType.Warning);

        if (World.Player.Hits < World.Player.Strenght)
          UO.BandageSelf();
        else if (World.Player.Mana > 4)
          UO.Cast(StandardSpell.MagicArrow, World.Player.Serial);
      }

      Game.Wait(250);
      UO.UseSkill(usedSkill);
      Game.Wait(50);
      counter.Step = counterStep;
      counter.StopMessage = "You can't seem to hide here,You have hidden yourself well";
      counter.StopMethod = IsHidden;
      counter.Run();
    }



    //---------------------------------------------------------------------------------------------

    public void HideInBattle()
    {
      HideInBattle(1500, 0, 400);
    }

    public void HideInBattle(int highlightTime, ushort highlightColor, int counterStep)
    {
      if (World.Player.Layers[Layer.LeftHand].Graphic == 0x0A15)//lantern
      {
        UOItem shield = new UOItem(Serial.Invalid);

        List<UOItem> items = new List<UOItem>();
        items.AddRange(World.Player.Backpack.Items);

        foreach (UOItem item in items)
        {
          foreach (Graphic g in ItemLibrary.Shields.GraphicArray)
          {
            if (item.Graphic == g && (!shield.Exist || shield.Graphic != 0x1B76))
              shield = item;
          }
        }

        if (shield.Exist)
          shield.Use();
        else
          World.Player.Layers[Layer.LeftHand].Move(1, World.Player.Backpack, 100, 30);

        Game.Wait(250);
      }


      Journal.Clear();
      SkillValue hidingSV = SkillsHelper.GetSkillValue("Hiding");
      SkillValue stealthSV = SkillsHelper.GetSkillValue("Stealth");
      StandardSkill usedSkill = StandardSkill.Hiding;
      
      if (stealthSV.RealValue > hidingSV.RealValue)
        usedSkill = StandardSkill.Stealth;

      AsyncCounter counter = new AsyncCounter();
      counter.PrefixText = "";
      if (highlightTime > 0)
        counter.HighlightTime = highlightTime;

      if (highlightColor > 0)
        counter.HighlightColor = highlightColor;

      Game.RunScript(3000);
      UO.UseSkill(usedSkill);

      if (Journal.WaitForText(true, 200, "You are preoccupied with thoughts of battle"))
      {
        Game.CurrentGame.CurrentPlayer.SwitchWarmode();
        World.Player.PrintMessage("[ Preoccupied ]", MessageType.Warning);
        Game.Wait(250);

        if (World.Player.Hits < World.Player.Strenght)
          UO.BandageSelf();
        else
          UO.UseSkill(StandardSkill.Meditation);

        //else if (World.Player.Mana > 4)
        //  UO.Cast(StandardSpell.MagicArrow, World.Player.Serial);

      }
      else
      {
        counter.Step = counterStep;
        counter.StopMessage = "You can't seem to hide here,You have hidden yourself well";
        counter.StopMethod = IsHidden;
        counter.Run();
      }


      //You are preoccupied with thoughts of battle

      //Game.RunScript(3500);



      //Hiding.HideRunning = true;

      //if (needAction)
      //{
      //  World.Player.PrintMessage("[ HID Action ]", MessageType.Warning);

      //  if (World.Player.Hits < World.Player.Strenght)
      //    UO.BandageSelf();
      //  else if (World.Player.Mana > 4)
      //    UO.Cast(StandardSpell.MagicArrow, World.Player.Serial);
      //}

      //Game.Wait(250);
      //UO.UseSkill(usedSkill);
      //Game.Wait(50);

    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void CraftBandana()
    {
      UOItem helma = World.Player.Layers[Layer.Hat];
      UOItem bandana = World.Player.FindType(Bandana.CraftBandana);

      if (!bandana.Exist)
        bandana = World.Player.FindType(Bandana.CraftBandana.Graphic);

      if (bandana.Exist)
      {
        if (bandana.Layer == Layer.Hat)
        {
          if (bandana.Move(1, World.Player.Backpack))
          {
            Game.Wait();
            bandana.Use();
          }
        }
        else
        {
          bandana.Use();
          Game.Wait(500);

          if (helma.Exist)
          {
            helma.Use();
          }
        }
      }
      else
      {
        World.Player.PrintMessage("[Nemas bandanu...]", MessageType.Error);
        ExecHide();
      }
     
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsHidden()
    {
      return World.Player.Hidden;
    }

    //---------------------------------------------------------------------------------------------

    #region exec
    
    [Executable("hide")]
    public static void ExecHide()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().Hide();
    }

    [Executable("hide")]
    public static void ExecHide(int highlightTime, ushort highlightColor, int counterStep)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().Hide(highlightTime, highlightColor, counterStep, Graphic.Invariant, UOColor.Invariant);
    }


    [Executable("hidedrop")]
    public static void ExecHideDrop(Graphic dropItemGraphic, UOColor dropItemColor)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().Hide(1500, 0, 400, dropItemGraphic, dropItemColor);
    }

    [Executable("hidedrop")]
    public static void ExecHideDrop(int highlightTime, ushort highlightColor, int counterStep, Graphic dropItemGraphic, UOColor dropItemColor)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().Hide(highlightTime, highlightColor, counterStep, dropItemGraphic, dropItemColor);
    }


    [Executable("hidedruid")]
    public static void ExecHideDruid(int wait)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().HideDruid(wait);
    }

    [Executable("hideray")]
    public static void ExecHideRay()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().HideRay();
    }



    [Executable("HideInBattle")]
    public static void ExecHideInBattle()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().HideInBattle();
    }

    #endregion
  }
}
