using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;


namespace CalExtension.UOExtensions
{
  public class PotionCollection : List<Potion>
  {
    public Potion GetItemByName(string name)
    {
      //oprava gramaticke chyby;
      name = (String.Empty + name).ToLower().Replace("strenght", "strength".ToLower());

      foreach (Potion p in this)
      {
        if (!String.IsNullOrEmpty(name) && p.Name.ToLower() == name.ToLower())
          return p;
      }
      return null;
    }

    public int GetIndexOf(Potion potion)
    {
      for (int i = 0; i < this.Count; i++)
      {
        if (potion.Name == this[i].Name) return i;
      }
      return -1;
    }

    public static PotionCollection Potions
    {
      get
      {
        return new PotionCollection() 
        {
          Potion.Agility, Potion.Cure, Potion.Explosion, Potion.ManaRefresh, Potion.Heal, Potion.Nightsight, Potion.Poison, Potion.Refresh,  Potion.Strength, 
          Potion.Shrink, Potion.Invisibility, Potion.ManaRefresh, Potion.TotalManaRefresh, Potion.FlaskOfHallucination, Potion.LavaBomb, Potion.Blood
        };
      }
    }

    //---------------------------------------------------------------------------------------------

    public static Potion GetPotionBy(Graphic g, UOColor c)
    {
      Potion p = null;

      foreach (Potion pot in Potions)
      {
        foreach (var pqd in pot.Qualities)
        {
          if (pqd.Value.Graphic == g && pqd.Value.Color == c)
            return pot; 
        }
      }


      return p;
    }

  }

  //---------------------------------------------------------------------------------------------
}
