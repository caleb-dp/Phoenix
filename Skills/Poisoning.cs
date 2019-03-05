using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;
using CalExtension.PlayerRoles;
using CalExtension.UOExtensions;

namespace CalExtension.Skills
{
  [RuntimeObject]
  public class Poisoning : Skill
  {
    //---------------------------------------------------------------------------------------------

    public static UOItemType TrainKit { get { return new UOItemType() { Graphic = 0x1837, Color = 0x0000, Name = "Apperentice's poisoning kit" }; } }
    public List<Serial> doneList;

    //---------------------------------------------------------------------------------------------

    public Poisoning()
    {
      this.doneList = new List<Serial>();
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsRunning = false;

    [Executable("PoisEngage")]
    public  void TrainPoisEngage()
    {
      if (IsRunning)
        return;
      IsRunning = true;

      UOItem trainKit = World.Player.Backpack.AllItems.FindType(TrainKit.Graphic, TrainKit.Color);

      if (trainKit.Exist)
      {
        IsRunning = true;
        List<UOCharacter> characters = new List<UOCharacter>();
        characters.AddRange(World.Characters.ToArray());
        var fiter = characters.Where(ch => (ch.Notoriety != Notoriety.Guild && (ch.Notoriety == Notoriety.Enemy || ch.Notoriety == Notoriety.Murderer || ch.Notoriety == Notoriety.Criminal || ch.Notoriety == Notoriety.Neutral))
                           && ch.Serial != World.Player.Serial
        && ch.Distance <= 1
                    && !Game.IsMob(ch.Serial)
                    && !Rename.IsMobRenamed(ch.Serial)
                    && !ch.Renamable
                    && !(ItemLibrary.IsMostCommonPlayerSummon(ch)));

        int done = 0;
        bool success = false;

        foreach (UOCharacter ch in fiter)
        {
          if (!this.doneList.Contains(ch.Serial))
          {
            Game.RunScriptCheck(1000);
            Game.CurrentGame.CurrentPlayer.SwitchWarmode();
            Game.Wait(250);
            UO.WaitTargetObject(ch.Serial);
            trainKit.Use();

            JournalEventWaiter jew = new JournalEventWaiter(true, "Uspesne jsi otravil svuj cil", "Kdyz se snazis pracovat s jedem, nemel bys delat nic jineho", "Na tomhle nemuzes trenovat", "Z teto nestvury se nic noveho nenaucis", "Na cili jiz nekdo trenoval");//todo 
            jew.Wait(500);

            if (Journal.Contains(true, "Uspesne jsi otravil svuj cil", "Na tomhle nemuzes trenovat", "Z teto nestvury se nic noveho nenaucis", "Na cili jiz nekdo trenoval"))
            {
              this.doneList.Add(ch.Serial);
              success = true;
            }

            Journal.Clear();
            break;
          }
          else
            done++;
        }

        if (!success && done > 0)
          World.Player.PrintMessage("Vse okolo poisnuto!");
        else if (fiter.Count() == 0)
          World.Player.PrintMessage("Nic k poisnuti!");
      }
      else
        World.Player.PrintMessage("Nemas pois KIT");

      IsRunning = false;
    }

    //---------------------------------------------------------------------------------------------
  }
}
