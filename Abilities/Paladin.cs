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
using Caleb.Library.CAL.Business;

namespace CalExtension.Abilities
{
  public static class Paladin
  {
    [Executable]
    public static void SupinaRess(string targets)
    {
      UOItem supina = World.Player.Backpack.AllItems.FindType(ItemLibrary.GoldenscaleSupina);


      if (!supina.Exist)
      {
         UOItem stit = World.Player.Backpack.AllItems.FindType(ItemLibrary.GoldenscaleSupina);
        if (!stit.Exist)
          stit = World.Player.FindType(ItemLibrary.GoldenscaleSupina);

        if (stit.Exist)
        {
          stit.Use();
          Game.Wait(200);
        }
        else
        {
          World.Player.PrintMessage("[ nemas mystik ]", MessageType.Warning);
          return;
        }
      }

      if (supina.Exist)
      {
        TargetAliasResult alias = Targeting.ParseTargets(targets); //TargetInfo tinfo = Targeting.GetTarget(targets);
        if (alias.IsStatic || alias.IsValid)
          alias.WaitTarget();
        supina.Use();

      }
      else
        World.Player.PrintMessage("[ nemas supinu ]", MessageType.Warning);

    }

    //---------------------------------------------------------------------------------------------
  }
}
