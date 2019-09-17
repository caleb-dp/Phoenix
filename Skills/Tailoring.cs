using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;

namespace CalExtension.Skills
{
  public class Tailoring : Skill
  {

    public static UOItemType FoldedCloth { get { return new UOItemType() { Graphic = 0x175D, Color = 0x0000 }; } }
    public static UOItemType Bandy { get { return new UOItemType() { Graphic = 0x0E21, Color = 0x0000 }; } }
    public static UOItemType SewingKit { get { return new UOItemType() { Graphic = 0x0F9D, Color = 0x0000 }; } }
    public static UOItemType Nuzky { get { return new UOItemType() { Graphic = 0x0F9E, Color = 0x0000 }; } }
    public static UOItemType Bandana { get { return new UOItemType() { Graphic = 0x153F, Color = 0x0000 }; } }


    //---------------------------------------------------------------------------------------------

    public void TailoringTrain()
    {
      Game.PrintMessage("Zvol kontejner s materialem > (esc) = ground");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      Journal.Clear();

      while (!UO.Dead)
      {
        if (!World.Player.Backpack.Items.FindType(FoldedCloth.Graphic).Exist)
        {
          Game.PrintMessage("Doplnuji cloth");
          UOItem materials = new UOItem(Serial.Invalid);
          if (containerFrom.Exist)
            materials = containerFrom.Items.FindType(FoldedCloth.Graphic);

          if (materials.Exist)
            materials.Move(100, World.Player.Backpack);
          else
          {
            Game.PrintMessage("Neni cloth");
            break;
          }
          Game.Wait();

          UOItem bandages = World.Player.Backpack.Items.FindType(Bandy.Graphic);
          if (bandages.Exist)
            bandages.Move(1000, containerFrom);
          Game.Wait();
        }

        UO.UseType(SewingKit.Graphic);
        UO.WaitTargetType(FoldedCloth.Graphic);
        UO.WaitMenu("Cloth", "Headwear", "Headwear", "Bandana");

        Journal.WaitForText(true, 5000, "You have failed to make anything", "You can't make anything", "You put");
        if (Journal.Contains("You put"))
        {
          while (World.Player.Backpack.Items.FindType(Bandana.Graphic).Exist)
          {
            UO.UseType(Nuzky.Graphic);
            UO.WaitTargetType(Bandana.Graphic);
            Game.Wait(400);
          }
        }

        if (Journal.Contains("You can't make anything"))
        {
          Game.PrintMessage("Nemas suroviny");
        }

        Journal.Clear();
      }
      Game.PrintMessage("Konec treninku Tailor");
    }

    #region exec


    //---------------------------------------------------------------------------------------------
    [Executable]
    public void MakeBloodRobeF(int amount)
    {
      Game.PrintMessage("Vyber container s Blood ingy");
      UOItem containerIngyFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Cloth");
      UOItem containerClothFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Thread");
      UOItem containerThreadFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container, kam to chces dat");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerIngyFrom.AllItems.FindType(0x1BEF, 0x04C2).Move(15, World.Player.Backpack); // presun Blood ingots
        Game.Wait();

        containerClothFrom.AllItems.FindType(0x175D, 0x0000).Move(15, World.Player.Backpack); // presun Cloth
        Game.Wait();

        containerThreadFrom.AllItems.FindType(0x0FA0, 0x0000).Move(5, World.Player.Backpack); // presun Thread
        Game.Wait();

        UO.UseType(SewingKit.Graphic);
        UO.WaitTargetType(FoldedCloth.Graphic);
        UO.WaitMenu("Cloth", "Colored Robe", "Colored Robes", "Damske kovovky", "Damske kovovky", "Blood Rock Robe");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "Tailoring failed", "You can't make anything", "You put"))
        {
          if (Journal.Contains("You put"))
          {
            count += 1;
            World.Player.Backpack.AllItems.FindType(0x1F01, 0x04C2).Move(1, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }
        }

        Game.PrintMessage("Vyrobeno rob: " + count);

      }

      Game.PrintMessage("MakeBloodRobeF - End");
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public void MakeBloodRobeM(int amount)
    {
      Game.PrintMessage("Vyber container s Blood ingy");
      UOItem containerIngyFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Cloth");
      UOItem containerClothFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Thread");
      UOItem containerThreadFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container, kam to chces dat");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerIngyFrom.AllItems.FindType(0x1BEF, 0x04C2).Move(15, World.Player.Backpack); // presun Blood ingots
        Game.Wait();

        containerClothFrom.AllItems.FindType(0x175D, 0x0000).Move(15, World.Player.Backpack); // presun Cloth
        Game.Wait();

        containerThreadFrom.AllItems.FindType(0x0FA0, 0x0000).Move(5, World.Player.Backpack); // presun Thread
        Game.Wait();

        UO.UseType(SewingKit.Graphic);
        UO.WaitTargetType(FoldedCloth.Graphic);
        UO.WaitMenu("Cloth", "Colored Robe", "Colored Robes", "Panske kovovky", "Panske kovovky", "Blood Rock Robe");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "Tailoring failed", "You can't make anything", "You put"))
        {
          if (Journal.Contains("You put"))
          {
            count += 1;
            World.Player.Backpack.AllItems.FindType(0x1F03, 0x04C2).Move(1, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }
        }

        Game.PrintMessage("Vyrobeno rob: " + count);

      }

      Game.PrintMessage("MakeBloodRobeM - End");
    }


    [Executable("TailoringTrain")]
    [BlockMultipleExecutions]
    public static void ExecTailoringTrain()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Tailoring>().TailoringTrain();
    }

    #endregion
  }
}
