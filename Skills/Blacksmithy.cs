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
  public class Blacksmithy : Skill
  {

    public static UOItemType IronIngot { get { return new UOItemType() { Graphic = 0x1BEF , Color = 0x0000 }; } }
    // Color: 0x0000  Graphic: 0x1BEF  
    public void Make(int quantity, params string[] menus)
    {
      int itemMake = 0;

      Journal.Clear();

      while (!UO.Dead && itemMake < quantity)
      {

        UO.UseType(IronIngot.Graphic, IronIngot.Color);
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
    //Serial: 0x403567CF  Position: 48.146.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x0F51  Amount: 1  Layer: None  Container: 0x4032F802 Dagger
    //---------------------------------------------------------------------------------------------

    public void BlacksmithyTrain()
    {
      Game.PrintMessage("Zvol kontejner s materialem > (esc) = ground");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      UOItem trainer = World.Ground.FindType(0x0FB1, 0x0161);


      while (!UO.Dead && trainer.Exist)
      {

        if (!World.Player.Backpack.Items.FindType((ushort)Mining2.IngotGraphic.type1, (ushort)Mining2.IgnotColor.Iron).Exist || World.Player.Backpack.Items.Count((ushort)Mining2.IngotGraphic.type1, (ushort)Mining2.IgnotColor.Iron) < 20)
        {

          Game.PrintMessage("Nejsou ingoty" + containerFrom.Exist);
          UOItem ironIgnots = new UOItem(Serial.Invalid);
          if (containerFrom.Exist)
            ironIgnots = containerFrom.Items.FindType((ushort)Mining2.IngotGraphic.type1, (ushort)Mining2.IgnotColor.Iron);
          else
            ironIgnots = World.Ground.FindType((ushort)Mining2.IngotGraphic.type1, (ushort)Mining2.IgnotColor.Iron);

          if (ironIgnots.Exist)
            ironIgnots.Move(100, World.Player.Backpack);
          else
          {
            
            Game.PrintMessage("Nejsou ingoty");
            break;
          }
          Game.Wait();
        }

        if (World.Player.Backpack.Items.Count(0x0F51, 0x0000) >= 10)
        {
          trainer.Use();
          Game.Wait();
        }
        Game.PrintMessage("Kudly: " + World.Player.Backpack.Items.Count(0x0F51, 0x0000));

        this.Make(10, "Blacksmithing", "Iron Weapons", "Iron Weapons", "Swords & Blades", "Iron Swords & Blades", "Dagger");
      }
      Game.PrintMessage("Konec treninku BS");
    }

    //---------------------------------------------------------------------------------------------
    [Executable]
    public void MakeBloodSphere(int amount)
    {
      Game.PrintMessage("Vyber container s Blood wire a Rose wire");
      UOItem containerWireFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Fairy Dustama");
      UOItem containerFDFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Iron ingy");
      UOItem containerIngyFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container, kam to chces dat");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerWireFrom.AllItems.FindType(0x1876, 0x04C2).Move(15, World.Player.Backpack); // presun Blood wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1876, 0x0665).Move(15, World.Player.Backpack); // presun Rose wire
        Game.Wait();

        containerFDFrom.AllItems.FindType(0x103D, 0x0B52).Move(1, World.Player.Backpack); // presun FD
        Game.Wait();

        containerIngyFrom.AllItems.FindType(0x1BEF, 0x0000).Move(5, World.Player.Backpack); // presun Iron ingu
        Game.Wait();

        UO.UseType(0x1BEF, 0x0000);
        UO.WaitMenu("Blacksmithing", "Tools", "Tools", "Blood Rock Sphere");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put"))
        {
          if (Journal.Contains("You put"))
          {
            count += 1;
            World.Player.Backpack.AllItems.FindType(0x0E2D, 0x0846).Move(1, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }
        }

        Game.PrintMessage("Vyrobeno sphere: " + count);

      }

      Game.PrintMessage("MakeBloodSphere - End");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MakeBlackSphere(int amount)
    {
      Game.PrintMessage("Vyber container s Black wire a Shadow wire");
      UOItem containerWireFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Fairy Dustama");
      UOItem containerFDFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Iron ingy");
      UOItem containerIngyFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container, kam to chces dat");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerWireFrom.AllItems.FindType(0x1876, 0x0455).Move(15, World.Player.Backpack); // presun Black wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1876, 0x0770).Move(15, World.Player.Backpack); // presun Shadow wire
        Game.Wait();

        containerFDFrom.AllItems.FindType(0x103D, 0x0B52).Move(1, World.Player.Backpack); // presun FD
        Game.Wait();

        containerIngyFrom.AllItems.FindType(0x1BEF, 0x0000).Move(5, World.Player.Backpack); // presun Iron ingu
        Game.Wait();

        UO.UseType(0x1BEF, 0x0000);
        UO.WaitMenu("Blacksmithing", "Tools", "Tools", "Black Rock Sphere");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put"))
        {
          if (Journal.Contains("You put"))
          {
            count += 1;
            World.Player.Backpack.AllItems.FindType(0x0E2D, 0x0B15).Move(1, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }
        }

        Game.PrintMessage("Vyrobeno sphere: " + count);

      }

      Game.PrintMessage("MakeBlackSphere - End");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MakeMythSphere(int amount)
    {
      Game.PrintMessage("Vyber container s Myth wire a Gold wire");
      UOItem containerWireFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Fairy Dustama");
      UOItem containerFDFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container se Soul Shardama");
      UOItem containerSSFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Iron ingy");
      UOItem containerIngyFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container, kam to chces dat");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerWireFrom.AllItems.FindType(0x1876, 0x052D).Move(15, World.Player.Backpack); // presun Myth wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1878, 0x0000).Move(15, World.Player.Backpack); // presun Golden wire
        Game.Wait();

        containerFDFrom.AllItems.FindType(0x103D, 0x0B52).Move(1, World.Player.Backpack); // presun FD
        Game.Wait();

        containerSSFrom.AllItems.FindType(0x0FC4, 0x0498).Move(1, World.Player.Backpack); // presun SS
        Game.Wait();

        containerIngyFrom.AllItems.FindType(0x1BEF, 0x0000).Move(5, World.Player.Backpack); // presun Iron ingu
        Game.Wait();

        UO.UseType(0x1BEF, 0x0000);
        UO.WaitMenu("Blacksmithing", "Tools", "Tools", "Mytheril Sphere");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put"))
        {
          if (Journal.Contains("You put"))
          {
            count += 1;
            World.Player.Backpack.AllItems.FindType(0x0E2D, 0x0B8A).Move(1, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }
        }

        Game.PrintMessage("Vyrobeno sphere: " + count);

      }

      Game.PrintMessage("MakeMythSphere - End");
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public void MakeMagicSphere(int amount)
    {
      Game.PrintMessage("Vyber container s Magic wire, Silver wire, Golden wire, Shadow wire a Rose wire.");
      UOItem containerWireFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container s Iron ingy");
      UOItem containerIngyFrom = new UOItem(UIManager.TargetObject());

      Game.PrintMessage("Vyber container, kam to chces dat");
      UOItem containerTo = new UOItem(UIManager.TargetObject());


      int count = 0;
      while (count < amount)
      {
        UO.DeleteJournal();

        containerWireFrom.AllItems.FindType(0x1876, 0x0782).Move(150, World.Player.Backpack); // presun Magic wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1877, 0x0000).Move(50, World.Player.Backpack); // presun Silver wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1878, 0x0000).Move(25, World.Player.Backpack); // presun Golden wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1876, 0x0770).Move(25, World.Player.Backpack); // presun Shadow wire
        Game.Wait();

        containerWireFrom.AllItems.FindType(0x1876, 0x0665).Move(25, World.Player.Backpack); // presun Rose wire
        Game.Wait();

        containerIngyFrom.AllItems.FindType(0x1BEF, 0x0000).Move(5, World.Player.Backpack); // presun Iron ingu
        Game.Wait();

        UO.UseType(0x1BEF, 0x0000);
        UO.WaitMenu("Blacksmithing", "Tools", "Tools", "Magic Armor Sphere");

        if (Journal.WaitForText(true, 8000, "You have failed to make anything", "You can't make anything", "You put"))
        {
          if (Journal.Contains("You put"))
          {
            count += 1;
            World.Player.Backpack.AllItems.FindType(0x0E2D, 0x0782).Move(1, containerTo);
            Game.Wait(500);
          }

          if (Journal.Contains("You can't make anything"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }
        }

        Game.PrintMessage("Vyrobeno sphere: " + count);

      }

      Game.PrintMessage("MakeMagicSphere - End");
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("Make")]
    [BlockMultipleExecutions]
    public static void ExecMake(int quantity, params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Blacksmithy>().Make(quantity, menus);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("BlacksmithyTrain")]
    [BlockMultipleExecutions]
    public static void ExecBlacksmithyTrain()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Blacksmithy>().BlacksmithyTrain();
    }

    #endregion
  }
}
