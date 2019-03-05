//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Text;
//using Phoenix.WorldData;
//using Phoenix.Runtime;
//using Phoenix;
//using Caleb.Library;
//using System.Threading;
//using CalExtension.Skills;
//using Caleb.Library.CAL.Business;
//using CalExtension.PlayerRoles;

//namespace CalExtension.UOExtensions
//{
//  //---------------------------------------------------------------------------------------------

//  //public class HealGuard
//  //{
//  //  private static Thread worker;

//  //  private static bool healEngage;
//  //  public static bool HealEngage
//  //  {
//  //    get { return healEngage; }
//  //    set
//  //    {
//  //      healEngage = value;
//  //      //if (!healEngage)
//  //      //  healEngageKlamak = false;
//  //    }
//  //  }

//  //  //private static bool healEngageKlamak;
//  //  //public static bool HealEngageKlamak
//  //  //{
//  //  //  get { return healEngageKlamak; }
//  //  //  set { healEngageKlamak = value; }
//  //  //}

//  //  //---------------------------------------------------------------------------------------------

//  //  public static void Reset()
//  //  {
//  //    if (worker != null)
//  //    {
//  //      worker.Abort();
//  //      worker = null;
//  //      UO.Print(CalStatusMessage.Val_InfoColor, "HealGuard Abort");
//  //    }

//  //    UO.Print(CalStatusMessage.Val_InfoColor, "HealGuard Start");
    
//  //    worker = new Thread(new ThreadStart(LifeGuard));
//  //    worker.Start();
//  //  }

//  //  //---------------------------------------------------------------------------------------------

//  //  protected static void LifeGuard()
//  //  {
//  //    int counter = 0;

//  //    UO.Print(CalStatusMessage.Val_InfoColor, "LifeGuard Start " + UO.Dead);

//  //    bool poisonTry = false;
//  //    List<Serial> dennyMobs = new List<Serial>();

//      while (true)
//      {
//        counter = counter + 25;

//        if (!Game.CheckRunning())
//        {
//          if (World.Player.Hits <= World.Player.MaxHits || World.Player.Poisoned)// || (SkillsHelper.GetSkillValue("Healing").RealValue >= 850 && World.Player.Poisoned))
//          {
//            if (World.Player.Hits < World.Player.MaxHits)
//            {
//              poisonTry = false;
//              Game.CurrentGame.CurrentPlayer.GetSkillInstance<Healing>().Bandage(1, "self");
//            }
//            else if (World.Player.Hits >= World.Player.MaxHits)
//            {
//              if (World.Player.Poisoned && !poisonTry && (SkillsHelper.GetSkillValue("Healing").RealValue >= 850))
//              {
//                poisonTry = true;
//                Game.CurrentGame.CurrentPlayer.GetSkillInstance<Healing>().Bandage(1, "self");
//              }

//              break;
//            }
//          }
//          else if (/*HealEngageKlamak && */World.Player.Hits >= World.Player.MaxHits)
//          {
//            UOCharacter maxPriorityKlamak = null;

//  //          foreach (UOCharacter klamak in World.Characters)
//  //          {
//  //            if (klamak.Distance <= 3 && klamak.Hits > 0 && (klamak.Renamable || MobMaster2.IsMob(klamak.Serial)) && (klamak.Hits < klamak.MaxHits || klamak.Poisoned) && !dennyMobs.Contains(klamak.Serial))
//  //            {
//  //              if (maxPriorityKlamak == null)
//  //                maxPriorityKlamak = klamak;

//  //              if (maxPriorityKlamak.Hits > klamak.Hits)
//  //                maxPriorityKlamak = klamak;
//  //            }
//  //          }

//  //          if (maxPriorityKlamak != null)
//  //          {
//  //            maxPriorityKlamak.Print("{0}/{1}", maxPriorityKlamak.Hits, maxPriorityKlamak.MaxHits);

//  //            new Healing().Bandage(1, maxPriorityKlamak);

//  //            if (Journal.Contains(true, "Nemuzes pouzit bandy na summona"))
//  //            {
//  //              dennyMobs.Add(maxPriorityKlamak.Serial);
//  //            }
//  //          }
//  //        }
//  //      }


//        if (counter % 30000 == 0)
//        {
//          UO.Print("Run ");
//        }

//        UO.Wait(25);
//      }
//    }

//  //  //---------------------------------------------------------------------------------------------

//  //  public static bool KlamakAround
//  //  {
//  //    get
//  //    {
//  //      return World.Characters.FirstOrDefault(klamak => klamak.Distance <= 3 && klamak.Hits > 0 && klamak.Renamable) != null;
//  //      //foreach (UOCharacter klamak in World.Characters)
//  //      //{
//  //      //  if (klamak.Distance <= 3 && klamak.Hits > 0 && klamak.Renamable
//  //    }
//  //  }


//  //  //---------------------------------------------------------------------------------------------

//  //  [Executable]
//  //  [BlockMultipleExecutions]
//  //  public static void SwitchHeal()
//  //  {
//  //    HealEngage = !HealEngage;
//  //    //if (!HealEngage)
//  //    //  HealEngageKlamak = false;
//  //    Game.CurrentGame.CurrentPlayer.Messages.Add("Heal " + (HealEngage ? "On" : "Off"));
//  //  }

//  //  //---------------------------------------------------------------------------------------------

//  //  //[Executable]
//  //  //[BlockMultipleExecutions]
//  //  //public static void SwitchHealKlamak()
//  //  //{
//  //  //  HealEngageKlamak = !HealEngageKlamak;
//  //  //  if (!HealEngage)
//  //  //    HealEngage = true;
//  //  //  Game.CurrentGame.CurrentPlayer.Messages.Add("HealKlamak " + (HealEngageKlamak ? "On" : "Off"));
//  //  //}
//  //}
//}
