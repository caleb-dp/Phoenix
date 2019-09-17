using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using CalExtension.UOExtensions;
using System.Timers;

namespace CalExtension.Skills
{
  public class Alchemy : Skill
  {
    //,exec gmmortar "Total Mana Refresh (612 Eyes of Newt nebo 306 Blue Eyes of Newt)"
    //---------------------------------------------------------------------------------------------

    public static UOItem Mortar
    {
      get
      {
        return World.Player.Backpack.Items.FindType(0x0E9B);
      }
    }


    //---------------------------------------------------------------------------------------------

    public void TrainAlchemy(string name, string quality)
    {
      Potion potion = null;
      PotionQuality pq = Potion.ParsePQOrDefault(quality, PotionQuality.Lesser);
      if ((potion = PotionCollection.Potions.GetItemByName(name)) != null)
      {
        TrainAlchemy(potion, pq);
      }
      else
        Game.PrintMessage("Potion s nazvem " + name + " neexistuje", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    public void TrainAlchemy(Potion potion, PotionQuality quality)
    {
      //WatchDogHelper.Watch();
      Game.CurrentGame.Mode = GameMode.Working;
      Game.PrintMessage("Vyber zdrojovy konterjner s regy:");
      UOItem container = new UOItem(UIManager.TargetObject());
      ItemHelper.EnsureContainer(container);

      decimal okCount = 0;
      decimal count = 0;

      while (GetMixureReagent(container, potion, quality))
      {
        FillKadRecursive(potion, quality);
        if (!MixurePotion(potion, quality))
        {
          //break;
        }
        else
        {
          count++;
          if (Journal.Contains(true, "You pour"))
            okCount++;

          decimal okDivide = (okCount / count);
          decimal okPerc = okDivide * 100;
          int regAmount = potion.GetAmountOrDefault(quality);


          decimal pricePerPcs = (1 / (okDivide > 0 ? (okCount / count) : 1)) * (decimal)regAmount * 5.0m;//

          Game.PrintMessage("Ks: " + okCount + " / " + count + " : " + String.Format("{0:n} %", okPerc));
        }
      }
      Game.PrintMessage("Trenovani dokonceno.");
    }

    //---------------------------------------------------------------------------------------------


    public void TrainAlchemyMag(Potion potion, PotionQuality quality)
    {
      //WatchDogHelper.Watch();
      Game.CurrentGame.Mode = GameMode.Working;
      Game.PrintMessage("Vyber zdrojovy konterjner s regy:");
      UOItem container = new UOItem(UIManager.TargetObject());
      ItemHelper.EnsureContainer(container);
      while (GetMixureReagent(container, potion, quality))
      {
        FillKadRecursive(potion, quality);
        if (!MixurePotion(potion, quality))
        {
          break;
        }
      }
      Game.PrintMessage("Trenovani dokonceno.");
    }

    public void TrainAlchemyMag(string name, string quality)
    {
      Game.PrintMessage("TrainAlchemyMag");
      Potion potion = PotionCollection.Potions.GetItemByName(name);
      PotionQuality pq = Potion.ParsePQOrDefault(quality, PotionQuality.Lesser);

      Game.PrintMessage("Vyber zdrojovy konterjner s regy:");
      UOItem container = new UOItem(UIManager.TargetObject());
      ItemHelper.EnsureContainer(container);


      decimal okCount = 0;
      decimal count = 0;

      while (!UO.Dead &&
        World.Player.Backpack.AllItems.FindType(Reagent.Nightshade.Graphic, Reagent.Nightshade.Color).Exist &&
        World.Player.Backpack.AllItems.FindType(Reagent.BlackPearl.Graphic, Reagent.BlackPearl.Color).Exist 
        )
      {
        while (World.Player.Mana > 10)
        {
          if (World.Player.Hits < 40)
          {
            while (World.Player.Hits < World.Player.Strenght)
            {
              UOItem banda = World.Ground.FindType(0x0E21);//bandy
              if (!banda.Exist)
                return;

              UO.WaitTargetSelf();
              banda.Use();
              Game.Wait(2500);
            }
          }

          UO.Cast(StandardSpell.MagicArrow, Aliases.Self);
          Game.Wait(1500);
        }

        while (World.Player.Hits < World.Player.Strenght)
        {
          UOItem banda = World.Ground.FindType(0x0E21);//bandy
          if (!banda.Exist)
            break;

          UO.WaitTargetSelf();
          banda.Use();
          Game.Wait(2500);
        }

        //UOItem mrKad = new UOItem(World.Player.Backpack.Items.FindType(Potion.KadGraphic, Potion.ManaRefresh.TopKadColor));
        //UOItem tmrKad = new UOItem(World.Player.Backpack.Items.FindType(Potion.KadGraphic, Potion.TotalManaRefresh.TopKadColor));
        //if (mrKad.Exist || tmrKad.Exist)
        //{
        //  Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion("Total Mana Refresh");
        //  Game.Wait();
        //}
        //else
        {
          while (GetMixureReagent(container, potion, pq) && World.Player.Mana < World.Player.Intelligence)
          {
            FillKadRecursive(potion, pq);
            if (!MixurePotion(potion, pq))
            {
              break;
            }
            else
            {
              count++;
              if (Journal.Contains(true, "You pour"))
                okCount++;

              decimal okDivide = (okCount / count);
              decimal okPerc = okDivide * 100;
              int regAmount = potion.GetAmountOrDefault(pq);


              decimal pricePerPcs = (1 / (okDivide > 0 ? (okCount / count) : 1)) * (decimal)regAmount * 5.0m;//

              Game.PrintMessage("Ks: " + okCount + " / " + String.Format("{0:n} %", okPerc) + " Prc: " + String.Format("{0:n} gp", pricePerPcs));
            }
          }
        }
      }
      Game.PrintMessage("TrainAlchemyMag END");
    }

    //---------------------------------------------------------------------------------------------

    public bool GetMixureReagent(UOItem container, Potion potion, PotionQuality quality)
    {
      if (!potion.Reagent.IsValid)
      {
        Game.PrintMessage("!potion.Reagent.IsValid :", MessageType.Error);
        return false;
      }
      int amount = potion.GetAmountOrDefault(quality);
      UOItem resource = Reagent.FindReagent(container.Serial, potion.Reagent.Graphic, potion.Reagent.Color);
      if (resource.Exist && amount > 0)
      {
        //UOItem invetoryRg = new UOItem(Serial.Invalid);
        //if ((invetoryRg = Reagent.FindReagent(UO.Backpack.Serial, potion.Reagent.Graphic, potion.Reagent.Color)).Exist)
        //{
        //  invetoryRg.Move(invetoryRg.Amount, container);
        //  Game.Wait(Settings.SmallestWait);
        //}
        resource.Move((ushort)amount, UO.Backpack.Serial, 20, 20);
        Game.Wait(Game.SmallestWait);
        return true;
      }
      else
      {
        Game.PrintMessage("Ve zdrojovem kontaineru nejsou regy.", MessageType.Error);
      }

      return false;
    }

    //---------------------------------------------------------------------------------------------


    public void MixurePotion(string name, string quality)
    {
      MixurePotion(name, quality, false);
    }

    public void MixurePotion(string name, string quality, bool findInAll)
    {
      Potion potion = null;
      PotionQuality pq = Potion.ParsePQOrDefault(quality, PotionQuality.Lesser);
      if ((potion = PotionCollection.Potions.GetItemByName(name)) != null )
      {
        MixurePotion(potion, pq);
      }
      else
        Game.PrintMessage("Potion s nazvem " + name + " neexistuje", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    public bool MixurePotion(Potion potion, PotionQuality quality)
    {
      return MixurePotion(potion, quality, true);
    }

    public bool MixurePotion(Potion potion, PotionQuality quality, bool findInAll)
    {
      int amount = potion.GetAmountOrDefault(quality);
      UOItem invetoryRg = Reagent.FindReagentAll(UO.Backpack.Serial, potion.Reagent.Graphic, potion.Reagent.Color, amount);

      if (!invetoryRg.Exist)
      {
        Game.PrintMessage("Nejsou regy.", MessageType.Error);
        return false;
      }

      if (!Mortar.Exist)
      {
        Game.PrintMessage("Neni mortar.", MessageType.Error);
        return false;
      }
      Game.RunScript(amount * 6000);

      Game.PrintMessage("Micham:" + potion.Name + " - " + quality.ToString());
      Journal.Clear();

      Targeting.ResetTarget();
      UO.WaitMenu("Vyber typ potionu", potion.GetQualityDefinition(quality).MenuName);
      Mortar.Use();
      JournalEventWaiter jew = new JournalEventWaiter(true, "*You toss", "You can't make another potion now", "You completed the mixture in the mortar", "Musis mit v batuzku prazdnou lahev.");
      DateTime dt1 = DateTime.Now;
      if (jew.Wait(60000))
      {

        if (Journal.Contains(true, "You can't make another potion now"))
        {
          Game.Wait(4000);
          return true;
        }
        else if (Journal.Contains(true, "You toss"))
        {
          return true;
        }
        else if (Journal.Contains(true, "Musis mit v batuzku prazdnou lahev."))
        {
          UIManager.Reset();
          return false;
        }
        else
        {
          Mortar.Use();
          if (Journal.WaitForText(false, 500, "You pour the completed"))
            return true;
        }
      }

      return true;
    }

    //---------------------------------------------------------------------------------------------

    protected string selectedPotion;
    protected string SelectedPotion
    {
      get { return this.selectedPotion; }
      set
      {
        this.selectedPotion = value;

        UOColor c = Game.Val_PureWhite;

        Potion p = PotionCollection.Potions.GetItemByName(this.selectedPotion);
        if (p != null)
          c = p.TopKadColor;
        
        World.Player.PrintMessage(value, c);
      }
    }

    //---------------------------------------------------------------------------------------------


    public void MovePotionNext(int direction, params string[] potions)
    {
      int index = 0;
      try
      {
        if (!String.IsNullOrEmpty(selectedPotion))
          index = UOExtensions.Utils.GetSwitchIndex(Array.IndexOf(potions, this.selectedPotion), direction, potions.Length);
      }
      catch { index = 0; }


      this.SelectedPotion = potions[index];
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem FindPotion(Potion potion, ItemsCollection serachCol)
    {
      Graphic gra = potion.DefaultGraphic;
      UOColor color = potion.DefaultColor;

      UOItem potionItem = new UOItem(Serial.Invalid);
      List<UOItem> matchItems = serachCol.Where(i => i.Graphic == gra && i.Color == color).ToList();

      if (matchItems.Count > 0)
        potionItem = matchItems[0];
      else
      {
        foreach (UOItem item in serachCol)
        {
          if (item.Graphic == gra)
          {
            if (item.Color == color)
            {
              potionItem = item;
              break;
            }
            else
            {
              if (String.IsNullOrEmpty(item.Name))
              {
                item.Click();
                Game.Wait(100);
              }

              string name = String.Empty + item.Name;
              if (name.ToLower().Contains(potion.name.ToLower()))
              {
                potionItem = item;
                break;
              }
            }
          }
        }
      }

      return potionItem;
    }

    //---------------------------------------------------------------------------------------------

    public void DrinkPotionOrDefault(string defaultPotion)
    {
      if (String.IsNullOrEmpty(this.SelectedPotion))
        this.SelectedPotion = defaultPotion;

      if (!String.IsNullOrEmpty(this.SelectedPotion))
        DrinkPotion(this.SelectedPotion);
      else
        World.Player.PrintMessage("[Zvol potion..]", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    public void DrinkPotion(params string[] nameAlters)
    {
      Potion potion = null;

      foreach (string name in nameAlters)
      { 
        if ((potion = PotionCollection.Potions.GetItemByName(name)) != null)
        {
          if (DrinkPotion(potion))
            return;
        }
      }

      Game.PrintMessage("Potion s nazvem " + (String.Join(", ", nameAlters)) + " neexistuje", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    public bool DrinkPotion(Potion potion)
    {
      UOItem potionItem = FindPotion(potion, World.Player.Backpack.AllItems);

      if (!potionItem.Exist)
      {
        ItemHelper.OpenContainerRecursive(World.Player.Backpack);
        potionItem = GetPotionFromKad(potion);
      }

      if (potionItem.Exist)
      {
        Journal.Clear();
        potionItem.Use();

        if (Journal.WaitForText(true, 250, "You can't drink another potion yet"))
        {

          if (Game.CurrentGame.LastDrinkTime != null && (17.0 - (DateTime.Now - Game.CurrentGame.LastDrinkTime.Value).TotalSeconds) >= 0)
            World.Player.PrintMessage(String.Format("[Lahev za {0:N1}s]", 17.0 - (DateTime.Now - Game.CurrentGame.LastDrinkTime.Value).TotalSeconds));

          if (Potion.GetEmptyPotion().Amount > 2)
          {
            GetPotionFromKad(potion);
          }
        }
        else
          Game.CurrentGame.LastDrinkTime = DateTime.Now;

        return true;
      }
      else
      {
        World.Player.PrintMessage("[Neni " + potion.Shortcut + "]", MessageType.Error);
        return false;
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void GetPotionFromKad(params string[] nameAlters)
    {
      Potion potion = null;

      foreach (string name in nameAlters)
      {
        if ((potion = PotionCollection.Potions.GetItemByName(name)) != null)
        {
          Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().GetPotionFromKad(potion);
          return;
        }
      }

      Game.PrintMessage("Potion s nazvem " + (String.Join(", ", nameAlters)) + " neexistuje", MessageType.Error);
    }

    //---------------------------------------------------------------------------------------------

    public UOItem GetPotionFromKad(Potion potion)
    {
      UOItem potionItem = FindPotion(potion, World.Player.Backpack.AllItems);
      UOItem kad = GetKadAny(potion);

      Game.RunScriptCheck(400);

      if (kad.Exist)
      {
        UOItem emptyPotion = Potion.GetEmptyPotion();
        if (emptyPotion.Exist)
        {
          Journal.Clear();
          Targeting.ResetTarget();


          potionItem = FindPotion(potion, World.Player.Backpack.AllItems);
          if (!potionItem.Exist)
          {
            Game.CurrentGame.CurrentPlayer.SwitchWarmode();
            Game.Wait(25);
          }

          UO.WaitTargetObject(emptyPotion);
          kad.Use();
          if (Journal.WaitForText(true, 150, "Pri praci s nadobou nemuzes delat neco jineho", "You put the"))
          {
            if (Journal.Contains(true, "Pri praci s nadobou nemuzes delat neco jineho"))
            {
              if (!potionItem.Exist)
              {
                Game.CurrentGame.CurrentPlayer.SwitchWarmode();
                Game.Wait(25);

                UO.WaitTargetObject(Potion.GetEmptyPotion());
                kad.Use();
                Game.Wait(75);
              }
            }
          }
          Game.Wait(75);

          //Game.Wait(200);

          potionItem = FindPotion(potion, World.Player.Backpack.AllItems);
        }
      }
      else
        World.Player.PrintMessage(String.Format("[Neni kad {0}]", potion.Shortcut), MessageType.Warning);

      return potionItem;
    }

    //---------------------------------------------------------------------------------------------

    public UOItem FindKad(Potion potion, ItemsCollection serachCol)
    {
      UOItem kad = new UOItem(Serial.Invalid);

      foreach (KeyValuePair<PotionQuality, PotionQualityDefinition> kvp in potion.Qualities)
      {
        List<UOItem> matchItems = serachCol.Where(i => i.Graphic == kvp.Value.KadGraphic && i.Color == kvp.Value.KadColor).ToList();

        if (matchItems.Count > 0)
        {
          kad = matchItems[0];
          break;
        }
        else
        {

          foreach (UOItem item in serachCol)
          {
            if (item.Distance > 5)
              continue;

            if (item.Graphic == kvp.Value.KadGraphic)
            {
              if (item.Color == kvp.Value.KadColor)
              {
                return item;
              }
              else
              {
                if (String.IsNullOrEmpty(item.Name))
                {
                  item.Click();
                  Game.Wait(100);
                }

                string itemName = item.Name + String.Empty;
                string searchName = (kvp.Key == PotionQuality.None ? "" : kvp.Key.ToString() + " ") + potion.name;

                if (kvp.Key == PotionQuality.None)
                {
                  if (itemName.ToLower() == searchName.ToLower())
                    return item;
                }
                else
                {
                  if (itemName.ToLower().Contains(searchName.ToLower()))
                    return item;
                }
              }
            }
          }
        }
      }

      return kad;
    }

    //---------------------------------------------------------------------------------------------

    public UOItem GetKadAny(Potion potion)
    {
      UOItem kad = FindKad(potion, World.Player.Backpack.AllItems);

      if (!kad.Exist)
        kad = FindKad(potion, World.Ground);

      if (!kad.Exist || !kad.Serial.IsValid)
      {
        ItemHelper.OpenContainerRecursive(World.Player.Backpack);
        kad = FindKad(potion, World.Player.Backpack.AllItems);

      }
      return kad;
    }

    //---------------------------------------------------------------------------------------------

    protected UOItem FindKadAround(Graphic graphic, UOColor color)
    {
      UOItem kad = UO.Backpack.Items.FindType(graphic, color);
      if (!kad.Exist)
        kad = World.Ground.FindType(graphic, color);

      if (kad.Exist && kad.Distance < 4)
      {
        return kad;
      }
      else
        return new UOItem(Serial.Invalid);
    }


    //---------------------------------------------------------------------------------------------

    public void FillKadRecursive(Potion potion, PotionQuality quality)
    {
      UOItem p = UO.Backpack.Items.FindType(potion.DefaultGraphic, potion.DefaultColor);

      PotionQualityDefinition pqDef = potion.GetQualityDefinition(quality);
      UOColor kadColor = pqDef != null ? pqDef.KadColor : potion.TopKadColor;
      UOColor potionColor = pqDef != null ? pqDef.Color : potion.DefaultColor;

      UOItem kad = FindKadAround(Potion.KadGraphic, kadColor);
      if (p.Exist && kad.Exist)
      {
        Journal.Clear();
        //Game.Wait(250);
        kad.Use();
        UO.WaitTargetObject(p);
        JournalEventWaiter jew = new JournalEventWaiter(true, "Tohle nejde");
        jew.Wait(250);
        if (UO.Backpack.Items.FindType(potion.DefaultGraphic, potionColor).Exist)
          FillKadRecursive(potion, quality);
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void gmmortar(string potionName)
    {
      World.Player.PrintMessage("Vyber cilovou kad:", Game.Val_SuperLightYellow);
      UOItem kadDo = new UOItem(UIManager.Target().Serial);
      if (!kadDo.Exist)
      {
        World.Player.PrintMessage("Neni cilova kad", Game.Val_SuperLightYellow);
        return;
      }
      else
        UO.PrintObject(kadDo, Game.Val_SuperLightYellow, "[Kad...]");

      UOItem gmMortar = World.Ground.FindType(0x0E9B, 0x0058);
      if (!gmMortar.Exist)
      {
        World.Player.PrintMessage("Neni GM mortar", Game.Val_SuperLightYellow);
        return;
      }
      else
        UO.PrintObject(gmMortar, Game.Val_SuperLightYellow, "[Mortar...]");



      while (!UO.Dead && kadDo.Exist && gmMortar.Exist)
      {
        if (World.Player.Backpack.Items.FindType(0x1843, 0x0000).Exist && World.Player.Backpack.Items.FindType(0x0F0E, 0x0000).Exist)
        {
          UO.WaitMenu("Vyber typ potionu", potionName);
          gmMortar.Use();
          Game.Wait();
          
          foreach(UOItem item in World.Player.Backpack.Items)
          {
            if (item.Serial == kadDo.Serial)
              continue;

            if (item.Graphic == kadDo.Graphic && item.Color == kadDo.Color)
            {
              UO.WaitTargetObject(kadDo);
              item.Use();

              Game.Wait();

              List<Serial> origItems = new List<Serial>();
              foreach (UOItem oi in World.Player.Backpack.Items)
                origItems.Add(oi.Serial);


              UO.WaitTargetObject(World.Player.Backpack.Items.FindType(0x0F0E, 0x0000));
              item.Use();

              Game.Wait();

              UOItem potion = null;

              foreach (UOItem oi in World.Player.Backpack.Items)
              {
                if (!origItems.Contains(oi.Serial))
                {
                  potion = oi;
                  break;

                }
              }

              if (potion != null)
              {
                UO.WaitTargetObject(potion);
                kadDo.Use();

                Game.Wait();
              }
            }
          }
        }
        else
        {
          World.Player.PrintMessage("Neni prazdna kad/lahve", Game.Val_SuperLightYellow);
          break;
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void kadtokad()
    {
      World.Player.PrintMessage("Vyber zdrojovou kad:", Game.Val_SuperLightYellow);
      UOItem kadFrom = new UOItem(UIManager.Target().Serial);

      World.Player.PrintMessage("Vyber cilovou kad:", Game.Val_SuperLightYellow);
      UOItem kadDo = new UOItem(UIManager.Target().Serial);

      UOItemExtInfo info = ItemHelper.GetItemExtInfo(kadFrom, null);
      
    }
   

    //---------------------------------------------------------------------------------------------

    #region exec

    [Executable("TrainAlchemy")]
    [BlockMultipleExecutions]
    public static void ExecTrainAlchemy(string name, string quality)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().TrainAlchemy(name, quality);
    }

    [Executable("TrainAlchemyMag")]
    [BlockMultipleExecutions]
    public static void ExecTrainAlchemyMag(string name, string quality)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().TrainAlchemyMag(name, quality);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MixurePotion")]
    [BlockMultipleExecutions]
    public static void ExecMixurePotion(string name, string quality)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MixurePotion(name, quality);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MixurePotion")]
    [BlockMultipleExecutions]
    public static void ExecMixurePotion(string name, string quality, bool findInAll)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MixurePotion(name, quality, findInAll);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("DrinkPotion")]
    [BlockMultipleExecutions]
    public static void ExecDrinkPotion(params string[] nameAlters)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotion(nameAlters);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("DrinkPotionOrDefault")]
    [BlockMultipleExecutions]
    public static void ExecDrinkPotionOrDefault(string defaultPotion)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().DrinkPotionOrDefault(defaultPotion);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("MovePotionNext")]
    [BlockMultipleExecutions]
    public static void ExecMovePotionNext(int direction, params string[] potions)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Alchemy>().MovePotionNext(direction, potions);
    }


    #endregion
  }
 

  //---------------------------------------------------------------------------------------------
}
