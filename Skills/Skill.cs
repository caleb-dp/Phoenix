using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using CalExtension.UOExtensions;

namespace CalExtension.Skills
{
  public class Skill
  {
    [Executable]
    public static void TrainSkill(string skillName, int timeout)
    {
      TrainSkill(skillName, timeout, false);
    }

    [Executable]
    public static void TrainSkill(string skillName, int timeout, bool useObject)
    {
      try
      {
        //StandardSkill.DetectingHidden
        StandardSkill skill = (StandardSkill)Enum.Parse(typeof(StandardSkill), skillName, true);
        UOItem item = new UOItem(Serial.Invalid);
        if (useObject)
        {
          Game.PrintMessage("Vyber predmet:");
          item = Targeting.GetTarget(null);
        }

        Game.CurrentGame.Mode = GameMode.Working;

        while (!UO.Dead)
        {
          if (useObject)
          {
            if (item.ExistCust())
            {
              UO.WaitTargetObject(item);
            }
            else
            {
              Game.PrintMessage("Predmet neexistuje!");
              break;
            }
          }

          UO.UseSkill(skill);
          Game.Wait(timeout);
        }
      }
      catch
      {
        Game.PrintMessage("Ivalid Skill name");
      }
    }
  }
}
