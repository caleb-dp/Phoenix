using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using Caleb.Library.CAL.Business;
using CalExtension.Skills;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using CalExtension.UOExtensions;

namespace CalExtension.Quests
{
  public class Mikulas
  {
    //---------------------------------------------------------------------------------------------

    [Executable]
    public void mikulasuse()
    {
      World.FindDistance = 20;
      if (World.Player.Backpack.Items.FindType(0x1ECD, 0x0B1D).Exist)
      {
        UO.UseType(0x1ECD, 0x0B1D);//ukolnicek
        Game.Wait();

      }
      //OpenDoorAll();
      if (!World.Player.Layers[Layer.Mount].Exist)
      {
        string summon = "";
        if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Ranger || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Mage || Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Cleric)
        {

          summon = "Horse";

        }
        else if (Game.CurrentGame.CurrentPlayer.PlayerClass == PlayerClass.Necromancer)
        {
          summon = "Dark Oclock";
        }

        bool fizz = false;
        if (!String.IsNullOrEmpty(summon))
        {
          Journal.Clear();
          UO.SummonCreature(summon, World.Player);
          fizz = Journal.WaitForText(true, 3500, "fizz");

          if (fizz)
          {
            Journal.Clear();
            UO.SummonCreature(summon, World.Player);
            fizz = Journal.WaitForText(true, 3500, "fizz");
          }



        }

        Game.Wait();
        if (!fizz)
        {
          Mount.Current.UseMount();
          Game.Wait(250);
          if (World.Player.Layers[Layer.Mount].Exist)
            return;
        }
      }


      foreach (UOCharacter ch in World.Characters)
      {
        if (String.IsNullOrEmpty(ch.Name))
        {
          ch.Click();
          Game.Wait();
        }

        if (ch.Distance <= 3)
        {
          if (ch.Model == 0x003E && ch.Color == 0x0B98 && ch.Name.ToLower() == "certik")
          {
            UO.Say(ch.Name + " branu");
            return;
          }
        }
        else if (ch.Distance <= 7)
        {
          if (ch.Model == 0x0190 && ch.Color == 0x0421 && ch.Name.ToLower() == "matej")
          {
            Journal.Clear();
            UO.Say(ch.Name + " hi");


            string kop = String.Empty;

            if (Journal.WaitForText(true, 250, "umbre"))
            {
              kop = "umbra";
            }

            if (!String.IsNullOrEmpty(kop))
            {
              if (World.Player.Backpack.Items.FindType(0x22C5, 0x0000).Exist)
              {
                Kniha.Current.CestovniKnihaUse(1);
                Game.Wait();

                if (kop == "umbra")
                {
                  Kniha.Current.CestovniKnihaUse(25);
                }
                //else
                //{
                //  Kniha.Current.CestovniKnihaUse(5);
                //}
              }
              Game.Wait(1000);
            }

            Hiding.ExecHide();
            return;
          }
        }

        if (ch.Distance <= 1)
        {
          if (ch.Model == 0x002F && ch.Color == 0x047E && ch.Name.ToLower() == "velice stary ent")
          {
            Journal.Clear();
            UO.Say(ch.Name + " hi");

            string kop = String.Empty;

            if (Journal.WaitForText(true, 250, "luny"))
            {
              kop = "luna";
            }
            else if (Journal.WaitForText(true, 250, "scara"))
            {
              kop = "scara";
            }
            else if (Journal.WaitForText(true, 250, "ilshe"))
            {
              kop = "jhelom";
            }

            if (!String.IsNullOrEmpty(kop))
            {
              if (World.Player.Backpack.Items.FindType(0x22C5, 0x0000).Exist)
              {
                Kniha.Current.CestovniKnihaUse(1);
                Game.Wait();

                if (kop == "luna")
                {
                  Kniha.Current.CestovniKnihaUse(24);
                }
                else if (kop == "scara")
                {
                  Kniha.Current.CestovniKnihaUse(12);
                }
                else
                  Kniha.Current.CestovniKnihaUse(8);
              }
              else 
              {
                if (kop == "luna")
                {
                  Kniha.Current.TravelBookUse(1);
                }
                else if (kop == "scara")
                {
                  Kniha.Current.RuneBookUse(5);
                }
                else
                  Kniha.Current.RuneBookUse(1);
              }
              Game.Wait(1000);
            }

            Hiding.ExecHide();

            return;
          }
        }
        

        
      } 

      foreach (UOItem o in World.Ground)
      {


        if (o.Distance <= 1)
        {

          if (o.Graphic == 0x09B1 && o.Color == 0x0000)//kosick
          {
            UO.PrintObject(o, "Kosik");
            o.Use();
            return;
          }
          else if (o.Graphic == 0x09D7 && o.Color == 0x0422)//Hilina
          {
            UO.PrintObject(o, "Hlina");
            o.Use();
            return;
          }
          else if (o.Graphic == 0x1777 && o.Color == 0x084F)//kameny
          {
            UO.PrintObject(o, "Kamen");
            o.Use();
            return;
          }
          else if (o.Graphic == 0x0C9A && o.Color == 0x0000)//stromek
          {
            UO.PrintObject(o, "Stromek");
            o.Use();
            return;
          }
          else if (o.Distance == 0 && o.Graphic == 0x3490 && o.Color == 0x0000)//vir
          {
            UO.PrintObject(o, "Vir");
            o.Use();
            return;
          }
          else if (o.Graphic == 0x0913 && o.Color == 0x0000)//hlina ubra
          {
            UO.PrintObject(o, "Hlina");
            o.Use();
            return;
          }
          else if (o.Graphic == 0x1366 && o.Color == 0x0B31)//Sklo
          {
            UO.PrintObject(o, "Sklo");
            o.Use();
            return;
          }
          else if (o.Graphic == 0x2000 && o.Color == 0x0000)//Truhla
          {
            UO.PrintObject(o, "Truhla");
            o.Use();
            return;
          }




          //Serial: 0x402C7C46  Position: 5261.157.15  Flags: 0x0000  Color: 0x0000  Graphic: 0x2000  Amount: 0  Layer: None Container: 0x00000000


          // Serial: 0x402DE34C  Name: "Recall Rune - Minoc Mining Ca"  Position: 142.111.0  Flags: 0x0000  Color: 0x0482  Graphic: 0x1F14  Amount: 1  Layer: None Container: 0x403174BC


          //          Serial: 0x402DFCAC  Name: "a porcelain clay"  Position: 3623.472.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x0913  Amount: 0  Layer: None Container: 0x00000000

          //Serial: 0x00375110  Name: "Matej"  Position: 2608.518.1  Flags: 0x0000  Color: 0x0421  Model: 0x0190  Renamable: False Notoriety: Innocent HP: -1 / -1

          //Serial: 0x4027C37D  Name: "a piece of glass"  Position: 2520.503.16  Flags: 0x0000  Color: 0x0B31  Graphic: 0x1366  Amount: 0  Layer: None Container: 0x00000000




          //          Serial: 0x400F5A69  Position: 818.2375.1  Flags: 0x0000  Color: 0x0000  Graphic: 0x3490  Amount: 0  Layer: None Container: 0x00000000

          //Serial: 0x402E44E2  Position: 838.2352.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x0C9A  Amount: 0  Layer: None Container: 0x00000000




          //Serial: 0x40260AF6  Position: 4815.1779.-20  Flags: 0x0000  Color: 0x084F  Graphic: 0x1777  Amount: 0  Layer: None  Container: 0x00000000


          /*TODO


          else if (o.Graphic == 0x09D7 && o.Color == 0x0422)//Hilina
          {
            UO.PrintObject(o, "Hlina");
            o.Use();
            return;
          }

          else if (o.Graphic == 0x09D7 && o.Color == 0x0422)//Hilina
          {
            UO.PrintObject(o, "Hlina");
            o.Use();
            return;
          }*/
        }
        else if (o.Distance <= 25 && o.Distance > 1)
        {
          if (o.Graphic == 0x1366 && o.Color == 0x0B31)//Sklo
          {
            UO.PrintObject(o, "Sklo");
          }
          else if (o.Graphic == 0x0913 && o.Color == 0x0000)//hlina ubra
          {
            UO.PrintObject(o, "Hlina");
          }
        }
      }
      // 0x0B1D  Graphic: 0x1ECD

    }


    //---------------------------------------------------------------------------------------------

  }
}

