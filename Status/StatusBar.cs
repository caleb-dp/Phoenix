using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using System.Threading;
using System.Windows.Forms;
using Phoenix.Gui.Controls;
using Phoenix.Gui;
using Phoenix.Runtime;
using System.Reflection;
using System.Security.Permissions;
using Phoenix.WorldData;
using System.Runtime.InteropServices;
using System.Security;

namespace CalExtension.UI.Status
{
  public class StatusBar
  {
    /// <summary>
    /// Zapina/vypina pruhlednost zalozek.
    /// </summary>
    public const bool Transparent = true;

    public static bool StatusExists(Serial id)
    {
      return WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == id).Count() > 0;
    }

    [Executable("calstatusbar")]
    public void Show()
    {
      UO.Print("Select");
      Show(UIManager.TargetObject());
    }

    public static event CharacterAppearedEventHandler OnStatusShow;


    [Executable("calstatusbar")]
    public void Show(Serial id)// StatusFormWrapper wrapper)
    {
      if (!id.IsValid)
        throw new ScriptErrorException("Invalid id.");

      if (StatusExists(id))
        return;

      //UOCharacter ch = new UOCharacter(id);
      //if (ch.Exist && (ch.Hits <= 0 || !String.IsNullOrEmpty(ch.Name)))
      //  ch.RequestStatus(150);

      // Vytvor okno
      StatusForm  form =  WindowManager.GetDefaultManager().CreateWindow(delegate ()
      {
        var f = new StatusForm(id);//, wrapper ?? StatusWrapper.GetCurrentWrapper(id));
        f.Transparency = Transparent;
        f.Manual = true;
        return f;
      }) as StatusForm;

      if (OnStatusShow != null)
        OnStatusShow(form, new CharacterAppearedEventArgs(id));// EventArgs());
    }

    [Executable("calstatusshowcloseall")]
    public void CloseShowAll()
    {
      //  CloseAll();
      ShowAll();
    }


    [Executable("calstatusall")]
    public void ShowAll()
    {
      foreach (var mob in World.Characters)
      {
        if (!StatusExists(mob.Serial))
        {
          UO.Exec("calstatusbar", mob.Serial);
          UO.Wait(100);
        }
      }
    }
  }
}
