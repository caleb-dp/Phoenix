//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using Phoenix;
//using Phoenix.WorldData;
//using Phoenix.Runtime;
//using System.Reflection;

//namespace Scripts.DarkParadise
//{
//  //[RuntimeObject]
//  public class CorpsesNotoriety
//  {
//    class JournalSerialEventWaiter : JournalEventWaiter
//    {
//      private Serial serial;

//      public JournalSerialEventWaiter(Serial serial)
//          : base(false, new string[0])
//      {
//        this.serial = serial;
//      }

//      protected override bool OnEventArgsTest(object eventSender, JournalEntryAddedEventArgs eventArgs)
//      {
//        return eventArgs.Entry.Serial == serial && eventArgs.Entry.Text.StartsWith(eventArgs.Entry.Name);
//      }
//    }

//    private static object syncRoot = new object();
//    private static Dictionary<Serial, Notoriety> corpsesNotoriety = new Dictionary<Serial, Notoriety>();

//    static CorpsesNotoriety()
//    {
//      Journal.EntryAdded += new JournalEntryAddedEventHandler(Journal_EntryAdded);
//      World.WorldCleaned += new EventHandler(World_WorldCleaned);
//      RuntimeCore.UnregisteringAssembly += new UnregisteringAssemblyEventHandler(RuntimeCore_UnregisteringAssembly);
//    }

//    static void RuntimeCore_UnregisteringAssembly(object sender, UnregisteringAssemblyEventArgs e)
//    {
//      if (e.Assembly == Assembly.GetExecutingAssembly())
//      {
//        corpsesNotoriety = null;
//      }
//    }

//    static void Journal_EntryAdded(object sender, JournalEntryAddedEventArgs e)
//    {
//      if (e.Entry.Type == SpeechType.Label && e.Entry.Text.StartsWith(e.Entry.Name))
//      {
//        UOItem item = World.GetItem(e.Entry.Serial);
//        if (item.Exist && !corpsesNotoriety.ContainsKey(item))
//        {
//          Notoriety notoriety;

//          switch (e.Entry.Color)
//          {
//            case 0x0063:
//              notoriety = Notoriety.Innocent;
//              break;

//            case 0x0026:
//              notoriety = Notoriety.Murderer;
//              break;

//            case 0x03E9:
//            case 0x03B2:
//              notoriety = Notoriety.Neutral;
//              break;

//            case 0x0044:
//              notoriety = Notoriety.Guild;
//              break;

//            case 0x002B:
//              notoriety = Notoriety.Enemy;
//              break;

//            default:
//              notoriety = Notoriety.Unknown;
//              break;
//          }

//          lock (syncRoot)
//          {
//            corpsesNotoriety.Add(item, notoriety);
//          }
//        }
//      }
//    }

//    static void World_WorldCleaned(object sender, EventArgs e)
//    {
//      CleanUp();
//    }

//    public static Notoriety Get(UOItem item, int timeout)
//    {
//      Notoriety notoriety;

//      lock (syncRoot)
//      {
//        if (corpsesNotoriety.TryGetValue(item, out notoriety))
//          return notoriety;
//      }

//      if (!item.Exist || item.Graphic != 0x2006)
//        throw new ScriptErrorException("Item doesn't exist or it's not a corpse.");

//      using (JournalSerialEventWaiter ew = new JournalSerialEventWaiter(item))
//      {
//        item.Click();
//        ew.Wait(timeout);
//      }

//      lock (syncRoot)
//      {
//        corpsesNotoriety.TryGetValue(item, out notoriety);
//      }

//      return notoriety;
//    }

//    private static void CleanUp()
//    {
//      lock (syncRoot)
//      {
//        List<Serial> removeList = new List<Serial>();

//        foreach (KeyValuePair<Serial, Notoriety> pair in corpsesNotoriety)
//        {
//          if (!World.GetItem(pair.Key).Exist)
//            removeList.Add(pair.Key);
//        }

//        for (int i = 0; i < removeList.Count; i++)
//        {
//          corpsesNotoriety.Remove(removeList[i]);
//        }
//      }
//    }
//  }
//}