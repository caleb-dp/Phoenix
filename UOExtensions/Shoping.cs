using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Communication;

namespace CalExtension.UOExtensions
{
  public class Shoping
  {
    [Executable]
    public static void PrintRealDistance()
    {
      World.Player.Print("[Vyber ..]");

      var o = Targeting.GetTarget(null);

      Notepad.WriteLine(String.Format("Serial: {0:X} RealDistance: {1:N10}", o.Object.Serial, o.Object.GetDistance()));

    }


    [Executable]
    public static void BuyManual()
    {
      SayToOneSteapChar("buy");
    }

    [Executable]
    public static void SellManual()
    {
      SayToOneSteapChar("sell");
    }

    public static void SayToOneSteapChar(string sentence)
    {
      List<UOCharacter> vendors = World.Characters.Where(ch => ch.Distance <= 1 && ch.Serial != World.Player.Serial).OrderBy(ch => ch.GetDistance()).ToList();

      UOObject vendor = null;

      if (vendors.Count > 0)
        vendor = vendors[0];


      if (vendor == null)
      {
        World.Player.Print("[Vyber char..]");
        vendor = new UOObject(UIManager.TargetObject());
      }

      if (vendor != null && vendor.Exist)
      {
        if (String.IsNullOrEmpty(vendor.Name))
        {
          vendor.Click();
          Game.Wait(150);
        }

        UO.Say(vendor.Name + " " + sentence);
      }
    }
  }
}
