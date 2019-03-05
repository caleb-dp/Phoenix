using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using System.Drawing;
using Phoenix.WorldData;
using CalExtension.UOExtensions;

namespace CalExtension.UI.Status
{
  public class StatusWrapper
  {
    //---------------------------------------------------------------------------------------------

    public static StatusFormWrapper GetCurrentWrapper(Serial serial)
    {
      return GetCurrentWrapper(GetWrapperType(serial));
    }

    //---------------------------------------------------------------------------------------------

    public static List<StatusFormWrapper> GetAllWrappers()
    {
      return WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusFormWrapper>().ToList();
    }

    //---------------------------------------------------------------------------------------------

      //TODO vyresit
    //public static StatusFormWrapper GetCurrentHoveringWrapper(Point location)
    //{
    //  foreach (StatusFormWrapper wrapper in GetAllWrappers())
    //  {
    //    if (wrapper.Location.X < location.X && location.X < (wrapper.Location.X + wrapper.Width) && wrapper.Location.Y < location.Y && location.Y < (wrapper.Location.Y + wrapper.Height))
    //      return wrapper;
    //  }

    //  return null;
    //}

    //---------------------------------------------------------------------------------------------

    public  static StatusFormWrapper GetCurrentWrapper(StatusFormWrapperType type)
    {
      StatusFormWrapper form = WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusFormWrapper>().Where(i => i.WrapperType == type).FirstOrDefault() as StatusFormWrapper;
      return form;
    }

    //---------------------------------------------------------------------------------------------

    public static void ShowWrapper(StatusFormWrapperType type)
    {
      if (GetCurrentWrapper(type) == null)
      {
        WindowManager.GetDefaultManager().CreateWindow(delegate ()
        {
          var f = new StatusFormWrapper(type);
          return f;
        });
      }
    }

    //---------------------------------------------------------------------------------------------

    public static StatusFormWrapperType GetWrapperType(Serial serial)
    {
      if (serial == World.Player.Serial)
        return StatusFormWrapperType.None;

      UOCharacter mobile = new UOCharacter(serial);

      if (Game.IsMob(mobile))
        return StatusFormWrapperType.Mob;
      else if (Game.CurrentGame.IsAlie(serial) || mobile.Notoriety == Notoriety.Innocent || mobile.Notoriety == Notoriety.Guild)
        return StatusFormWrapperType.Other;
      else return StatusFormWrapperType.Enemy;
    }

    //---------------------------------------------------------------------------------------------

    public static bool IsType(Serial serial, StatusFormWrapperType type)
    {
      UOCharacter mobile = new UOCharacter(serial);

      if (type == StatusFormWrapperType.Mob)
        return Game.IsMob(mobile);
      else if (type == StatusFormWrapperType.Other)
        return Game.CurrentGame.IsAlie(serial) || mobile.Notoriety == Notoriety.Innocent || mobile.Notoriety == Notoriety.Guild;
      else if (type == StatusFormWrapperType.Enemy)
        return !IsType(serial, StatusFormWrapperType.Mob) && !IsType(serial, StatusFormWrapperType.Other);

      return false;
    }

    //---------------------------------------------------------------------------------------------

    [Executable("showutilityform")]
    public static void ShowUtilityForm()
    {
      if (WindowManager.GetDefaultManager().OwnedWindows.OfType<UtilityForm>().Count() == 0)
      {
        WindowManager.GetDefaultManager().CreateWindow(delegate ()
        {
          var f = new UtilityForm();
          return f;
        });
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable("showslotform")]
    public static void ShowSlotForm()
    {
      if (WindowManager.GetDefaultManager().OwnedWindows.OfType<SlotForm>().Count() == 0)
      {
        WindowManager.GetDefaultManager().CreateWindow(delegate ()
        {
          var f = new SlotForm();
          return f;
        });
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable("showenemywrapper")]
    public static void ShowEnemyWrapper()
    {
      ShowWrapper(StatusFormWrapperType.Enemy);  
    }

    //---------------------------------------------------------------------------------------------

    [Executable("showotherwrapper")]
    public static void ShowOtherWrapper()
    {
      ShowWrapper(StatusFormWrapperType.Other);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("showmobwrapper")]
    public static void ShowMobWrapper()
    {
      ShowWrapper(StatusFormWrapperType.Mob);
    }


    //---------------------------------------------------------------------------------------------

    [Executable("showfreewrapper")]
    public static void ShowFreebWrapper()
    {
      ShowWrapper(StatusFormWrapperType.Free);
    }
    //---------------------------------------------------------------------------------------------
  }
}
