//using System;
//using System.Collections.Generic;
//using System.Text;
//using Phoenix.WorldData;
//using Phoenix.Runtime;
//using Phoenix;
//using Caleb.Library;
//using System.Threading;
//using CalExtension.Skills;
//using Caleb.Library.CAL.Business;
//using System.Collections;
//using CalExtension.UOExtensions;
//using Phoenix.Communication;
//using System.Text.RegularExpressions;

//namespace CalExtension.Skills
//{
//  //Serial: 0x401AE86C  Name: "Training Taming Staff crafted"  Position: 72.92.0  Flags: 0x0000  Color: 0x04B9  Graphic: 0x13F4  Amount: 1  Layer: LeftHand  Container: 0x00343F08

//  public class Taming : Skill
//  {
//    public UOItemType TrainTamingStaff { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x04B9 }; } }
//    public UOItemType TamingStaff { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x076B }; } }
//    public UOItemType TamingStaffCharged { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x096D }; } }
//    //---------------------------------------------------------------------------------------------

//    public Taming()
//    {
//      this.doneList = new List<Serial>();
//    }

//    public static Graphic[] ShrinkKlamaci
//    {
//      get
//      {
//        Graphic[] lootItem = new Graphic[38];
//        lootItem[0] = 0x2121;
//        lootItem[1] = 0x2120;
//        lootItem[2] = 0x211F;
//        lootItem[3] = 0x2124;
//        lootItem[4] = 0x20F6;
//        lootItem[5] = 0x2137;
//        lootItem[6] = 0x2126;
//        lootItem[7] = 0x2127;
//        lootItem[8] = 0x2135;
//        lootItem[9] = 0x20E1; // polar bear
//        lootItem[10] = 0x20F7; // panther, walrus
//        lootItem[11] = 0x2119; // leopard/snow leopard1
//        lootItem[12] = 0x2102; // leopard/snow leopard2
//        lootItem[13] = 0x211D; // eagle
//        lootItem[14] = 0x2136; // zostrich
//        lootItem[15] = 0x2123; //  Rat
//        lootItem[16] = 0x20D1; // Chicken
//        lootItem[17] = 0x211B; // Cat
//        lootItem[18] = 0x2D97; // Veverka
//        lootItem[19] = 0x20EE; // Papousek
//        lootItem[20] = 0x20D4; // Hind
//        lootItem[21] = 0x2130; // Frog
//        lootItem[22] = 0x20EA; // Timber wolf
//        lootItem[23] = 0x20FC; // Snake
//        lootItem[24] = 0x20CF; // Brown bear
//        lootItem[25] = 0x2118; // Black bear
//        lootItem[26] = 0x2108; // Goat
//        lootItem[27] = 0x2101; // Pig
//        lootItem[28] = 0x211C; // Dog
//        lootItem[29] = 0x20D0; // Giant Rat
//        lootItem[30] = 0x20F5; // Gorila
//        lootItem[31] = 0x211E; // Grizzli bear
//        lootItem[32] = 0x20E6; // Lamb
//        lootItem[33] = 0x20EF; // Bull
//        lootItem[34] = 0x2103; // Cow
//        lootItem[35] = 0x20E2; // Jackrabit
//        lootItem[36] = 0x20EB; // Sheep
//        lootItem[37] = 0x2131; // Aligator
//        return lootItem;
//      }
//    }

//    public static List<Graphic> TrofejKlamaci
//    {
//      get
//      {
//        List<Graphic> list = new List<Graphic>();
//        list.Add(0x00EE);//rat
//        list.Add(0x00ED);//hind
//        list.Add(0x00D1);//goat
//        list.Add(0x00D3);//"Brown Bear
//        list.Add(0x00E1);//Wolf
//        list.Add(0x0034);//snake
//        list.Add(0x00D4);//bear
//        list.Add(0x00CD);//rabid
//        return list;
//      }
//    }

//    //    Serial: 0x00372C3D  Name: "Brown Bear"  Position: 3254.1948.23  Flags: 0x0040  Color: 0x01BB  Model: 0x00D3  Renamable: False Notoriety: Neutral HP: 100/100

//    //Serial: 0x00372C3D  Name: "Brown Bear"  Position: 3254.1948.23  Flags: 0x0040  Color: 0x01BB  Model: 0x00D3  Renamable: False Notoriety: Neutral HP: 100/100


//    //Serial: 0x0035DCA1  Name: "Goat"  Position: 1206.1594.0  Flags: 0x0000  Color: 0x0000  Model: 0x00D1  Renamable: False Notoriety: Neutral HP: 50/50

//    public UOItem EnsuredTamingStaff
//    {
//      get
//      {
//        UOItem tamingStaff = UO.Backpack.AllItems.FindType(0x13F4, 0x096D); //*/TamingStaffCharged.FindItem(Precision.GraphicColor, Search.Both);
//        if (!tamingStaff.Exist)
//        {
//          Game.CurrentGame.Messages.Add("What  EnsuredTamingStaff!");
//          tamingStaff = UO.Backpack.AllItems.FindType(0x13F4, 0x076B);  //TamingStaff.FindItem(Precision.GraphicColor, Search.Both);
//          if (tamingStaff.Exist)// && shringKad.Exist)
//          {
//            UO.WaitTargetType(0x1843, 0x0724);
//            tamingStaff.Use();
//            Game.Wait();
//          }
//        }
//        return tamingStaff;
//      }
//    }

//    public List<Serial> doneList;
//    public void TrainManual()
//    {
//      this.doneList = new List<Serial>();
//      if (!TrainTamingStaff.FindItem(Precision.GraphicColor, Search.Both).Exist)
//      {
//        Game.CurrentGame.Messages.Add("Neni taming staff");
//        return;
//      }

//      while (TrainTamingStaff.FindItem(Precision.GraphicColor, Search.Both).Exist)
//      {

//        foreach (UOCharacter character in World.Characters)
//        {
//          Game.Wait(50);

//          if (character.Distance < 5 && character.Serial != World.Player.Serial && !doneList.Contains(character.Serial))
//          {
//            this.TameCharacter(character, 100, false);
//          }
//        }
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    public void TrainTamingRecusive(int maxTries, params string[] positionsDefinition)
//    {
//      Robot r = new Robot();
//      r.UseTryGoOnly = true;
//      r.SearchSuqareSize = 450;
//      this.doneList = new List<Serial>();


//      string[] locations = positionsDefinition;

//      foreach (string loc in locations)
//      {
//        string[] options = loc.Split('|');

//        int button = -1;
//        string bookType = "r";

//        if (!String.IsNullOrEmpty(options[0]) && Regex.IsMatch(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})"))
//        {
//          Match m = Regex.Match(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})");
//          bookType = m.Groups["booktype"].Value.ToLower();
//          button = Int32.Parse(m.Groups["button"].Value);
//        }

//        if (button > -1)
//        {
//          string book = "RuneBookUse";
//          if (bookType == "t")
//          {
//            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
//            UO.Wait(1000);
//            book = "TravelBookUse";
//          }
//          else if (bookType == "c")
//          {
//            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["CestovniKnihaUse"], 1);
//            UO.Wait(1000);
//            book = "CestovniKnihaUse";
//          }



//          bool teleported = false;
//          while (!teleported)
//          {
//            UO.DeleteJournal();

//            Phoenix.Runtime.RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList[book], button);
//            Game.Wait(500);
//            if (!World.Player.Hidden)
//              UO.UseSkill("Hiding");

//            UO.Print("Cekam na kop.. nehybat");

//            if (Journal.WaitForText(true, 2000, "Nesmis vykonavat zadnou akci"))
//            {
//              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
//              Game.Wait(1000);
//            }
//            else if (Journal.WaitForText(true, 120000, "You have been teleported"))
//              teleported = true;

//            if (Game.CurrentGame.WorldSave())
//            {
//              UO.Print("WS opakovani kopu za 45s");
//              Game.Wait(45000);
//              if (bookType == "t")
//              {
//                Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
//                UO.Wait(1000);
//              }
//              Game.Wait(500);
//            }
//          }
//        }

//        for (int i = 1; i < options.Length; i++)
//        {
//          if (UO.Dead)
//            return;

//          string[] parm = options[i].Split('.');

//          UOPosition pos = new UOPosition(ushort.Parse(parm[0]), ushort.Parse(parm[1]), (ushort)0);

//          int distance = parm.Length > 2 ? Utils.ToNullInt(parm[2]).GetValueOrDefault(1) : 1;
//          int gotries = parm.Length > 3 ? Utils.ToNullInt(parm[3]).GetValueOrDefault(1000) : 1000;

//          Thread musicRun = Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TrainMusic"]);

//          if (r.GoTo(pos, distance, gotries))
//          {
//            Phoenix.Runtime.RuntimeCore.Executions.Terminate(musicRun.ManagedThreadId);
//            if (parm[parm.Length - 1].ToLower() == "opendoor")
//            {
//              ItemHelper.OpenDoor();
//              Game.Wait();
//            }

//            if (!TrainTamingStaff.FindItem(Precision.GraphicColor, Search.Both).Exist)
//            {
//              Game.CurrentGame.Messages.Add("Neni taming staff");
//              return;
//            }

//            List<UOCharacter> characters = new List<UOCharacter>();
//            characters.AddRange(World.Characters);
//            characters.Sort(delegate (UOCharacter char1, UOCharacter char2)
//            {
//              return char1.Distance.CompareTo(char2.Distance);
//            });

//            foreach (UOCharacter character in characters)
//            {
//              if (UO.Dead)
//                return;

//              Game.Wait(50);

//              if (character.Distance < 6 && character.Serial != World.Player.Serial && !doneList.Contains(character.Serial) && character.Model != 0x0190 && character.Model != 0x0191 && character.Model != 0x0192)
//              {
//                this.TameCharacter(character, maxTries);
//              }

//            }
//          }
//          else
//            Phoenix.Runtime.RuntimeCore.Executions.Terminate(musicRun.ManagedThreadId);
//        }
//      }

//      if (UO.Dead)
//        return;

//      TrainTamingRecusive(maxTries, positionsDefinition);
//    }

//    //---------------------------------------------------------------------------------------------

//    public void TameCharacter(UOCharacter character, int maxTries)
//    {
//      TameCharacter(character, maxTries, true);
//    }
//    public void TameCharacter(UOCharacter character, int maxTries, bool train)//, Robot r)
//    {

//      Robot r = new Robot();
//      r.UseTryGoOnly = true;
//      r.SearchSuqareSize = 20;

//      UOItem currentStaff = train ? TrainTamingStaff.FindItem(Precision.GraphicColor, Search.Both) : EnsuredTamingStaff;

//      Game.CurrentGame.CurrentPlayer.SwitchWarmode();
//      Game.Wait();

//      bool robeSwitch = false;
//      int tries = 0;
//      while (character.Exist && character.RequestStatus(500) && character.Hits > 0 && character.Distance < 6 && !UO.Dead)
//      {
//        UO.DeleteJournal();

//        bool end = false;
//        bool kill = false;

//        if (character.Distance > 1)
//        {
//          r.GoTo(new UOPosition(character.X, character.Y, 0), 1, 2);
//          Game.Wait(500);
//        }

//        UO.UseSkill("Hiding");
//        UO.Wait(450);
//        IRequestResult result = UO.WaitTargetObject(character.Serial);
//        currentStaff.Use();
//        //Game.Wait(500);

//        using (JournalEventWaiter jew = new JournalEventWaiter(true,
//          "You can't see the target",
//          "Ochoceni se nezdarilo",
//          "Your taming failed",
//          "byl tamnut",
//          "not tamable",
//          "You are not able to tame",
//          "Jsi moc daleko",
//          "Jeste nemuzes pouzit hulku"
//          ))
//        {

//          if (jew.Wait(15000))
//          {
//            if (Journal.Contains(true, "Ochoceni se nezdarilo"))
//            {
//              Game.CurrentGame.Messages.Add("Try - Ochoceni se nezdarilo");
//            }
//            else if (Journal.Contains(true, "Your taming failed"))
//            {
//              Game.CurrentGame.Messages.Add("Try - Your taming failed");
//            }
//            else if (Journal.Contains(true, "Jeste nemuzes pouzit hulku"))
//            {
//              Game.CurrentGame.Messages.Add("Try - Jeste nemuzes pouzit hulku");
//              Game.Wait(6000);
//            }
//            else if (Journal.Contains(true, "Jsi moc daleko"))
//            {
//              bool go = r.GoTo(new UOPosition(character.X, character.Y, 0), 1, 10);
//              if (!go)
//              {
//                end = true;
//              }
//              Game.Wait();
//              Game.CurrentGame.Messages.Add("Try - Jsi moc daleko: " + go);
//            }
//            else if (Journal.Contains(true, "You can't see the target"))
//            {
//              UO.WaitTargetCancel();
//              Game.Wait();
//              UO.Press(System.Windows.Forms.Keys.Escape);
//              Game.Wait();
//              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
//              Game.Wait();
//              bool go = r.GoTo(new UOPosition(character.X, character.Y, 0), 1, 10);
//              if (!go)
//              {
//                end = true;
//              }
//              Game.Wait();

//              Game.CurrentGame.Messages.Add("Try - You can't see the target go: " + go);
//            }
//            else if (Journal.Contains(true, "byl tamnut") && currentStaff.Color == TamingStaffCharged.Color)
//            {
//              Game.CurrentGame.Messages.Add("End - byl tamnut");
//              end = true;
//            }
//            else if (Journal.Contains(true, "You are not able to tame"))
//            {
//              if (robeSwitch)
//              {
//                Game.CurrentGame.Messages.Add("End - You are not able to tame");
//                /*kill = */
//                end = true;
//              }
//              else
//              {
//                robeSwitch = true;
//                if (World.Player.Backpack.AllItems.FindType(0x1F03).Exist)
//                {
//                  World.Player.Backpack.AllItems.FindType(0x1F03).Use();
//                }
//                Game.Wait(500);
//                Game.CurrentGame.Messages.Add("Try - You are not able to tame");
//              }
//            }
//            else if (Journal.Contains(true, "not tamable"))
//            {

//              character.Click();
//              Game.Wait(500);
//              character.RequestStatus(500);

//              bool isTrofejAnimal = false;// TrofejKlamaci.Contains(character.Model);

//              foreach (Graphic g in TrofejKlamaci)
//              {
//                if (g == character.Model)
//                  isTrofejAnimal = true;
//              }


//              if (isTrofejAnimal && character.Notoriety != Notoriety.Innocent && character.Notoriety != Notoriety.Invulnerable && character.Notoriety != Notoriety.Guild)//krysa/hind/bear/goat/snake atd)TODO
//              {
//                kill = true;
//                //zabit / riznout / vylotit
//                Game.CurrentGame.Messages.Add("Kill - not tamable " + character.Model);
//              }
//              else
//              {
//                Game.CurrentGame.Messages.Add("End - not tamable");
//                end = true;
//              }
//            }
//          }
//          else
//          {
//            if (Game.CurrentGame.WorldSave())
//            {
//              Game.Wait(60000);
//              Game.CurrentGame.Messages.Add("Try - WordSave" + character.Serial);
//            }
//            else
//            {


//              if (tries > 1)
//              {
//                Game.CurrentGame.Messages.Add("End - JEW timeout " + character.Serial);
//                end = true;
//              }
//            }
//          }

//          if (tries > maxTries)// && currentStaff.Color == TrainTamingStaff.Color)
//            end = true;

//          if (Journal.Contains(true, "You Cannot learn anything more from this animal"))
//          {
//            if (robeSwitch)
//            {
//              Game.CurrentGame.Messages.Add("End - You Cannot learn anything more from this animal");
//              //kill = end = true;

//              if (World.Player.Backpack.AllItems.FindType(0x1F03).Exist)
//              {
//                World.Player.Backpack.AllItems.FindType(0x1F03).Use();
//                Game.Wait(500);
//              }

//              currentStaff = this.EnsuredTamingStaff;
//              if (!currentStaff.Exist)
//                end = true;

//              Game.Wait();
//            }
//            else
//            {
//              robeSwitch = true;
//              if (World.Player.Layers[Layer.OuterTorso].Exist)
//              {
//                World.Player.Layers[Layer.OuterTorso].Move(1, World.Player.Backpack);
//              }
//              Game.Wait(1500);
//              Game.CurrentGame.Messages.Add("Try - You Cannot learn anything more from this animal");
//            }
//          }

//          if (kill)
//          {
//            if (UO.Backpack.AllItems.FindType(0x1400).Exist)
//            {
//              UO.WaitTargetObject(character);
//              UO.Backpack.AllItems.FindType(0x1400).Use();
//              Game.Wait();
//            }

//            int killTries = 0;
//            while (character.Exist && character.Distance < 8 && character.Hits > -1 && killTries < 30)
//            {
//              Game.CurrentGame.Messages.Add("Kill " + character.Name + " Trie:" + killTries);

//              if (World.Player.Hits < World.Player.MaxHits)
//              {
//                Healing.ExecBandage(10, "self");
//              }

//              World.Player.ChangeWarmode(WarmodeChange.War);
//              Game.Wait();

//              UO.Attack(character);
//              Game.Wait();
//              r.GoTo(character.X, character.Y, 1, 4);
//              character.Print("{0}/{1}", character.Hits, character.MaxHits);
//              killTries++;

//              Game.Wait(5000);
//            }

//            if (!character.Exist)
//            {
//              Game.Wait(500);
//              new Loot().LootGround(Loot.LootType.Quick, true);
//            }
//          }

//          if (end)
//          {
//            Game.CurrentGame.Messages.Add("While - end: " + tries);
//            doneList.Remove(character.Serial);
//            break;
//          }
//        }
//        tries++;
//        Game.CurrentGame.Messages.Add("While - cyklus pokus: " + tries);
//      }
//      Game.Wait(1000);

//      foreach (UOItem item in World.Ground)
//      {

//        if (item.Distance < 5)
//        {
//          item.Click();
//          Game.Wait(250);

//          bool isKlamak = false;
//          foreach (Graphic g in Taming.ShrinkKlamaci)
//          {
//            if (item.Graphic == g)
//              isKlamak = true;
//          }

//          Game.CurrentGame.Messages.Add("Item: " + item.Name + " Distance: " + item.Distance + " IsKlamak: " + (Array.IndexOf(ItemLibrary.ShrinkKlamaci.GraphicArray, item.Graphic) > -1 || Array.IndexOf(Taming.ShrinkKlamaci, item.Graphic) > -1) + " isKlamak: " + isKlamak);

//          if (Array.IndexOf(ItemLibrary.ShrinkKlamaci.GraphicArray, item.Graphic) > -1 || Array.IndexOf(ShrinkKlamaci, item.Graphic) > -1 || isKlamak)
//          {
//            Game.Wait();
//            item.Move(1, (ushort)(World.Player.X + 1), (ushort)(World.Player.Y + 1), item.Z);
//            Game.Wait();
//            item.Move(1, World.Player.Backpack, 30, 30);
//            Game.Wait();
//          }
//        }
//        Game.Wait();
//      }

//      Game.CurrentGame.Messages.Add("While - END");
//      if (World.Player.Backpack.AllItems.FindType(0x1F03).Exist)
//      {
//        World.Player.Backpack.AllItems.FindType(0x1F03).Use();
//        Game.Wait();
//      }
//    }

//    //---------------------------------------------------------------------------------------------

//    #region exec

//    //---------------------------------------------------------------------------------------------

//    [Executable("TrainTamingManual")]
//    [BlockMultipleExecutions]
//    public static void ExecTrainManual()
//    {
//      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming>().TrainManual();
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable("TrainTamingAuto")]
//    [BlockMultipleExecutions]
//    public static void ExecTrainTamingAuto(params string[] str)
//    {
//      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming>().TrainTamingRecusive(25, str);
//    }

//    //---------------------------------------------------------------------------------------------

//    [Executable("TrainTamingAuto")]
//    [BlockMultipleExecutions]
//    public static void ExecTrainTamingAuto(int maxTries, params string[] str)
//    {
//      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming>().TrainTamingRecusive(maxTries, str);
//    }



//    #endregion
//  }
//}
