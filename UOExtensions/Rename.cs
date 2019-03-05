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
using Phoenix.Communication;
using CalExtension.UI.Status;
using System.Collections;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------

  [RuntimeObject]
  public class Rename
  {
    //---------------------------------------------------------------------------------------------

    public Rename()
    {
      if (renameHtLock == null)
        renameHtLock = new object();

      World.CharacterAppeared += World_CharacterAppeared;
    }

    //---------------------------------------------------------------------------------------------

    private void World_CharacterAppeared(object sender, CharacterAppearedEventArgs e)
    {
      if (e.Serial == World.Player.Serial)
        return;

      if (!(CalebConfig.Rename == RenameType.OnAppeared || CalebConfig.Rename == RenameType.Booth))
        return;

      UOCharacter ch = new UOCharacter(e.Serial);

      if (!Game.IsPossibleMob(ch))
        return;

      if (Rename.IsMobRenamed(ch.Serial))
        return;


      if (!Rename.RenameCharacter(ch.Serial))
      {
        if (ch.Name == null || !Rename.IsMobRenamed(ch.Serial))
          ch.Changed += mob_Changed;
      }

      if (Rename.IsMobRenamed(ch.Serial))
      {
        new StatusBar().Show(ch.Serial);
        return;
      }
    }

    //---------------------------------------------------------------------------------------------

    private void mob_Changed(object sender, ObjectChangedEventArgs e)
    {
      UOCharacter mob = new UOCharacter(e.Serial);

      if (Game.Debug)
        Game.PrintMessage("!mob_Changed " + (mob.Name != null));

      if (mob.Name != null)
      {
        mob.Changed -= mob_Changed;
        Init(mob);
      }
    }

    //---------------------------------------------------------------------------------------------

    private void Init(UOCharacter mob)
    {
      Rename.RenameCharacter(mob.Serial);
    }

    //---------------------------------------------------------------------------------------------

    private static string playerShortCode = null;
    public static string PlayerShortCode
    {
      get
      {
        if (playerShortCode == null)
        {
          if (String.IsNullOrEmpty(World.Player.Name))
          {
            World.Player.Click();
            int timer = 0;
            while (timer < 150)
            {
              Thread.Sleep(5);
              timer += 5;

              if (!String.IsNullOrEmpty(World.Player.Name))
                break;
            }
          }

          string name = (String.Empty + World.Player.Name).ToLower().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("_", "").Replace(".", "").Replace(",", "");
          string code = name;

          if (name.Length > 2)
          {
            int mid = name.Length / 2;
            code = name[0].ToString() + name[mid].ToString() + name[name.Length - 1].ToString();
          }

          playerShortCode = code;
        }
        return playerShortCode;
      }
    }

    //---------------------------------------------------------------------------------------------

    public static string RenameSufix(string name)
    {
      if (name != null && name.Length > 1)
        return name[name.Length - 2].ToString().ToUpper() + name[name.Length - 1].ToString().ToUpper();

      return null;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsMobRenamed(Serial e)
    {
      UOCharacter ch = new UOCharacter(e);
      string name = ch.Name;
      return IsRenamedByPlayer(name);
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsRenamedByPlayer(string name)
    {
      if (name != null)
        return name.ToLower().StartsWith(PlayerShortCode.ToLower());

      return false;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsRenamedByValidName(string name)
    {
      return IsRenamedByPlayer(name);
    }

    //---------------------------------------------------------------------------------------------

    private static Hashtable _renamedHt;
    private static Hashtable renamedHt
    {
      get
      {
        if (_renamedHt == null)
        {
          _renamedHt = new Hashtable();
        }
        return _renamedHt;
      }
    }

    //---------------------------------------------------------------------------------------------
    private static object renameHtLock;

    private static MobRenameInfo EnsureRegisterRenameInfo(Serial serial)
    {
      lock (renameHtLock)
      {
        if (renamedHt[serial] == null)
        {
          renamedHt[serial] = new MobRenameInfo();
          ((MobRenameInfo)renamedHt[serial]).Serial = serial;
          ((MobRenameInfo)renamedHt[serial]).Character = new UOCharacter(serial);
          ((MobRenameInfo)renamedHt[serial]).Character.Changed += RenameInfoCharacterChange;
        }
      }

      return (MobRenameInfo)renamedHt[serial];
    }

    //---------------------------------------------------------------------------------------------

    public static MobRenameInfo GetRenameInfo(Serial serial)
    {
      lock (renameHtLock)
      {
        if (renamedHt[serial] != null)
          return (MobRenameInfo)renamedHt[serial];
      }
      return new MobRenameInfo();
    }

    //---------------------------------------------------------------------------------------------

    private static void RenameInfoCharacterChange(object sender, ObjectChangedEventArgs e)
    {
      if (Game.Debug)
        Game.PrintMessage("!RenameInfoCharacterChange");

      UOCharacter ch = new UOCharacter(e.Serial);
      if (ch.Distance > 30 || ch.Distance < 0)
      {
        lock (renameHtLock)
        {
          if (renamedHt[e.Serial] != null)
          {
            MobRenameInfo info = (MobRenameInfo)renamedHt[e.Serial];
            info.Character.Changed -= RenameInfoCharacterChange;
            renamedHt[e.Serial]  = info = null;
          }
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    public static MobRenameInfo RenameCharacter(Serial serial)
    {
      return RenameCharacter(serial, 2);
    }

    public static MobRenameInfo RenameCharacter(Serial serial, int maxTries)
    {
      UOCharacter ch = new UOCharacter(serial);
      MobRenameInfo renameInfo = EnsureRegisterRenameInfo(serial);//Game.renamedHt[serial] != null ? (MobRenameInfo)Game.renamedHt[serial] : new MobRenameInfo();

      if (renameInfo.Success || renameInfo.Tries >= maxTries)
        return renameInfo;

      renameInfo.Tries++;

      bool check = false;
      check = !String.IsNullOrEmpty(ch.Name);

      if (!check)
      {
        if (Game.Debug)
          Game.PrintMessage("Rename Check - Name EMPTY");
        check = ch.RequestStatus(250 + Core.CurrentLatency);
      }

      if (!check)
      {
        if (Game.Debug)
          Game.PrintMessage("Rename Check - !RequestStatus");

        ch.Click();
        Game.Wait(150);
        check = !String.IsNullOrEmpty(ch.Name) && ch.RequestStatus(250);
      }

      renameInfo.OriginalName = ch.Name;

      if (check)
      {
        string playerCode = Rename.PlayerShortCode;

        if ((ch.Name + String.Empty).StartsWith(playerCode))
        {
          renameInfo.NewName = ch.Name;
          renameInfo.Success = true;
        }
        else if (ch.Renamable || Game.IsPossibleMob(ch) && ch.Distance <= 4)//Uvidime, ty klamaky nejak casti blbnou oproti summum
        {
          var chars = "abcdefghijklmnopqrstuvwxyz1234567890";
          var random = new Random();
          var result = new string(
              Enumerable.Repeat(chars, 5)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray());

          result = playerCode + result;
          if (result.Length > 1)
          {
            result = result.Substring(0, result.Length - 2) + result[result.Length - 2].ToString().ToUpper() + result[result.Length - 1].ToString().ToUpper();
          }
          renameInfo.NewName = result;
          renameInfo.Success = ch.Rename(result);


          if (Game.Debug)
            Game.PrintMessage("Rename : " + renameInfo.Success + " [" + result + "]/[" + renameInfo.OriginalName + "]");

          Game.Wait(Core.CurrentLatency);

          if (renameInfo.Success)
          {
            byte[] data = PacketBuilder.CharacterSpeechUnicode(ch.Serial, ch.Model, result, SpeechType.Regular,
                                                                SpeechFont.Normal, Game.Val_PureWhite,
                                                                "[ " + result + " ]");
            Core.SendToClient(data, true);
          }
        }
      }
      else if (Game.Debug)
        Game.PrintMessage("!Rename Check");

      if (renameInfo.Success)
      {
        new StatusBar().Show(ch.Serial);
      }

      return renameInfo;
    }

    //---------------------------------------------------------------------------------------------
  }

  //---------------------------------------------------------------------------------------------

  public class MobRenameInfo
  {
    public MobRenameInfo()
    {
      RenameTime = DateTime.Now;
    }
    public bool Success = false;
    public string OriginalName;
    public string NewName;
    public Serial Serial;
    public UOCharacter Character;
    public DateTime RenameTime;
    public int Tries = 0;
    public bool Timeout
    {
      get
      {
        return (DateTime.Now - RenameTime).TotalSeconds > 4;
      }
    }

    public static implicit operator bool(MobRenameInfo info)
    {
      return info.Success;
    }

  }
}
