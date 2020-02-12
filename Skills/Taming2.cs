using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;
using Caleb.Library.CAL.Business;
using System.Collections;
using CalExtension.UOExtensions;
using Phoenix.Communication;
using System.Text.RegularExpressions;
using CalExtension.Abilities;

namespace CalExtension.Skills
{
  /*
   Taming novy, mozne respy:
   
    Jhelom + ostrovy
   
    1451.3706|1343.3676|1317.3660|1301.3677|1285.3701|1251.3711|1284.3754|1304.3768|b1334.3765|1476.4004|1455.3989|1395.4004|1388.4028|1123.3603|1137.3589|1116.3583|1154.3620|1136.3634|1144.3439|

    Vesper
    2857.721|2811.980|2768.1011|

    Yew
    529.946|444.976|354.1034|659.964|680.1002|697.1014|551.1238|574.1243|

    Minoc / Nova nara
    2441.544|2547.342|2542.85|2584.116|2644.99|2590.191|

     Zento:
     351.573|351.538|461.580|519.469|520.457|536.471|155.474|
    


    Serpent:
    3161.1817|4632.303|4673.323|4712.158|2981.3410|2914.3382|3034.3375|3042.3402|2899.3504|2866.3488|2901.3529|2798.3524|

    Brit
    1650.1319|1617.1299|1594.1286|1491.1308|1408.1318|1439.1499|1221.1600|1158.1667|1200.1681|1183.1726|1133.1800|1219.1895|1284.1883|1340.1803|1374.1711|1406.1732|1313.1581|1342.3678|
     
     * 
     * * */





  //Serial: 0x401AE86C  Name: "Training Taming Staff crafted"  Position: 72.92.0  Flags: 0x0000  Color: 0x04B9  Graphic: 0x13F4  Amount: 1  Layer: LeftHand  Container: 0x00343F08

  public class Taming2 : Skill
  {
    public UOItemType TrainTamingStaff { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x04B9 }; } }
    public UOItemType TamingStaff { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x076B }; } }
    public UOItemType TamingStaffCharged { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x096D }; } }
    //---------------------------------------------------------------------------------------------

    private static Hashtable waipointHt;
    [Executable]
    public static void PrintWaipoint(string target)
    {
      if (waipointHt == null)
        waipointHt = new Hashtable();

      if (target == "none")
      {
        Notepad.Write(World.Player.X + "." + World.Player.Y + "|");
        World.Player.PrintMessage("[Print...]");
      }
      else
      {
        TargetInfo info = new TargetInfo(target).GetTarget();
        if (info.Success && waipointHt[info.Character.Serial] == null)
        {
          waipointHt[info.Character.Serial] = info;
          string name = String.Empty;
          if (info.Object.Exist && String.IsNullOrEmpty(info.Object.Name))
          {
            info.Object.Click();
            Game.Wait(100);
          }

          name = info.Object.Name;

          Notepad.Write(info.StaticTarget.X + "." + info.StaticTarget.Y + (String.IsNullOrEmpty(name) ? "" : "//" + name) + "|");

          info.Character.PrintMessage("[Print...]");
        }
        else
          World.Player.PrintMessage("Neni klamak", Game.Val_LightPurple);
      }
    }

    //---------------------------------------------------------------------------------------------

    public Taming2()
    {
      this.doneList = new List<Serial>();
    }

    public static Graphic[] ShrinkKlamaci
    {
      get
      {
        Graphic[] lootItem = new Graphic[38];
        lootItem[0] = 0x2121;
        lootItem[1] = 0x2120;
        lootItem[2] = 0x211F;
        lootItem[3] = 0x2124;
        lootItem[4] = 0x20F6;
        lootItem[5] = 0x2137;
        lootItem[6] = 0x2126;
        lootItem[7] = 0x2127;
        lootItem[8] = 0x2135;
        lootItem[9] = 0x20E1; // polar bear
        lootItem[10] = 0x20F7; // panther, walrus
        lootItem[11] = 0x2119; // leopard/snow leopard1
        lootItem[12] = 0x2102; // leopard/snow leopard2
        lootItem[13] = 0x211D; // eagle
        lootItem[14] = 0x2136; // zostrich
        lootItem[15] = 0x2123; //  Rat
        lootItem[16] = 0x20D1; // Chicken
        lootItem[17] = 0x211B; // Cat
        lootItem[18] = 0x2D97; // Veverka
        lootItem[19] = 0x20EE; // Papousek
        lootItem[20] = 0x20D4; // Hind
        lootItem[21] = 0x2130; // Frog
        lootItem[22] = 0x20EA; // Timber wolf
        lootItem[23] = 0x20FC; // Snake
        lootItem[24] = 0x20CF; // Brown bear
        lootItem[25] = 0x2118; // Black bear
        lootItem[26] = 0x2108; // Goat
        lootItem[27] = 0x2101; // Pig
        lootItem[28] = 0x211C; // Dog
        lootItem[29] = 0x20D0; // Giant Rat
        lootItem[30] = 0x20F5; // Gorila
        lootItem[31] = 0x211E; // Grizzli bear
        lootItem[32] = 0x20E6; // Lamb
        lootItem[33] = 0x20EF; // Bull
        lootItem[34] = 0x2103; // Cow
        lootItem[35] = 0x20E2; // Jackrabit
        lootItem[36] = 0x20EB; // Sheep
        lootItem[37] = 0x2131; // Aligator
        return lootItem;
      }
    }

    public static List<Graphic> TrofejKlamaci
    {
      get
      {
        List<Graphic> list = new List<Graphic>();
        list.Add(0x00EE);//rat
        list.Add(0x00ED);//hind
        list.Add(0x00D1);//goat
        list.Add(0x00D3);//"Brown Bear
        list.Add(0x00E1);//Wolf
        list.Add(0x0034);//snake
        list.Add(0x00D4);//bear
        list.Add(0x00CD);//rabid
        list.Add(0x00CA);// Aligator
        list.Add(0x001D);// todo gorila
        return list;
      }
    }

    //0x0006  Ptacek


    //    Serial: 0x00372C3D  Name: "Brown Bear"  Position: 3254.1948.23  Flags: 0x0040  Color: 0x01BB  Model: 0x00D3  Renamable: False Notoriety: Neutral HP: 100/100

    //Serial: 0x00372C3D  Name: "Brown Bear"  Position: 3254.1948.23  Flags: 0x0040  Color: 0x01BB  Model: 0x00D3  Renamable: False Notoriety: Neutral HP: 100/100


    //Serial: 0x0035DCA1  Name: "Goat"  Position: 1206.1594.0  Flags: 0x0000  Color: 0x0000  Model: 0x00D1  Renamable: False Notoriety: Neutral HP: 50/50

    public UOItem EnsuredTamingStaff
    {
    //public UOItemType TrainTamingStaff { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x04B9 }; } }
    //public UOItemType TamingStaff { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x076B }; } }
    //public UOItemType TamingStaffCharged { get { return new UOItemType() { Graphic = 0x13F4, Color = 0x096D }; } }

    get
      {
        Game.PrintMessage("EnsuredTamingStaff");
        UOItem tamingStaff = UO.Backpack.AllItems.FindType(0x13F4, 0x096D); //*/TamingStaffCharged.FindItem(Precision.GraphicColor, Search.Both);
        if (!tamingStaff.Exist)
        {
          tamingStaff = World.Player.Layers.FindType(0x13F4, 0x096D);

          if (!tamingStaff.Exist)
          {
            tamingStaff = UO.Backpack.AllItems.FindType(0x13F4, 0x076B);  //TamingStaff.FindItem(Precision.GraphicColor, Search.Both);

            if (!tamingStaff.Exist)
              tamingStaff = World.Player.Layers.FindType(0x13F4, 0x076B);

            if (tamingStaff.Exist)
            {
              UO.WaitTargetType(0x1843, 0x0724);
              tamingStaff.Use();
              Game.Wait();
            }
            else
            {
              tamingStaff = UO.Backpack.AllItems.FindType(0x13F4, 0x04B9);//Train
              if (!tamingStaff.Exist)
              {
                tamingStaff = World.Player.Layers.FindType(0x13F4, 0x04B9);//Train
              }
              else
              {
                Game.PrintMessage("EnsuredTrainTamingStaff! OK - 3");
              }

              if (tamingStaff.Exist)
                tamingStaff.Use();

              Game.PrintMessage("EnsuredTamingStaff! OK - 2");
            }
          }
          else
            Game.PrintMessage("EnsuredTamingStaff! OK - 1");
        }
        else
          Game.PrintMessage("EnsuredTamingStaff! OK");
        return tamingStaff;
      }
    }

    public List<Serial> doneList;
    public void TrainManual()
    {
      this.doneList = new List<Serial>();

      Game.PrintMessage("Vyber  >");
      UOCharacter character = new UOCharacter(UIManager.TargetObject());
      Game.Wait();
      character.Click();

      Game.PrintMessage("" + character.Name);

      if (character.Exist)
      {
        UOItem equipedKrk = World.Player.Layers[Layer.Neck];
        UOItem tamingNeklak = World.Player.Backpack.AllItems.FindType(0x1088, 0x0B18);
        if (tamingNeklak.Exist)
        {
          tamingNeklak.Use();
          Game.Wait();
        }

        this.TameCharacter(character, 100, false);

        if (equipedKrk.Exist)
          equipedKrk.Use();

      }
    }

    //---------------------------------------------------------------------------------------------

      [Executable]
    public void TrainOtherSkills()
    {

      Game.CurrentGame.CurrentPlayer.SwitchWarmode();
      while (!UO.Dead)
      {
        if (UO.Backpack.AllItems.FindType(0x0E9D, 0x0000).Exist)
        {

          Game.PrintMessage("Startuji Musican Train");

          Journal.Clear();
          UO.UseType(0x0E9D, 0x0000);
          if (Journal.WaitForText(true, 2500, "You play poorly")) { }
          //Game.Wait(1000);

          if (Journal.Contains(true, " You are preoccupied with thoughts of battle"))
          {
            Game.CurrentGame.CurrentPlayer.SwitchWarmode();
            Game.Wait();
          }
          //Game.Wait(1500);


        }
        else if (SkillsHelper.GetSkillValue("ArmsLore").RealValue < 900 && World.Player.Backpack.AllItems.FindType(0x1400).Exist)
        {
          UO.WaitTargetObject(World.Player.Backpack.AllItems.FindType(0x1400));
          UO.UseSkill(StandardSkill.ArmsLore);
          Game.Wait(1350);
        }
      }
    }

    //---------------------------------------------------------------------------------------------
    public void TrainTamingRecusive(int maxTries, params string[] positionsDefinition)
    {
      TrainTamingRecusive(maxTries, 6, positionsDefinition);
    }


    public void TrainTamingRecusive(int maxTries, int maxCharDistance, params string[] positionsDefinition)
    {
      Robot r = new Robot();
      r.UseTryGoOnly = true;
      r.UseMinWait = true;
      r.UseRun = true;
      r.SearchSuqareSize = 450;

      this.doneList = new List<Serial>();

      string[] locations = positionsDefinition;

      Game.PrintMessage("TrainTamingRecusive START " + maxTries + ", " + maxCharDistance + ", " + positionsDefinition.Length);
      Game.Wait(1000);

      foreach (string loc in locations)
      {
        string[] options = loc.Split('|');

        int button = -1;
        string bookType = "r";

        if (!String.IsNullOrEmpty(options[0]) && Regex.IsMatch(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})"))
        {
          Match m = Regex.Match(options[0], "(?<booktype>[a-z])(?<button>\\d{1,2})");
          bookType = m.Groups["booktype"].Value.ToLower();
          button = Int32.Parse(m.Groups["button"].Value);
        }

        if (button > -1)
        {
          string book = "RuneBookUse";
          if (bookType == "t")
          {
            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
            UO.Wait(1000);
            book = "TravelBookUse";
          }
          else if (bookType == "c")
          {
            Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["CestovniKnihaUse"], 1);
            UO.Wait(1000);
            book = "CestovniKnihaUse";
          }



          bool teleported = false;
          while (!teleported)
          {
            UO.DeleteJournal();

            Phoenix.Runtime.RuntimeCore.Executions.Execute(RuntimeCore.ExecutableList[book], button);
            Game.Wait(500);
            if (!World.Player.Hidden)
              UO.UseSkill("Hiding");

            UO.Print("Cekam na kop.. nehybat");

            if (Journal.WaitForText(true, 2000, "Nesmis vykonavat zadnou akci"))
            {
              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
              Game.Wait(1000);
            }
            else if (Journal.WaitForText(true, 120000, "You have been teleported"))
              teleported = true;

            if (Game.CurrentGame.WorldSave())
            {
              UO.Print("WS opakovani kopu za 45s");
              Game.Wait(45000);
              if (bookType == "t")
              {
                Phoenix.Runtime.RuntimeCore.Executions.Execute(Phoenix.Runtime.RuntimeCore.ExecutableList["TravelBookUse"], 1);
                UO.Wait(1000);
              }
              Game.Wait(500);
            }
          }
        }

        for (int i = (button > -1 ? 1 : 0); i < options.Length; i++)
        {
          if (UO.Dead)
            return;

          string[] parm = options[i].Split('.');

          string x = parm[0];
          string[] y = parm[1].Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
          string placeName = "";
          if (y.Length > 1)
            placeName = y[1];


          UOPositionBase pos = new UOPositionBase(ushort.Parse(x), ushort.Parse(y[0]), (ushort)0);

          int distance = parm.Length > 2 ? UOExtensions.Utils.ToNullInt(parm[2]).GetValueOrDefault(1) : 1;
          int gotries = parm.Length > 3 ? UOExtensions.Utils.ToNullInt(parm[3]).GetValueOrDefault(1000) : 1000;

          Game.PrintMessage("GoTo: " + pos);
          if (r.GoTo(pos, distance, gotries))
          {
            Game.PrintMessage("In position: " + pos);

            if (parm[parm.Length - 1].ToLower() == "opendoor")
            {
              ItemHelper.OpenDoorAll();
              Game.Wait();


              if (World.Player.Layers[Layer.OuterTorso].Exist)
              {
                World.Player.Layers[Layer.OuterTorso].Move(1, World.Player.Backpack);
                Game.Wait();
              }
            }

            List<UOCharacter> characters = new List<UOCharacter>();
            characters.AddRange(World.Characters.Where(c => !doneList.Contains(c.Serial)));
            characters.Sort(delegate (UOCharacter char1, UOCharacter char2)
            {
              return char1.Distance.CompareTo(char2.Distance);
            });

            foreach (UOCharacter character in characters)
            {
              if (UO.Dead)
                return;

              Game.Wait(50);

              if (character.Distance < maxCharDistance && character.Serial != World.Player.Serial && character.Model != 0x0190 && character.Model != 0x0191 && character.Model != 0x0192)
              {
                doneList.Add(character);

                SkillValue atSkill = SkillsHelper.GetSkillValue("Animal Taming");
                bool isBird = character.Model == 0x0006;
                bool tryKill = atSkill.RealValue > 450 && maxTries < 100;
                bool tame = true;

                if (isBird)
                {
                  if (tryKill)
                  {
                    World.Player.PrintMessage("Try Kill.");
                    UO.Cast(StandardSpell.Harm, character.Serial);
                    Game.Wait(3000);

                    if (character.Exist)
                    {
                      UO.Cast(StandardSpell.Harm, character.Serial);
                      Game.Wait(2500);
                    }
                  }
                  tame = atSkill.RealValue < 450;
                  //World.Player.PrintMessage("Try Kill result: " + KillCharacter(character));
                }

                if (Game.Debug) 
                  Game.PrintMessage("Bird: "+ isBird + ", Tame: " + tame + ", Kill: " + tryKill + ", Skill: " + atSkill.Value);

                if (tame && r.GoTo(character.GetPosition(), 2, 25))
                  this.TameCharacter(character, maxTries);
              }

            }
          }
        }

        Game.PrintMessage("NextLocation");
        Game.Wait(1000);
      }

      if (UO.Dead)
        return;

      TrainTamingRecusive(maxTries, maxCharDistance, positionsDefinition);
    }

    //---------------------------------------------------------------------------------------------

    public void TameCharacter(UOCharacter character, int maxTries)
    {
      TameCharacter(character, maxTries, true);
    }

    public void TameCharacter(UOCharacter character, int maxTries, bool keepDisttance)//, Robot r)
    {
      Game.CurrentGame.CurrentPlayer.SwitchWarmode();

      Robot r = new Robot();
      r.UseTryGoOnly = true;
      r.SearchSuqareSize = 20;

      UOItem currentStaff = EnsuredTamingStaff;
      int tries = 0;

      if (Game.Debug)
        Game.PrintMessage(String.Format("{0}, {1}, {2}, {3}, {4}", character.Exist, character.RequestStatus(500), character.Hits, character.Distance, UO.Dead));

      while (character.Exist && character.RequestStatus(500) && character.Hits > 0 && character.Distance < 6 && !UO.Dead)
      {
        UO.DeleteJournal();

        bool end = false;
        bool kill = false;

        if (character.Distance > 3 && keepDisttance)
        {
          r.GoTo(new UOPositionBase(character.X, character.Y, 0), 2, 2);
          Game.Wait(500);
        }

        UO.UseSkill("Hiding");
        Game.Wait(250);

        if (World.Player.Hidden)
        {
          UO.Say(" ");
          Game.Wait(250);
        }

        IRequestResult result = UO.WaitTargetObject(character.Serial);
        currentStaff.Use();

        //Game.Wait(500);
        SkillValue tamingValue = SkillsHelper.GetSkillValue("AnimalTaming");
        UOItem robe = World.Player.Layers[Layer.OuterTorso];
        Game.PrintMessage("RV: " + tamingValue.RealValue + " / " + robe.Exist);

        using (JournalEventWaiter jew = new JournalEventWaiter(true,
          "You can't see the target",
          "Ochoceni se nezdarilo",
          "Your taming failed",
          "byl tamnut",
          "not tamable",
          "You are not able to tame",
          "Jsi moc daleko",
          "Jeste nemuzes pouzit hulku",
          "Toto zvire nelze ochocit",
          "Toto zvire nedokazes ochocit",
          "You are not able to tame this animal"

          ))
        {

          if (jew.Wait(15000))
          {
            if (Journal.Contains(true, "Ochoceni se nezdarilo"))
            {
              Game.PrintMessage("Try - Ochoceni se nezdarilo");
            }
            else if (Journal.Contains(true, "Your taming failed"))
            {
              Game.PrintMessage("Try - Your taming failed");
            }
            else if (Journal.Contains(true, "Jeste nemuzes pouzit hulku"))
            {
              Game.PrintMessage("Try - Jeste nemuzes pouzit hulku");
              Game.Wait(6000);
            }
            else if (Journal.Contains(true, "Jsi moc daleko"))
            {
              if (keepDisttance)
              {
                bool go = r.GoTo(new UOPositionBase(character.X, character.Y, 0), 1, 10);
                if (!go)
                {
                  end = true;
                }
                Game.Wait();
                Game.PrintMessage("Try - Jsi moc daleko: " + go);
              }
            }
            else if (Journal.Contains(true, "You can't see the target"))
            {
              UO.WaitTargetCancel();
              Game.Wait();
              UO.Press(System.Windows.Forms.Keys.Escape);
              Game.Wait();
              Game.CurrentGame.CurrentPlayer.SwitchWarmode();
              Game.Wait();
              bool go = r.GoTo(new UOPositionBase(character.X, character.Y, 0), 1, 10);
              if (!go)
              {
                end = true;
              }
              Game.Wait();

              Game.PrintMessage("Try - You can't see the target go: " + go);
            }
            else if (Journal.Contains(true, "byl tamnut"))// && currentStaff.Color == TamingStaff.Color)
            {
              Game.PrintMessage("End - byl tamnut");
              end = true;
            }
            else if (Journal.Contains(true, "not tamable") || Journal.Contains(true, "Toto zvire nelze ochocit"))
            {

              character.Click();
              Game.Wait(500);
              character.RequestStatus(500);

              bool isTrofejAnimal = false;// TrofejKlamaci.Contains(character.Model);

              foreach (Graphic g in TrofejKlamaci)
              {
                if (g == character.Model)
                  isTrofejAnimal = true;
              }


              if (isTrofejAnimal && character.Notoriety != Notoriety.Innocent && character.Notoriety != Notoriety.Invulnerable && character.Notoriety != Notoriety.Guild)//krysa/hind/bear/goat/snake atd)TODO
              {
                kill = true;
                //zabit / riznout / vylotit
                Game.PrintMessage("Kill - not tamable " + character.Model);
              }
              else
              {
                Game.PrintMessage("End - not tamable");
                end = true;
              }
            }
            else if (Journal.Contains(true, "Toto zvire nedokazes ochocit") || Journal.Contains(true, "You are not able to tame this animal"))
            {

              if (World.Player.Backpack.AllItems.FindType(0x1F03).Exist)
              {
                World.Player.Backpack.AllItems.FindType(0x1F03).Use();
                Game.Wait(500);
              }
              else if (World.Player.Backpack.AllItems.FindType(0x1F01).Exist)
              {
                World.Player.Backpack.AllItems.FindType(0x1F01).Use();
                Game.Wait(500);
              }

              if (tries > 1)
              {
                Game.PrintMessage("End - Nelze ochocit " + character.Serial);
                end = true;
              }

              Game.Wait(3500);
            }
            else
            {
              Game.Wait(1000);
            }
          }
          else
          {
            if (Game.CurrentGame.WorldSave())
            {
              Game.Wait(60000);
              Game.PrintMessage("Try - WordSave" + character.Serial);
            }
            else
            {
              if (tries > 1)
              {
                Game.PrintMessage("End - JEW timeout " + character.Serial);
                end = true;
              }
            }
          }

          if (tries > maxTries)// && currentStaff.Color == TrainTamingStaff.Color)
            end = true;

          if (tamingValue.RealValue < 1000 && maxTries < 100)
          {
            bool switchToTrain = false;
            bool switchToTaming = false;
            if (tries == 0)
            {
              if ((Journal.Contains(true, "nenauc") || Journal.Contains(true, "You Cannot learn anything more")) && robe.Exist)
              {
                robe.Move(1, World.Player.Backpack);
                Game.Wait();
                switchToTrain = true;
              }
              
              if (!Journal.Contains(true, "nenauc"))
                switchToTrain = true;
            }
            else if (tries == 1)
            {
              if (Journal.Contains(true, "nenauc") || Journal.Contains(true, "You Cannot learn anything more"))
              {
                switchToTaming = true;

                Game.PrintMessage("robe.Exist:" + robe.Exist);

                if (robe.Exist)
                {
                  robe.Use();
                  Game.Wait();
                }
              }
            }

            if (switchToTrain)
            {
              if (World.Player.Backpack.AllItems.FindType(TrainTamingStaff.Graphic, TrainTamingStaff.Color).Exist)
                currentStaff = World.Player.Backpack.AllItems.FindType(TrainTamingStaff.Graphic, TrainTamingStaff.Color);
              else if (World.Player.Layers.FindType(TrainTamingStaff.Graphic, TrainTamingStaff.Color).Exist)
                currentStaff = World.Player.Backpack.AllItems.FindType(TrainTamingStaff.Graphic, TrainTamingStaff.Color);

              Game.Wait();
            }


            if (switchToTaming)
            {
              currentStaff = EnsuredTamingStaff;
              Game.Wait();
            }

          }
          if (tries > 2)
          {
            if (Journal.Contains(true, "nenauc") || currentStaff.Color == TamingStaffCharged.Color && !World.Player.Layers[Layer.OuterTorso].Exist)
            {
              if (World.Player.Backpack.AllItems.FindType(0x1F03).Exist)
              {
                World.Player.Backpack.AllItems.FindType(0x1F03).Use();
                Game.Wait();
              }
              else if (World.Player.Backpack.AllItems.FindType(0x1F01).Exist)
              {
                World.Player.Backpack.AllItems.FindType(0x1F01).Use();
                Game.Wait(500);
              }
            }
          }

          if (kill)
          {
            if (!this.KillCharacter(character))
              end = true;
          }

          if (end)
          {
            Game.PrintMessage("While - end: " + tries);
         //   doneList.Remove(character.Serial);
            break;
          }
        }
        tries++;
        Game.PrintMessage("While - cyklus pokus: " + tries);
      }

      foreach (UOItem item in World.Ground)
      {

        if (item.Distance < 5)
        {
          bool isKlamak = false;
          foreach (Graphic g in Taming2.ShrinkKlamaci)
          {
            if (item.Graphic == g)
              isKlamak = true;
          }

          if (Array.IndexOf(ItemLibrary.ShrinkKlamaci.GraphicArray, item.Graphic) > -1 || Array.IndexOf(ShrinkKlamaci, item.Graphic) > -1 || isKlamak)
          {
            item.Move(1, (ushort)(World.Player.X + 1), (ushort)(World.Player.Y + 1), item.Z);
            Game.Wait(150);
            item.Move(1, World.Player.Backpack, 30, 30);
            Game.Wait(150);
          }
        }
      }

      Game.PrintMessage("While - END");
      if (World.Player.Backpack.AllItems.FindType(0x1F03).Exist)
      {
        World.Player.Backpack.AllItems.FindType(0x1F03).Use();
        Game.Wait(250);
      }
      else if (World.Player.Backpack.AllItems.FindType(0x1F01).Exist)
      {
        World.Player.Backpack.AllItems.FindType(0x1F01).Use();
        Game.Wait(250);
      }
    }

    //---------------------------------------------------------------------------------------------

    protected bool KillCharacter(UOCharacter character)
    {
      UOItem wepn = new UOItem(Serial.Invalid);

      foreach (UOItemType t in ItemLibrary.WeaponsFenc)
      {
        wepn = UO.Backpack.AllItems.FindType(t.Graphic);
        if (wepn.Exist)
          break;
        else
        {
          wepn = World.Player.Layers[Layer.LeftHand].Graphic == t.Graphic ? World.Player.Layers[Layer.LeftHand] : wepn;//.FindType(t.Graphic);
          if (wepn.Exist)
            break;
          else
          {
            wepn = World.Player.Layers[Layer.RightHand].Graphic == t.Graphic ? World.Player.Layers[Layer.RightHand] : wepn;
            if (wepn.Exist)
              break;
          }
        }
      }
      bool isArch = false;
      if (!wepn.Exist)
      {
        foreach (UOItemType t in ItemLibrary.WeaponsArch)
        {
          wepn = UO.Backpack.AllItems.FindType(t.Graphic);
          if (wepn.Exist)
          {
            isArch = true;
            break;
          }
          else
          {
            wepn = World.Player.Layers[Layer.LeftHand].Graphic == t.Graphic ? World.Player.Layers[Layer.LeftHand] : wepn;//.FindType(t.Graphic);
            if (wepn.Exist)
              break;
            else
            {
              wepn = World.Player.Layers[Layer.RightHand].Graphic == t.Graphic ? World.Player.Layers[Layer.RightHand] : wepn;
              if (wepn.Exist)
                break;
            }
          }
        }
      }
      
      if (wepn.Exist)
      {
        UO.WaitTargetObject(character);
        if (wepn.Layer != Layer.RightHand || wepn.Layer != Layer.LeftHand)
        {
          wepn.Use();
          Game.Wait();
        }
        int killTries = 0;
        while (character.Exist && character.Distance < 8 && character.Hits > -1 && killTries < 50)
        {
          Game.PrintMessage("Kill " + character.Name + " Trie:" + killTries);

          if (isArch)
          {
            Ranger.EnsureArcherAmmo();
            Game.Wait(Game.SmallestWait);
          }

          World.Player.ChangeWarmode(WarmodeChange.War);
          Game.Wait();


          UO.Attack(character);
          Game.Wait();
          new Robot().GoTo(character.X, character.Y, 1, 4);
          character.Print("{0}/{1}", character.Hits, character.MaxHits);
          killTries++;

          Game.Wait(5000);
        }

        if (!character.Exist)
        {
          Game.Wait(500);
          new Loot().LootGround(Loot.LootType.Quick, true);

          return true;
        }
      }
      else
        Game.PrintMessage("Nemas u sebe Kryss");

      return false;
    }

    //---------------------------------------------------------------------------------------------

    #region exec

    //---------------------------------------------------------------------------------------------

    [Executable("TrainTamingManual2")]
    [BlockMultipleExecutions]
    public static void ExecTrainManual()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming2>().TrainManual();
    }

    //---------------------------------------------------------------------------------------------

    [Executable("TrainTamingAuto2")]
    [BlockMultipleExecutions]
    public static void ExecTrainTamingAuto(params string[] str)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming2>().TrainTamingRecusive(25, str);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("TrainTamingAuto2")]
    [BlockMultipleExecutions]
    public static void ExecTrainTamingAuto(int maxTries, params string[] str)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming2>().TrainTamingRecusive(maxTries, str);
    }

    //---------------------------------------------------------------------------------------------

    [Executable("TrainTamingAuto2")]
    [BlockMultipleExecutions]
    public static void ExecTrainTamingAuto(int maxTries, int perimeter ,params string[] str)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming2>().TrainTamingRecusive(maxTries, perimeter, str);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public static void TameSpot(int perimeter)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming2>().TrainTamingRecusive(100, perimeter, World.Player.X + "." + World.Player.Y + "|" + World.Player.X + "." + World.Player.Y);
    }

    [Executable]
    [BlockMultipleExecutions]
    public static void TameSpot(int perimeter, int maxTries)
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Taming2>().TrainTamingRecusive(maxTries, perimeter, World.Player.X + "." + World.Player.Y + "|" + World.Player.X + "." + World.Player.Y);
    }



    #endregion
  }
}
