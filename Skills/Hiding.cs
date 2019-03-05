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

    public void Hide()
    {
      Hide(1500, 0, 400);
    }

    public void Hide(int highlightTime, ushort highlightColor, int counterStep)
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
      //UOItem whWepna = World.Player.FindColor(0x0B60, new Graphic[] { 0x143A, 0x1404, 0x0F62, 0x13B9, 0x0F5C, 0x1438, 0x0F60, 0x0F5E, 0x0E87 });        //Nightstone zbran u WJ
      //if (whWepna.Exist && whWepna.Layer != Layer.LeftHand && whWepna.Layer != Layer.RightHand)
      //{
      //  whWepna.Use();
      //  Game.Wait(150);
      //  Targeting.ResetTarget();

      //}

      Game.RunScript(3000);

      AsyncCounter counter = new AsyncCounter();
      counter.PrefixText = "";
      if (highlightTime > 0)
        counter.HighlightTime = highlightTime;

      if (highlightColor > 0)
        counter.HighlightColor = highlightColor;

      Hiding.HideRunning = true;
      UO.UseSkill(usedSkill);
      Game.Wait(50);
      counter.Step = counterStep;
      counter.StopMessage = "You can't seem to hide here,You have hidden yourself well";
      counter.StopMethod = IsHidden;
      counter.Run();
    }

    //---------------------------------------------------------------------------------------------

    public void HideRay()
    {
      HideRay(1500, 0, 400);
    }

    public void HideRay(int highlightTime, ushort highlightColor, int counterStep)
    {
      Journal.Clear();


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

      UO.BandageSelf();
      Game.Wait(100);
      UO.UseSkill(usedSkill);
      Game.Wait(50);
      counter.Step = counterStep;
      counter.StopMessage = "You can't seem to hide here,You have hidden yourself well";
      counter.StopMethod = IsHidden;
      counter.Run();
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void CraftBandana()
    {
      UOItem helma = World.Player.Layers[Layer.Hat];
      UOItem bandana = World.Player.FindType(Bandana.CraftBandana);

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
          Game.Wait(750);

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

    public bool IsHidden()
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
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().Hide(highlightTime, highlightColor, counterStep);
    }


    [Executable("hidedruid")]
    public static void ExecHideDruid(int wait)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Hiding>().HideDruid(wait);
    }


    #endregion
  }
}
