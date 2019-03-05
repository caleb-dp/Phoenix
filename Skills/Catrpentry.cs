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
  public class Carpentry : Skill
  {

    public static UOItemType Log { get { return new UOItemType() { Graphic = 0x1BDD, Color = 0x0000 }; } }
    public static UOItemType Saw { get { return new UOItemType() { Graphic = 0x1035, Color = 0x0000 }; } }
    public static UOItemType Board { get { return new UOItemType() { Graphic = 0x1BD7, Color = 0x0000 }; } }
    public static UOItemType BarrelLid { get { return new UOItemType() { Graphic = 0x1DB8, Color = 0x0000 }; } }
    public static UOItemType CarpentryTrainer { get { return new UOItemType() { Graphic = 0x1E7A, Color = 0x03B9 }; } }
    //Color: 0x03B9  Graphic:  
      
    //---------------------------------------------------------------------------------------------

    public void CarpentryTrain()
    {
      Game.PrintMessage("Zvol kontejner s materialem > (esc) = ground");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      Journal.Clear();

      while (!UO.Dead)
      {
        if (!World.Player.Backpack.Items.FindType(Log.Graphic, 0x0000).Exist)
        {
          Game.PrintMessage("Doplnuji Log");
          UOItem materials = new UOItem(Serial.Invalid);
          if (containerFrom.Exist)
            materials = containerFrom.Items.FindType(Log.Graphic, 0x0000);

          if (materials.Exist)
            materials.Move(100, World.Player.Backpack);
          else
          {
            Game.PrintMessage("Neni Log");
            break;
          }
          Game.Wait();

          UOItem items = World.Player.Backpack.Items.FindType(Board.Graphic);
          if (items.Exist)
            items.Move(1000, containerFrom);
          Game.Wait();
        }

        UO.UseType(Saw.Graphic);
        UO.WaitMenu("Carpentry", "Miscellaneous", "Miscellaneous", "Boards");

        Journal.WaitForText(true, 5000, "You have failed to make anything", "You can't make anything", "You put");//, "You fail to");

        if (Journal.Contains("You can't make anything"))
        {
          Game.PrintMessage("Nemas suroviny");
          Game.Wait(2500);
        }

        Journal.Clear();
      }
      Game.PrintMessage("Konec treninku carpetnry");
    }

    //---------------------------------------------------------------------------------------------

    public void CarpentryTrainBL()
    {
      Game.PrintMessage("Zvol kontejner s materialem > (esc) = ground");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());

      Journal.Clear();

      while (!UO.Dead)
      {
        if (!World.Player.Backpack.Items.FindType(Board.Graphic).Exist || World.Player.Backpack.Items.FindType(Board.Graphic).Amount < 2)
        {
          Game.PrintMessage("Doplnuji Board");
          UOItem materials = new UOItem(Serial.Invalid);
          if (containerFrom.Exist)
            materials = containerFrom.Items.FindType(Board.Graphic);

          if (materials.Exist)
            materials.Move(100, World.Player.Backpack);
          else
          {
            Game.PrintMessage("Neni Board");
            break;
          }
          Game.Wait();

          UOItem items = World.Player.Backpack.Items.FindType(BarrelLid.Graphic);
          if (items.Exist)
            items.Move(1000, containerFrom);
          Game.Wait();
        }

        UO.UseType(Saw.Graphic);
        UO.WaitMenu("Carpentry", "Containers & Cont. parts", "Containers & Cont. parts", "Barrel Lid");

        Journal.WaitForText(true, 5000, "You have failed to make anything", "You can't make anything", "You put");//, "You fail to");

        if (Journal.Contains("You can't make anything"))
        {
          Game.PrintMessage("Nemas suroviny");
        }

        //if (World.Player.Backpack.Items.Count(BarrelLid.Graphic, BarrelLid.Color) >= 10)
        //{
        //  UOItem trainer = World.Ground.FindType(CarpentryTrainer.Graphic, CarpentryTrainer.Color);
        //  if (trainer.Exist)
        //  {
        //    Game.Wait(100);
        //    trainer.Use();
        //    Game.Wait(100);
        //  }
        //  else
        //  {
        //    Game.PrintMessage("Neni Trainer");
        //    break;
        //  }
        //}

        Journal.Clear();
      }
      Game.PrintMessage("Konec treninku carpetnry");
    }

    public void MakeCarp(int quantity, params string[] menus)
    {
      int itemMake = 0;

      Journal.Clear();

      while (!UO.Dead && itemMake < quantity)
      {

        UO.UseType(0x1034, 0x0000);
        UO.WaitMenu(menus);

        Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put");
        if (Journal.Contains("You put"))
          itemMake++;

        if (Journal.Contains("You can't make anything"))
        {
          Game.PrintMessage("Nemas suroviny");
          break;
        }


        Journal.Clear();
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void MoveitemTypeToMatBox(ushort amount, int totalItems)
    {
      UO.Print("Vyber typ >");
      UOItem type = new UOItem(UIManager.TargetObject());
      UOItem containerFrom = new UOItem(type.Container);

      UO.Print("Material Box do >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());


      Graphic typeG = type.Graphic;
      UOColor typeC = type.Color;

      int sychr = 0;
      int count = 0;

      while (containerFrom.Items.FindType(typeG, typeC).Exist)
      {
        sychr++;
        UOItem item = containerFrom.Items.FindType(typeG, typeC);
        UO.DeleteJournal();
        item.Move(amount, World.Player.Backpack);
        item.WaitTarget();
        containerTo.Use();
        Game.Wait(600);
        if (Journal.Contains(false, "V boxu muze byt maximalne 15 000 kusu!"))
        {
          UO.Print("Naplnena kapacita boxu!");
          break;
        }
        count += amount;

        if (totalItems > 0 && count >= totalItems)
          break;

        UO.Print("Total: " + count);
      }

      UO.Print("Konec prenosu.");
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    //---------------------------------------------------------------------------------------------

    [Executable("CarpentryTrain")]
    [BlockMultipleExecutions]
    public static void ExecCarpentryTrain()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Carpentry>().CarpentryTrain();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("CarpentryTrainBL")]
    [BlockMultipleExecutions]
    public static void ExecCarpentryTrainBL()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Carpentry>().CarpentryTrainBL();
    }


    [Executable("MakeCarp")]
    [BlockMultipleExecutions]
    public static void ExecMake(int quantity, params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Carpentry>().MakeCarp(quantity, menus);
    }

    #endregion
  }
}
