using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;
using Caleb.Library.CAL.Business;
using Caleb.Library;
using CalExtension.Skills;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  public class Mount
  {
    //---------------------------------------------------------------------------------------------

    public static Mount Current;

    public Mount()
    {
      if (Current == null)
        Current = this;


    }

    //---------------------------------------------------------------------------------------------
    public static Serial _Mount = Serial.Invalid;
    private static List<string> denyMounts = new List<string>();
    private static List<string> skipMounts = new List<string>();

    //---------------------------------------------------------------------------------------------

    [Executable]
    public bool UseMount(int dummy)
    {
      return UseMount();//histrocika kompatibilita s hotkou
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public bool UseMount()
    {
      var currChars = World.Characters.Where(ch => ch.Serial != World.Player.Serial && ch.Distance <= 6).ToArray();
      var newChars = World.Characters.Where(ch => ch.Serial != World.Player.Serial && ch.Distance <= 6).ToArray();
      var diff = newChars.Where(ch => currChars.Count(cch => cch.Serial == ch.Serial) == 0).ToArray();

      UOItem currentMount = World.Player.Layers[Layer.Mount];
      Serial originalMount = _Mount;

      bool dismount = currentMount.ExistCust();

      _Mount = Serial.Invalid;

      if (dismount)
      {
        UOColor currentColor = currentMount.Color;
        World.Player.Use();
        Game.Wait(100);

        newChars = World.Characters.Where(ch => ch.Serial != World.Player.Serial && ch.Distance <= 6).ToArray();
        diff = newChars.Where(ch => currChars.Count(cch => cch.Serial == ch.Serial) == 0).ToArray();
        diff = diff.OrderBy(ch => ch.Distance).ThenBy(ch => (ch.Color == currentColor ? 0 : 1)).ToArray();

        if (diff.Count() > 0)
          _Mount = diff.ToArray()[0].Serial;
      }
      else
      {
        UOCharacter curr = new UOCharacter(originalMount);

        if (!curr.ExistCust())
        {
          UOItem shrink = new UOItem(Serial.Invalid);
          foreach (UOItem itm in World.Player.Backpack.AllItems)
          {
            if (ItemLibrary.ShrinkMountTypes.Count(uot => uot.Graphic == itm.Graphic) > 0)
            {
              shrink = itm;
              break;
            }
          }

          var m = currChars.
            Where(ch => !denyMounts.Contains(ch.GetUniqueKey()) && !skipMounts.Contains(ch.GetUniqueKey())).
            OrderBy(ch => (ch.Renamable ? 0 : 1)).
            ThenBy(ch => ItemLibrary.MountTypes.Count(mt => mt.Graphic == ch.Model) > 0 ? 0 : 1).
            ThenBy(ch => ch.Distance).ToArray();

          if (m.Length > 0)
            _Mount = m[0].Serial;
          else
          {
            if (shrink.Exist)
            {
              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
              shrink.Use();
              Game.Wait(150);

              newChars = World.Characters.Where(ch => ch.Serial != World.Player.Serial && ch.Distance <= 6).ToArray();
              diff = newChars.Where(ch => currChars.Count(cch => cch.Serial == ch.Serial) == 0).ToArray();
              diff = diff.OrderBy(ch => ch.Distance).ToArray();

              if (diff.Count() > 0)
                _Mount = diff.ToArray()[0].Serial;
            }
            skipMounts.Clear();
          }
        }
        else
          _Mount = originalMount;

        curr = new UOCharacter(_Mount);

        if (curr.ExistCust())
        {
          curr.Use();

          if (Journal.WaitForText(true, 150, "You dont own that horse"))
          {
            curr.PrintMessage("[ deny add ]");
            denyMounts.Add(curr.GetUniqueKey());
            _Mount = Serial.Invalid;
          }
          else if (!World.Player.Layers[Layer.Mount].ExistCust())
          {
            if (curr.Serial == originalMount && new UOCharacter(originalMount).Exist)
            {
              curr.PrintMessage("[ mount ]");
            }
            else
            {
              curr.PrintMessage("[ skip add ]");
              skipMounts.Add(curr.GetUniqueKey());
              _Mount = Serial.Invalid;
            }
          }

        }
        else
          World.Player.PrintMessage("[ neni mount ]");
      }

      return _Mount.IsValidCust();
    }

    //---------------------------------------------------------------------------------------------
  }
}
