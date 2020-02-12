using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;

namespace Phoenix.Scripts.Raziel
{
  public class Recharge
  {
    public Recharge()
    {
    }

    [Command]
    public void recharge(int pocet)
    {
      UO.Print("Vyber krystal");
      UOItem krystal = new UOItem(UIManager.TargetObject());
      Graphic g = krystal.Graphic;
      UOColor c = krystal.Color;

      UO.Print("Vyber co nabijet");
      UOItem cil = new UOItem(UIManager.TargetObject());
      for (int i = 1; i <= pocet; i++)
      {
        UO.DeleteJournal();
        UO.UseType(g, c);
        cil.WaitTarget();
        UO.Wait(500);
        while (!UO.InJournal("Item has been recharged"))
          UO.Wait(500);
      }
      UO.Print("Hotovo.");

    }
  }
}