using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;
using CalExtension.UOExtensions;

namespace CalExtension.Skills
{
  //,exec TrainInscription "" "" "Total Mana Refresh" true 50 "Spell Circles" "Spell Circle 7" "Spell Circle 7" "Energy Field"
  //,exec TrainInscription "" "" "Mana Refresh" true 10 "Spell Circles" "Spell Circle 1" "Spell Circle 1" "Heal"
  
    //,exec TrainInscription "Mana Refresh" "" "" false 50 "Spell Circles" "Spell Circle 4" "Spell Circle 4" "Recall"
  public class Inscription : Skill
  {

    public static UOItemType BlankScroll { get { return new UOItemType() { Graphic = 0x0E34, Color = 0x0000 }; } }
    public void Write(int? quantity, params string[] menus)
    {
      int itemMake = 0;

      Journal.Clear();
      Game.PrintMessage("Write - " + String.Join(",", menus));
      while (!UO.Dead && itemMake < quantity.GetValueOrDefault(1))
      {

        UO.UseType(BlankScroll.Graphic, BlankScroll.Color);
        UO.WaitMenu(menus);

        Journal.WaitForText(true, 15000, "You fail to inscribe", "You dont't know any", "You put");

        if (Journal.Contains("You put"))
          itemMake++;

        if (Journal.Contains("You dont't know any"))
        {
          Game.PrintMessage("Nemas suroviny");
          break;
        }

        Journal.Clear();

        if (!quantity.HasValue)
          break;

      }
    }
    //Serial: 0x403567CF  Position: 48.146.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x0F51  Amount: 1  Layer: None  Container: 0x4032F802 Dagger
    //---------------------------------------------------------------------------------------------
    public static UOItemType AtlasType { get { return new UOItemType() { Graphic = 0x0FBE, Color = 0x0B98 }; } }


    public void Train(string name, string quality, string drinkPotion, bool drink, int manaLimit, params string[] menus)
    {
      Game.PrintMessage("Zvol kontejner s materialem > (esc) = ground");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      Game.CurrentGame.Mode = GameMode.Working;

      decimal okCountAlch = 0;
      decimal countAlch = 0;

      decimal okCount = 0;
      decimal count = 0;

      bool restart = true;

      while (!UO.Dead)
      {
        Journal.Clear();
        if (!World.Player.Backpack.AllItems.FindType(BlankScroll.Graphic, BlankScroll.Color).Exist)
        {
          foreach (UOItem item in World.Player.Backpack.AllItems)
          {
            if (item.Graphic > 0x1F20 && item.Graphic < 0x1F99)
            {
              item.Move(10000, containerFrom);
              Game.Wait(500);
            }
          }

          UOItem sourceItem = containerFrom.Items.FindType(BlankScroll.Graphic, BlankScroll.Color);
          if (sourceItem.Exist)
          {
            sourceItem.Move(100, World.Player.Backpack);
            Game.Wait(500);
          }
          else
          {
            break;
          }
        }

        if (World.Player.Mana > manaLimit)
        {

          Game.PrintMessage("Write - " + String.Join(",", menus));
          UO.UseType(BlankScroll.Graphic, BlankScroll.Color);
          UO.WaitMenu(menus);
          Journal.WaitForText(true, 7500, "You fail to inscribe", "You dont't know any", "You put");

          count++;

          if (Journal.Contains("You put"))
            okCount++;


          if (Journal.Contains("You dont't know any"))
          {
            Game.PrintMessage("Nemas suroviny");
            break;
          }


          decimal okDivide = (okCount / count);
          decimal okPerc = okDivide * 100;
          Game.PrintMessage("Ks: " + okCount + " / " + count + " - " + String.Format("{0:n} %", okPerc));
        }

        Potion p = PotionCollection.Potions.GetItemByName(drinkPotion);

        UOItem mrKad = null;

        if (p != null)
        {
          mrKad = new UOItem(World.Player.Backpack.Items.FindType(Potion.KadGraphic, p.TopKadColor));
          if (mrKad == null || !mrKad.Exist)
              mrKad = World.Ground.FindType(Potion.KadGraphic, p.TopKadColor);
        }

//        Game.PrintMessage("" + (p != null) + " / " + (mrKad != null && mrKad.Exist));

        //new UOItem(World.Player.Backpack.Items.FindType(Potion.KadGraphic, Potion.ManaRefresh.TopKadColor));
        int manaOffset = 60;
        if (p != null && p.Name == "ManaRefresh")
          manaOffset = 25;

        if (World.Player.Mana < manaLimit || (restart && World.Player.Mana < (World.Player.Intelligence - manaOffset)))
        {
          restart = false;


          if (drink && mrKad != null && mrKad.Exist)
          {
            Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion(p);
            Game.Wait(1500);
            if (Journal.Contains("You can't drink", "You can't "))
            {
              Game.Wait(8000);
              Journal.Clear();
            }
          }
        }

        if (World.Player.Mana < manaLimit)
        {
          World.Player.ChangeWarmode(WarmodeChange.Peace);
          Game.Wait(250);
          int counter = 0;
          bool alchemyOK = true;
          double timeCounter = 0;
          restart = true;
          long lastDrink = 0;
          UOItem atlas = World.Player.Backpack.AllItems.FindType(AtlasType.Graphic, AtlasType.Color);

          //while (World.Player.Mana < World.Player.Intelligence)
          //{
            Journal.Clear();

            DateTime start = DateTime.Now;
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(quality) && alchemyOK)
            {
              Potion potion = null;
              PotionQuality pq = Potion.ParsePQOrDefault(quality, PotionQuality.Lesser);
              if ((potion = PotionCollection.Potions.GetItemByName(name)) != null)
              {
                Alchemy a = new Alchemy();

                if (a.GetMixureReagent(containerFrom, potion, pq))
                {
                  a.FillKadRecursive(potion, pq);
                  if (!a.MixurePotion(potion, pq, false))
                  {
                    break;
                  }
                  else
                  {
                    countAlch++;
                    if (Journal.Contains(true, "You pour"))
                      okCountAlch++;

                    decimal okDivide = (okCountAlch / countAlch);
                    decimal okPerc = okDivide * 100;
                    int regAmount = potion.GetAmountOrDefault(pq);
                    decimal pricePerPcs = (1 / (okDivide > 0 ? (okCountAlch / countAlch) : 1)) * (decimal)regAmount * 5.0m;//

                    Game.PrintMessage("Ks: " + okCountAlch + " / " + String.Format("{0:n} %", okPerc) + " Prc: " + String.Format("{0:n} gp", pricePerPcs));
                  }
                }
                Game.PrintMessage("Trenovani dokonceno.");
              }
              else
              {
                alchemyOK = false;
                Game.PrintMessage("Potion s nazvem " + name + " neexistuje", MessageType.Error);
              }
              //WatchDogHelper.Watch();

            }
            else if (atlas.Exist && atlas.Amount > 0)
            {
              //map 0x14EF

              foreach (UOItem map in World.Player.Backpack.AllItems)
              {
                if (map.Graphic == 0x14EB && map.Color == 0x0000)
                {
                  UO.WaitTargetObject(map);
                  atlas.Use();
                  Game.Wait();
                }
              }

              if (!World.Player.Backpack.AllItems.FindType(0x14EB, 0x0000).Exist)
              {
                UO.WaitTargetSelf();
                atlas.Use();
                Game.Wait();
              }

              if (World.Player.Backpack.AllItems.FindType(0x14EB, 0x0000).Exist)
              {
                UO.WaitMenu("What sort of map do you want to draw ?", "Detail Map");
                UO.UseType(0x14EB, 0x0000);
                JournalEventWaiter jew = new JournalEventWaiter(true, "You put the map", "Thy trembling hand results in an unusable map");
                jew.Wait(5000);
              }
            }
            else if (World.Player.Backpack.AllItems.FindType(0x0E9D).Exist && SkillsHelper.GetSkillValue("Musicianship").RealValue < 500)
            {

              UO.UseType(0x0E9D, 0x0000);
              if (Journal.WaitForText(true, 2500, "You play poorly")) { }
              //Game.Wait(1000);

              if (Journal.Contains(true, " You are preoccupied with thoughts of battle"))
              {
                Game.CurrentGame.CurrentPlayer.SwitchWarmode();
                Game.Wait();
              }
            }
            else if (World.Player.Backpack.AllItems.FindType(0x1438).Exist && SkillsHelper.GetSkillValue("ItemIdentification").RealValue < 1000)//war hamer
            {
              UO.WaitTargetType(0x1438);
              UO.UseSkill(StandardSkill.ItemIdentification);
              Game.Wait(1500);
            }
            //else if (SkillsHelper.GetSkillValue("DetectingHidden").RealValue < 150)
            //{
            //  UO.UseSkill(StandardSkill.DetectingHidden);
            //  Game.Wait(1000);
            //}

            else
            {
              UO.UseSkill(StandardSkill.Meditation);
              Game.Wait(2500);
            }

            Game.Wait(100);
            counter++;



            double elapsedTime = (DateTime.Now - start).TotalMilliseconds;
            timeCounter += elapsedTime;

            int longpartCount = (int)Math.Truncate(timeCounter) / 5000;
            Game.PrintMessage("Cas:" + (longpartCount * 5000) + " / " + ((longpartCount * 5000) - lastDrink));

            if ((longpartCount * 5000) - lastDrink > 10000)
            {
              lastDrink = (longpartCount * 5000);
              if (drink && mrKad != null && mrKad.Exist && World.Player.Mana < (World.Player.Intelligence - manaOffset))//        if (World.Player.Mana < 50 || (restart && World.Player.Mana < (World.Player.Intelligence - manaLimit)))
              {
                Journal.Clear();
                Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion(p);
                Game.Wait(1500);
              }
            }
          //}
        }

      }
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("WriteScroll")]
    [BlockMultipleExecutions]
    public static void ExecMake(int quantity, params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Inscription>().Write(quantity, menus);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("TrainInscription")]
    [BlockMultipleExecutions]
    public static void ExecTrain(string dringPoiton, bool drink, params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Inscription>().Train(null, null, dringPoiton, drink, 50, menus);
    }

    [Executable("TrainInscription")]
    [BlockMultipleExecutions]
    public static void ExecTrain(string a, string b, string dringPoiton, bool drink, params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Inscription>().Train(a, b, dringPoiton, drink, 50, menus);
    }

    [Executable("TrainInscription")]
    [BlockMultipleExecutions]
    public static void ExecTrain(string a, string b, string dringPoiton, bool drink, int manaLimit,params string[] menus)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Inscription>().Train(a, b, dringPoiton, drink, manaLimit, menus);
    }

    #endregion
  }
}
