using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalExtension;
using CalExtension.UOExtensions;
using Caleb.Library;
using CalExtension.Skills;

namespace CalExtension.Abilities
{
  public static class MagicMiner
  {
    public static IUOItemType IronIngot { get { return new UOItemTypeBase(0x1BEF, 0x0000);  } }
    public static IUOItemType IronWall { get { return new UOItemTypeBase(0x2206, 0x0835); } }//Todo color
    //,exec makedust 2 0x04C2 --blood
    // ,exec makedust 2 0x052D --myth 
    //,exec makedust 2 0x0455 --black 
    //---------------------------------------------------------------------------------------------

    public static UOItem AdaHammer
    {
      get { return World.Player.FindType(0x1438, 0x044C); }
    }

    //---------------------------------------------------------------------------------------------
    //0x04C2 
    [Executable]
    public static void MakeDust(ushort amount, UOColor ingotColor)
    {
      if (!AdaHammer.Exist)
      {
        World.Player.PrintMessage("Nemas kladivo", MessageType.Error);
        return;
      }

      if (!World.Player.FindType(0x1BEF, ingotColor).Exist)
      {
        World.Player.PrintMessage("Nemas Ingoty", MessageType.Error);
        return;
      }

      if (!World.Player.FindType(0x1BEF, ingotColor).Move(amount, World.Player.X, World.Player.Y, World.Player.Z))
      {
        World.Player.PrintMessage("Nepovedl se drop", MessageType.Error);
        return;
      }

      Game.Wait(250);

      UOItem ingot = World.Ground.OrderBy(i => i.Distance).FirstOrDefault(i => i.Graphic == 0x1BEF && i.Color == ingotColor);//.FindType(0x1BEF, ingotColor);
      if (ingot != null && ingot.Distance > 1)
      {
        World.Player.PrintMessage("Ingot moc daleko " + ingot.Distance, MessageType.Error);
        ingot.PrintMessage("Tento!", MessageType.Error);
        return;
      }

      Journal.Clear();

      UO.WaitTargetObject(ingot);
      AdaHammer.Use();

      Journal.WaitForText(true, 500, "Rozbils ingot na kouzelne shardy!");

      


      Game.Wait(250);

      if (Journal.Contains(true, "at your feet. It is too heavy.."))
      {
        World.Player.PrintMessage("[ too heavy.. ]", MessageType.Warning);
      }

      UOItemExtInfo info = ItemHelper.GetItemExtInfo(AdaHammer);
      Game.PrintMessage("Nabiti kladiva " + info.PlusValue);
    }
  }
}
