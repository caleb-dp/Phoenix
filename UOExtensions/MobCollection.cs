using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;


namespace CalExtension.UOExtensions
{
  public class MobCollection : List<Mob>
  {
    //---------------------------------------------------------------------------------------------

    public bool Contains(UOCharacter mobChar)
    {
      foreach (Mob mob in this)
        if (mob.MobChar.Serial == mobChar.Serial) return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public bool Contains(string name)
    {
      foreach (Mob mob in this)
        if (mob.GivenName == name) return true;

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public Mob this[string serial]
    {
      get 
      {
        foreach (Mob mob in this)
        {
          if (mob.MobChar.Serial.ToString() == serial)
            return mob;
        }

        return null;
      }
    }
  }
}
