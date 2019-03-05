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
using CalExtension.UOExtensions;

namespace CalExtension.Skills
{
  public class Fishing : Skill
  {
    public static UOItemType FishingPole { get { return new UOItemType() { Graphic = 0x0DBF, Color = 0x0000, Name = "fishing pole" }; } }
    public static UOItemType Vlasec { get { return new UOItemType() { Graphic = 0x0FA0, Color = 0x02B3, Name = "vlasec" }; } }
    //Color: 0x02B3  Graphic: 0x0FA0
    //---------------------------------------------------------------------------------------------

    private bool FishTile(int x, int y, sbyte z, out bool enemyDetected, out bool worthItem)
    {
      enemyDetected = false;
      worthItem = false;
      bool result = true;
      JournalEventWaiter jew = new JournalEventWaiter(true, "Lovit lze jen v hluboke vode", "There are no fish here", "That is too far away", "You pull out a nice fish", "Podarilo se ti chytit", "Try fishing in water", "You fish a while");
      Game.PrintMessage(String.Format("Chytam na {0}, {1}", x, y));
      UO.WaitTargetTileRel(x, y, z, 0);
      UO.UseType(FishingPole.Graphic);
      jew.Wait(3000);
      Game.Wait(50);

      if (Journal.Contains(true, "Lovit lze jen v hluboke vode", "There are no fish here", "That is too far away", "Try fishing in water"))
        result =  false;
      
      if (Journal.Contains(true, "Kraken", "Sea Serpent"))
        enemyDetected = true;
      if (Journal.Contains(true, "Podarilo se ti chytit"))
        worthItem = true;

      Journal.Clear();
      return result;
    }

    //---------------------------------------------------------------------------------------------

    private bool PoleExist()
    {
      bool result = UO.Backpack.Items.FindType(FishingPole.Graphic).Exist || World.Player.Layers.FindType(FishingPole.Graphic).Exist;
      if (!result)
        Game.PrintMessage("Neni prut");

      return result;
    }

    //---------------------------------------------------------------------------------------------

    int moveTries = 0;

    //---------------------------------------------------------------------------------------------

    private bool TryMoveNextPlace()
    {
      if (moveTries > 2)
      {
        moveTries = 0;
        Game.PrintMessage("moveTries:" + moveTries );
        return false;
      }

      UOPositionBase startPosition = new UOPositionBase(World.Player.X, World.Player.Y, 0);
      UOPositionBase currentPosition = new UOPositionBase(World.Player.X, World.Player.Y, 0);

      if (moveTries > 0)
      {
        Game.Wait();
        UO.Say("Turn left");
      }
      UO.Say("Forward");
      UO.Say("Forward");

      //
      int currentNotChangeCount = 0;

      while(Robot.GetRelativeVectorLength(startPosition, currentPosition) < 12)
      {
        Game.Wait(500);
        Game.PrintMessage("Distance:" + Robot.GetRelativeVectorLength(startPosition,currentPosition));
        if (currentPosition.X == World.Player.X && currentPosition.Y == World.Player.Y)
          currentNotChangeCount++;

        if (currentNotChangeCount > 10)
        {
          moveTries++;
          return TryMoveNextPlace();
        }
        currentPosition = new UOPositionBase(World.Player.X, World.Player.Y, 0);
      }

      UO.Say("Stop");
      moveTries = 0;
      return true;
    }


//    Serial: 0x400000AF  Position: 133.103.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x097F  Amount: 1  Layer: None Container: 0x40276E6D

//Serial: 0x401BD51D  Name: "draw knife"  Position: 87.92.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x10E4  Amount: 1  Layer: None Container: 0x40276E6D

//Serial: 0x40359906  Name: "raw fish steaks"  Position: 137.128.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x097A  Amount: 915  Layer: None Container: 0x40276E6D

//Serial: 0x400000AF  Position: 133.103.0  Flags: 0x0000  Color: 0x0000  Graphic: 0x097F  Amount: 1  Layer: None Container: 0x40276E6D

//Serial: 0x40001905  Position: 3352.344.10  Flags: 0x0020  Color: 0x0000  Graphic: 0x097B  Amount: 3  Layer: None Container: 0x00000000


    //---------------------------------------------------------------------------------------------

    public void StartFishing()//Zatim jen zacatek uvidime jak to bude fachat
    {
      Journal.Clear();
      Game.CurrentGame.Mode = GameMode.Working;

      UOItem panvicka = World.Player.Backpack.Items.FindType(ItemLibrary.Panvicka.Graphic);
      UOItem dwarfKnife = World.Player.Backpack.Items.FindType(ItemLibrary.DwarfKnife.Graphic, 0x0000);

      Game.PrintMessage("Panvicka: " + panvicka.Exist + " DwarfKnife: " + dwarfKnife.Exist);

      while (!World.Player.Dead)
      {
        if (PoleExist())
        {
          bool end = false;

          for (int x = -6; x < 7; x++)
          {
            List<UOItem> ground = new List<UOItem>();
            ground.AddRange(World.Ground.ToArray());

            foreach (UOItem groundItem in ground)
            {
              bool grab = true;

              if (groundItem.Graphic == 0x097B)//upeceny steak
                grab = false;

              if (!panvicka.Exist || !dwarfKnife.Exist)
              {
                foreach (UOItemType fish in ItemLibrary.Fish)
                {
                  if (groundItem.Graphic == fish.Graphic && groundItem.Color == fish.Color)
                    grab = false;
                }
              }

              if (groundItem.Distance < 3 && grab)//groundItem.Graphic != 0x097B)//upeceny steak
              {
                groundItem.Move(60000, World.Player.Backpack);
                Game.Wait();
              }
            }


            if (panvicka.Exist && dwarfKnife.Exist)
            {
              List<UOItem> backpack = new List<UOItem>();
              backpack.AddRange(World.Player.Backpack.Items.ToArray());

              foreach (UOItem item in backpack)
              {
                bool cut = false;

                foreach (UOItemType fish in ItemLibrary.Fish)
                {
                  if (item.Graphic == fish.Graphic && item.Color == fish.Color)
                    cut = true;
                }

                if (cut)
                {
                  UO.WaitTargetObject(item.Serial);
                  dwarfKnife.Use();
                  Game.Wait();
                }

              }

              int sychr = 0;
              while (panvicka.Exist && World.Player.Backpack.Items.FindType(ItemLibrary.RawFishSteak.Graphic, ItemLibrary.RawFishSteak.Color).Exist && sychr < 1500)
              {
                Journal.Clear();
                UOItem rawSteak = World.Player.Backpack.Items.FindType(ItemLibrary.RawFishSteak.Graphic, ItemLibrary.RawFishSteak.Color);

                UO.WaitTargetObject(rawSteak.Serial);
                panvicka.Use();

                if (!Journal.WaitForText(true, 5000 + Core.CurrentLatency, "...akce skoncila"))
                {
                  //end?
                }
                Game.Wait(50);

                sychr++;
              }


              if (World.Player.Backpack.Items.FindType(0x097B, ItemLibrary.RawFishSteak.Color).Exist)
              {
                World.Player.Backpack.Items.FindType(0x097B, ItemLibrary.RawFishSteak.Color).DropHere(60000);
                Game.Wait();
              }

              Game.Wait();
            }

            for (int y = -6; y < 7; y++)
            {
              bool enemyDetect;
              bool worthItem;
              while (FishTile(x, y, World.Player.Z, out enemyDetect, out worthItem))
              {
                if (enemyDetect)
                {
                  Game.PrintMessage("Monstrum aaaaaa...");

                  while (World.Player.Hidden || !World.Player.Dead)
                  {
                    Mount.Current.UseMount();
                    Game.Wait();
                    Hiding.ExecHide();
                    Game.Wait(3000);
                  }

                  if (World.Player.Dead)
                  {
                    end = true;
                    break;
                  }
                  else
                  {
                    Mount.Current.UseMount();

                    while (Characters.ExistEnemy())
                    {
                      Game.Wait(1000);
                      Game.PrintMessage("Monstrum bojim bojim...");
                    }
                  }
                  //TODO HID pres lamu dokud nezmizi
                }

                if (!PoleExist())
                {
                  end = true;
                  break;
                }
              }
              Game.Wait();
            }



            if (end)
              break;
          }

          if (!end)
          {
            if (this.TryMoveNextPlace())
            {
              this.StartFishing();
              return;
            }
            else
              break;
          }
          else
            break;

        }
        else
          break;
      }

      Game.PrintMessage("Rybareni dokonceno");
    }

    #region exec

    [Executable("StartFishing")]
    [BlockMultipleExecutions]
    public static void ExecStartFishing()
    {
      Game.CurrentGame.CurrentPlayer.GetSkillInstance<Fishing>().StartFishing();
    }


    #endregion
  }
}
