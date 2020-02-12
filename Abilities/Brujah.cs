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
  public class Brujah
  {
    public static UOItemTypeBase GreeziArtefakt { get { return new UOItemTypeBase(0x1B17, 0x0493) { Name = "Greezi Artefakt" }; } }

    [Executable]
    public void Rewind()
    {
      ItemHelper.EnsureContainer(World.Player.Backpack);

      UOItem uOItem = World.Player.Backpack.Items.FindType(GreeziArtefakt.Graphic);


      
      if (uOItem.Exist)
      {
        Journal.Clear();

        uOItem.Use();

        if (Journal.WaitForText(true, 500, "You used Rewind"))
        {
          Game.PrintMessage("Rewin Start");

          AsyncCounter counter = new AsyncCounter();
          counter.PrefixText = "Rewind: ";
          counter.PrintType = "Game";
          counter.ForceCounter = true;
          counter.HighlightTime = 3100;
          counter.HighlightColor = 0x055A;
          counter.Step = 500;
          counter.MaxTries = 12;
          counter.StopMessage = "You was ported back in time";
          counter.StopMethod = Hiding.IsHidden;
          counter.RunComplete += Counter_RunComplete;
          counter.Run();
        }

      }
      else
      {
        World.Player.PrintMessage("Nemas artefakt", MessageType.Warning);
      }
    }

    private static void Counter_RunComplete(object sender, EventArgs e)
    {
      Game.PrintMessage("Rewin End");
    }


    //---------------------------------------------------------------------------------------------

    //    19:11 System: Targeting Cancelled
    //19:12 You see: a Greezi Artefakt
    //19:12 : [Use]
    //19:12 Portia Labiata: [Odhidnut! ]
    //19:12 System: You have been revealed
    //19:12 System: Vitej doma.
    //19:12 : [Use]
    //19:12 Portia Labiata: *You used Rewind*
    //19:12 System: Za 5 sekund budes vracen v case!
    //19:12 Portia Labiata: *You was ported back in time*
    //19:12 Portia Labiata: Artefakt jeste neni pripraven k pouziti!
  }
}
