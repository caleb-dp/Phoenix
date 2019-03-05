using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;

namespace CalExtension.UOExtensions
{
  public class Mob : IEquatable<Mob>
  {
    //---------------------------------------------------------------------------------------------

    public Mob(UOCharacter mob, string name)
    {
      this.mobChar = mob;
      this.givenName = name;
      if (mob != null && mob.Exist)
        this.mobChar.Rename(this.givenName);
    }

    //---------------------------------------------------------------------------------------------

    protected UOCharacter mobChar;
    public UOCharacter MobChar
    {
      get { return this.mobChar; }
      set { this.mobChar = value; }
    }

    //---------------------------------------------------------------------------------------------

    protected string givenName;
    public string GivenName
    {
      get { return this.givenName; }
      set { this.givenName = value; }
    }

    //---------------------------------------------------------------------------------------------

    public bool Exists
    {
      get { return this.MobChar != null && World.Exists(this.MobChar.Serial); }
    }

    //---------------------------------------------------------------------------------------------

    public bool Equals(Mob other)
    {
      if (other == null || other.MobChar == null || this.MobChar == null) return false;
      else if (other.MobChar.Serial == this.MobChar.Serial) return true;
      return false;
    }

    //---------------------------------------------------------------------------------------------

    public override bool Equals(object obj)
    {
      return this.Equals((Mob)obj);
    }

    //---------------------------------------------------------------------------------------------

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
