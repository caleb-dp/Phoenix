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
using CalExtension.UI.Status;
using Caleb.Library.CAL.Business;
using System.Linq;
using System.Collections;

namespace CalExtension.PlayerRoles
{
  [RuntimeObject]
  public class MobMaster2
  {
    public object SyncRoot;
    private bool debug = false;

    //---------------------------------------------------------------------------------------------
    public static MobMaster2 CurrentMaster;
    private static Hashtable renamedHt;
    public MobMaster2()
    {
      World.CharacterAppeared += World_CharacterAppeared;
      renamedHt = new Hashtable();
      if (CurrentMaster == null)
        CurrentMaster = this;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsMob(Serial e)
    {
      return renamedHt[e] != null;
    }

    //---------------------------------------------------------------------------------------------

    public void Rename(Serial serial, bool force)
    {
      if (debug)
        UO.Print("Rename " + serial);

      if (renamedHt[serial] != null && !force)
      {
        if (debug)
          UO.Print("renamedHt " + serial);
        return;
      }

      UOCharacter ch = new UOCharacter(serial);
      bool typeCheck = force || UOItemTypeBase.ListContains(ch.Model, ch.Color, ItemLibrary.PlayerSummonsFull) || UOItemTypeBase.ListContains(ch.Model, ch.Color, ItemLibrary.AttackKlamaci);

      if (debug)
        UO.Print("Rename " + serial + "  typeCheck " + typeCheck);

      if (typeCheck && ch.Distance < 15)
      {
        if (String.IsNullOrEmpty(ch.Name))
          ch.RequestStatus(150);

        if ((ch.Name + String.Empty).StartsWith("S5"))
        {
          renamedHt[serial] = ch;
          new StatusBar().Show(ch.Serial);
          return;
        }

        if (ch.Renamable)
        {
          var chars = "abcdefghijklmnopqrstuvwxyz1234567890";
          var random = new Random();
          var result = new string(
              Enumerable.Repeat(chars, 5)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray());

          result = "S5" + result;
          ch.Rename(result);
          ch.Click();
          UO.Wait(150);
          new StatusBar().Show(ch.Serial);
          //          ch.RequestStatus(150);
          // ch.Print(0x008f, "[" + ch.Name + "]");
          renamedHt[serial] = ch;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      this.Rename(e.Serial, false);
    }

    //---------------------------------------------------------------------------------------------
  }
}