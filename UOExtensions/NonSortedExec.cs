using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;
using CalExtension.Skills;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Caleb.Library.CAL;

namespace CalExtension.UOExtensions
{
  public class NonSortedExec
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void DeleteNoTree()
    {
      Caleb.Library.CAL.Business.UOPositionCollection positions = new Caleb.Library.CAL.Business.UOPositionCollection();
      positions.Load();
      int remove = 0;
      Game.PrintMessage("Positions: " + positions.Count);
      foreach (Caleb.Library.CAL.Business.UOPosition p in positions)
      {
        if (!p.IsTree)
        {
          remove++;
          p.Remove();
        }
      }

      Game.PrintMessage("Rmove: " + remove);
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void DestroySpiderWeb()
    {
      foreach (UOItem item in World.Ground)
      {
        if (item.Distance <= 1)
        {
          if (
            item.Graphic == (new Graphic(0x0EE1)) ||
            item.Graphic == (new Graphic(0x0EE2)) ||
            item.Graphic == (new Graphic(0x0EE3)) ||
            item.Graphic == (new Graphic(0x0EE4)) ||
            item.Graphic == (new Graphic(0x0EE5)) ||
            item.Graphic == (new Graphic(0x0EE6))
            )
          {
            item.Use();
            UO.Wait(500);
          }

        }
      }
    }


    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void DestroyOneSpiderWeb()
    {
      foreach (UOItem item in World.Ground)
      {
        if (item.Distance <= 1)
        {
          if (
            item.Graphic == (new Graphic(0x0EE1)) ||
            item.Graphic == (new Graphic(0x0EE2)) ||
            item.Graphic == (new Graphic(0x0EE3)) ||
            item.Graphic == (new Graphic(0x0EE4)) ||
            item.Graphic == (new Graphic(0x0EE5)) ||
            item.Graphic == (new Graphic(0x0EE6))
            )
          {
            item.Use();
            UO.Wait(150);
            break;
          }

        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void HodSvetloKad()
    {
      if (World.Player.Backpack.AllItems.FindType(0x1843, 0x03C4).Exist)
      {
        UO.WaitTargetSelf();
        UO.UseType(0x1843, 0x03C4);

        World.Player.PrintMessage("[Nighsight...]", Game.Val_GreenBlue);
      }
      else if (World.Player.Layers[Layer.LeftHand].Graphic == 0x0A15/* && World.Player.Layers[Layer.LeftHand].Color == 0x0493*/)
      {
        if (!World.Player.Hidden)
        {
          World.Player.Layers[Layer.LeftHand].Use();
          World.Player.PrintMessage("[Lucerna...]", Game.Val_GreenBlue);
        }
      }
      else
      {
        if (!World.Player.Hidden)
        {


          Game.CurrentGame.CurrentPlayer.GetSkillInstance<Magery>().CastSpell(StandardSpell.NightSight, World.Player.Serial);
        }
        else
          World.Player.PrintMessage("Ale ale copak to delas!! ");
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void ChangeProfile(string profile)
    {
      Config.Profile.ChangeProfile(profile);
    }

    //---------------------------------------------------------------------------------------------

    private static uint lastWrongSerial = 0;

    [Executable]
    public static void WrongKlic()
    {
      List<UOCharacter> npcs = World.Characters.Where(i => i.Distance < 25 && i.Notoriety == Notoriety.Innocent && i.Serial != World.Player.Serial).OrderBy(i => (uint)i.Serial).ToList();

      List<UOCharacter> npcsNext = npcs.Where(i => (uint)i.Serial > lastWrongSerial).ToList();

      UOCharacter ch = new UOCharacter(Serial.Invalid);

      if (npcsNext.Count > 0)
        ch = npcsNext[0];
      else if (npcs.Count > 0)
        ch = npcs[0];

      if (ch.Serial.IsValidCust() && ch.Exist)
      {
        Journal.Clear();

        if (String.IsNullOrEmpty(ch.Name))
        {
          ch.Click();
          Game.Wait(150);
        }

        if (!String.IsNullOrEmpty(ch.Name))
        {
          UO.Say(ch.Name + " klic ");
          Game.Wait(250);

          if (ch.Exist)
          {
            if (Journal.Contains(true, "modry"))
              ch.PrintMessage("[Modry..]", Game.Val_Blue);
            else if (Journal.Contains(true, "cerveny"))
              ch.PrintMessage("[Cerveny..]", Game.Val_Red);
            else if (Journal.Contains(true, "zeleny"))
              ch.PrintMessage("[Zleny..]", Game.Val_Green);
            else if (Journal.Contains(true, "zluty"))
              ch.PrintMessage("[Zluty..]", Game.Val_LightYellow);
            else
              ch.PrintMessage("[Zadny..]", Game.Val_LightPurple);

          }
        }

        lastWrongSerial = ch.Serial;
      }
    }

    //---------------------------------------------------------------------------------------------

    //[Executable]
    //public static void OverPack()
    //{
    //  UO.PrintInformation("Posuneme itemy nad backpack");
    //  UO.Wait(500);

    //  UO.PrintObject(Aliases.Self, 70, "Zamer batoh na srovnani veci nad nej");
    //  UOItem bag = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 1");
    //  UOItem item1 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 2");
    //  UOItem item2 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 3");
    //  UOItem item3 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 4");
    //  UOItem item4 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 5");
    //  UOItem item5 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 6");
    //  UOItem item6 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 7");
    //  UOItem item7 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);
    //  UO.Print("Zamer item 8");
    //  UOItem item8 = new UOItem(UIManager.TargetObject());
    //  UO.Wait(100);

    //  UO.PrintObject(Aliases.Self, 70, "Chvilku strpeni, srovnavam");
    //  UO.Wait(1000);
    //  UO.MoveItem(item1, 0, bag, 15, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item2, 0, bag, 35, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item3, 0, bag, 55, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item4, 0, bag, 75, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item5, 0, bag, 95, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item6, 0, bag, 115, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item7, 0, bag, 135, 10);
    //  UO.Wait(1000);
    //  UO.MoveItem(item8, 0, bag, 155, 10);
    //  UO.Wait(1000);

    //  UO.PrintObject(Aliases.Self, 70, "Hotovo");
    //}

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void Imortal()
    {
      Game.PrintMessage("Importal Runn");

      int runTime = 0;

      while (true)
      {
        Game.Wait(5);
        runTime += 5;

        if (World.Player.Hits <= 0)
        {
          Alchemy.ExecDrinkPotion("Heal");
        }

        //if (runTime % 2500 == 0)
        //{
        //  Game.PrintMessage("Imortal Run...");
        //}

      }

     // Game.PrintMessage("Importal End");

    }



    //---------------------------------------------------------------------------------------------
  }
}