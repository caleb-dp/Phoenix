using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;

namespace Phoenix.Scripts
{
  public class BackpackArranger2
  {
    [Command("arrange2")]
    public void arrangingall(int pause)
    {
      arranging(pause);
      arrangingregy(pause);
    }

    [Command("posunPlusPlus2")]
    public void posunto1(ushort X, ushort Y)
    {
      UO.Print("Zamer item k posunuti.");
      UOItem item = World.GetItem(UIManager.TargetObject());
      ushort posunX = item.X;
      ushort posunY = item.Y;
      posunX += X;
      posunY += Y;
      UO.MoveItem(item.Serial, item.Amount, item.Container, posunX, posunY);
      UO.Print(item.Name + " posunut o X:" + X + " Y: " + Y);
    }

    [Command("posunPlusMinus2")]
    public void posunto2(ushort X, ushort Y)
    {
      UO.Print("Zamer item k posunuti.");
      UOItem item = World.GetItem(UIManager.TargetObject());
      ushort posunX = item.X;
      ushort posunY = item.Y;
      posunX += X;
      posunY -= Y;
      UO.MoveItem(item.Serial, item.Amount, item.Container, posunX, posunY);
      UO.Print(item.Name + " posunut o X:" + X + " Y: " + Y);
    }

    [Command("posunMinusPlus2")]
    public void posunto3(ushort X, ushort Y)
    {
      UO.Print("Zamer item k posunuti.");
      UOItem item = World.GetItem(UIManager.TargetObject());
      ushort posunX = item.X;
      ushort posunY = item.Y;
      posunX -= X;
      posunY += Y;
      UO.MoveItem(item.Serial, item.Amount, item.Container, posunX, posunY);
      UO.Print(item.Name + " posunut o X:" + X + " Y: " + Y);
    }

    [Command("posunMinusMinus2")]
    public void posunto4(ushort X, ushort Y)
    {
      UO.Print("Zamer item k posunuti.");
      UOItem item = World.GetItem(UIManager.TargetObject());
      ushort posunX = item.X;
      ushort posunY = item.Y;
      posunX -= X;
      posunY -= Y;
      UO.MoveItem(item.Serial, item.Amount, item.Container, posunX, posunY);
      UO.Print(item.Name + " posunut o X:" + X + " Y: " + Y);
    }

    [Command("bottlearrange2")]
    public void arranging(int pause)
    {
      ushort potionGraphic1 = 0x0F09;
      ushort potionColor1 = 0x0003;
      ushort potionGraphic2 = 0x0F09;
      ushort potionColor2 = 0x0000;
      ushort potionGraphic3 = 0x0F0C;
      ushort potionColor3 = 0x0000;
      ushort potionGraphic4 = 0x0F0B;
      ushort potionColor4 = 0x0000;
      ushort potionGraphic5 = 0x0F07;
      ushort potionColor5 = 0x0000;
      ushort potionGraphic6 = 0x0F09;
      ushort potionColor6 = 0x0005;
      ushort potionGraphic7 = 0x0F0C;
      ushort potionColor7 = 0x0025;
      ushort potionGraphic8 = 0x0F01; //Essence of Jabara
      ushort potionColor8 = 0x005B;
      ushort potionGraphic9 = 0x0F02; //Essence of Cinchona
      ushort potionColor9 = 0x0835;
      ushort potionGraphic10 = 0x0EFE; //Essence of Refresh
      ushort potionColor10 = 0x005B;
      ushort potionGraphic11 = 0x0F0D; //Lava Bomb
      ushort potionColor11 = 0x000E;

      ushort vychoziX = 20;
      ushort maxX = 160;
      ushort X = 20;
      ushort Y = 160;
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic1 && item.Color == potionColor1)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5; //5
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic2 && item.Color == potionColor2)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5; //5
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic3 && item.Color == potionColor3)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5; //5
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic4 && item.Color == potionColor4)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5; //5
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic5 && item.Color == potionColor5)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }

      X += 5;
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic6 && item.Color == potionColor6)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic7 && item.Color == potionColor7)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic8 && item.Color == potionColor8)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic9 && item.Color == potionColor9)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      X += 10;
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic10 && item.Color == potionColor10)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      foreach (UOItem item in World.Player.Backpack.AllItems)
      {
        if (item.Graphic == potionGraphic11 && item.Color == potionColor11)
        {
          UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
          X += 5;
          if (X >= maxX)
          {
            Y += 15;
            X = vychoziX;
          }
          UO.Wait(pause);
        }
      }
      //foreach (UOItem item in World.Player.Backpack.AllItems)
      //{
      //if (item.Graphic == potionGraphic12 && item.Color == potionColor12)
      //{
      //UO.MoveItem(item.Serial, 1, Aliases.Backpack, X, Y);
      //X += 5;
      //if (X >= maxX)
      //{
      //Y += 15;
      //X = vychoziX;
      //}
      //UO.Wait(pause);
      //}
      //}
      UO.Print(0x0435, "KONEC rovnani!");
    }

    [Command("regy2")]
    public void arrangingregy(int pause)
    {
      ushort X = 17;
      ushort Y = 160;
      ushort[] regy = new ushort[23];
      regy[0] = 0x0F7A; //blackpearls
      regy[1] = 0x0F85; //Ginseng
      regy[2] = 0x0F86; //mandrake root 
      regy[3] = 0x0F8D; //spider silk 
      regy[4] = 0x0F7B; //Blood Moss 
      regy[5] = 0x0F88; //Nightshade 
      regy[6] = 0x0F84; //garlic 
      regy[7] = 0x0F8C; //sulforous ash 
      regy[8] = 0x0F78; //Batwing
      regy[9] = 0x0F91; //Wyrms heart
      regy[10] = 0x0F87; // eon  
      regy[11] = 0x0F8F; //Volcanic Ash
      regy[12] = 0x0F83; //Execution's Cap
      regy[13] = 0x0F79; //Blackmoor
      regy[14] = 0x0F89; //Obsidian        
      regy[15] = 0x0F7E; //Bone
      regy[16] = 0x0F7D; //Blood Vial 
      regy[17] = 0x0F80; //Daemon Bone
      regy[18] = 0x0F8B; //Pumice
      regy[19] = 0x0F81; //Fertile Dirt
      regy[20] = 0x0F8E; //Serpent Scales
      regy[21] = 0x0F7C; //BloodSpawn
      regy[22] = 0x0F7F; //Brimstone
      for (int i = 0; i < 2; i++)
      {
        if (UO.Count(regy[i]) > 0)
        {
          UO.Print("Rovni regu.");
          UOItem reg = World.Player.Backpack.Items.FindType(regy[i]);
          UO.MoveItem(reg, 30000, World.Player.Backpack, X, Y);
          X += 12;
          UO.Wait(pause);
        }
      }
      X += 5;
      for (int i = 2; i < 4; i++)
      {
        if (UO.Count(regy[i]) > 0)
        {
          UOItem reg = World.Player.Backpack.Items.FindType(regy[i]);
          UO.MoveItem(reg, 30000, World.Player.Backpack, X, Y);
          X += 28;
          UO.Wait(pause);
        }
      }
      X = 25;
      Y += 16;
      for (int i = 4; i < 6; i++)
      {
        if (UO.Count(regy[i]) > 0)
        {
          UOItem reg = World.Player.Backpack.Items.FindType(regy[i]);
          UO.MoveItem(reg, 30000, World.Player.Backpack, X, Y);
          X += 25;
          UO.Wait(pause);
        }
      }
      X = 120;
      Y = 160;
      for (int i = 6; i < 8; i++)
      {
        if (UO.Count(regy[i]) > 0)
        {
          UOItem reg = World.Player.Backpack.Items.FindType(regy[i]);
          UO.MoveItem(reg, 30000, World.Player.Backpack, X, Y);
          X += 18;
          UO.Wait(pause);
        }
      }
      X = 110;
      Y += 16;
      for (int i = 8; i < 12; i++)
      {
        if (UO.Count(regy[i]) > 0)
        {
          UOItem reg = World.Player.Backpack.Items.FindType(regy[i]);
          UO.MoveItem(reg, 30000, World.Player.Backpack, X, Y);
          X += 9;
          UO.Wait(pause);
        }
      }
      X = 164;
      Y = 177;
      for (int i = 12; i < 23; i++)
      {
        if (UO.Count(regy[i]) > 0)
        {
          UOItem reg = World.Player.Backpack.Items.FindType(regy[i]);
          UO.MoveItem(reg, 30000, World.Player.Backpack, X, Y);
          UO.Wait(pause);
        }
      }
      UO.Print("Dokonceno rovnani regu.");
    }
  }
}
