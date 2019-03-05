using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;

namespace CalExtension.Skills
{
  public class SkillsHelper
  {
    //---------------------------------------------------------------------------------------------

    public static SkillValue GetSkillValue(string skillName)
    {
      return World.Player.Skills[skillName];
    }

    //---------------------------------------------------------------------------------------------
  }
}
