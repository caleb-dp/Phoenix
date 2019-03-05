using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;
using Phoenix.Communication;
using Caleb.Library.CAL.Business;
using CalExtension;

namespace Scripts.DarkParadise
{
  public class AdaKladivo
  {
    UOItemType IronIngotType { get { return new UOItemType() { Graphic = 0x1BEF, Color = 0x0000 }; } }
    
    //---------------------------------------------------------------------------------------------

    [Executable]
    public static void BandujCicmundy()
    {
      while (World.Player.Hits < World.Player.MaxHits)
      {
        if (UO.BandageSelf())
        {
          Journal.WaitForText(true, 5000, "Chces vytvorit mumii?", "You put the bloody bandagess in your pack.", "You apply the bandages, but they barely help.", "Your target is already fully healed");
          Journal.Clear();
        }
        else
          break;
      }

      foreach (UOCharacter character in World.Characters)
      {
        if (character.Distance <= 6 && character.RequestStatus(1000) && character.Renamable  && (character.Model == 0x00D6 || character.Model == 0x00C9))
        {
          UO.PrintInformation("Banduju cicu {0}", character.Name);
          while (character.Hits < character.MaxHits)
          {
            UO.WaitTargetObject(character.Serial);
            UO.UseType(0x0E21);
            UO.Wait(1500);
          }

          UO.PrintInformation("Cica {0} dobandovana", character.Name);
        }
      }
    }

    [Executable]
    public static void TamniKockoKocku()
    {
      foreach (UOCharacter character in World.Characters)
      {
        if (character.Distance <= 1 && (character.Model == 0x00D6 || character.Model == 0x00C9))
        {
          UO.WaitTargetObject(character.Serial);
          UO.UseType(0x2119, 0x0530);
          UO.Wait(500);
        }
      }
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void ThrowLavaBombTom()
    {
      UOItem potionItem = null;
      string text = "";

      foreach (UOItem item in UO.Backpack.AllItems)
        if (item.Graphic == 0x0F0D && item.Color == 0x000E && item.Flags == 0x0000) { potionItem = item; break; }

      if (potionItem == null || !potionItem.Exist)
      {
        UO.Warmode(!World.Player.Warmode);
        UO.Wait(100);

        UOItem kad = World.Player.Backpack.AllItems.FindType(0x1843, 0x000E);
        UOItem empty = UO.Backpack.Items.FindType(0x0F0E);

        if (kad.Exist && empty.Exist)
        {
          UO.WaitTargetObject(empty);
          kad.Use();
          UO.Wait(250);

          potionItem = World.Player.Backpack.Items.FindType(0x0F0D, 0x000E);

          if (potionItem.Exist)
          {
            potionItem.Move(1, World.Player.Backpack);
            UO.Wait(100);
          }
        }
        else
          UO.Print("Nemas kad s Lavama nebo prazdny lahve!");
      }

      if (potionItem != null && potionItem.Exist)
      {
        World.Player.Print("Hazu lavabombu" + text);
        StaticTarget st = UIManager.Target();
        potionItem.Move(1, st.X, st.Y, st.Z);
      }
      else
        World.Player.Print("Nemas lavabombu" + text);
    }

    //---------------------------------------------------------------------------------------------

    [Executable]
    [BlockMultipleExecutions]
    public void UsumonLavaBombTom()
    {
      UOItem potionItem =  null;

      UOItem kad = World.Player.Backpack.AllItems.FindType(0x1843, 0x000E);
      UOItem empty = UO.Backpack.Items.FindType(0x0F0E);

      if (kad.Exist && empty.Exist)
      {
        UO.WaitTargetObject(empty);
        kad.Use();
        UO.Wait(250);

        potionItem = World.Player.Backpack.Items.FindType(0x0F0D, 0x000E);

        if (potionItem.Exist)
        {
          potionItem.Move(1, World.Player.Backpack);
          UO.Wait(100);
        }
      }
      else
        UO.Print("Nemas kad s Lavama nebo prazdny lahve!");

      if (potionItem != null && potionItem.Exist && potionItem.Flags != 0x0020)
      {
        foreach (UOItem item in UO.Backpack.AllItems)
        {
          if (item.Graphic == 0x0F0D && item.Color == 0x000E && item.Flags == 0x0020)
          {
            potionItem = item;
            break;
          }
        }
      }

      if (potionItem != null && potionItem.Exist)
      {
        World.Player.Print("Unsumon lavabomb");
        potionItem.Use();
      }
    }

    [Executable]
    public static void VyhodLavaBombuByTom()
    {

      UO.Backpack.AllItems.FindType(0x0F0D, 0x000E).DropHere();
      UO.Wait(1000);
      UOItem lootItemSeber = World.Ground.FindType(0x0F0D);
      if (lootItemSeber.Exist)
      {
        UO.MoveItem(lootItemSeber, 1, Aliases.Backpack, 165, 55);

      }
      UO.Wait(200);
      if (lootItemSeber != null && lootItemSeber.Exist && lootItemSeber.Flags != 0x0020)
      {
        foreach (UOItem item in UO.Backpack.AllItems)
        {
          if (item.Graphic == 0x0F0D && item.Color == 0x000E && item.Flags == 0x0020)
          {
            lootItemSeber = item;
            World.Player.Print("Nasel sem");
            break;
          }
        }
      }
      UO.Wait(200);
      World.Player.Print("klikam");
      lootItemSeber.Use();
    }

    //---------------------------------------------------------------------------------------------

    public static UOItem Kladivo
    {
      get
      {
        UOItem kladivo = World.Player.Layers.FindType(0x1438, 0x0000);//0x044C);// kouknem zda uz neni v ruce
        if (!kladivo.Exist)
          kladivo = World.Player.Backpack.AllItems.FindType(0x1438, 0x0000);  //0x044C);// pripadne do bathu

        if (!kladivo.Exist)
          UO.PrintWarning("Nemas u sebe Ada kladivo!");

        return kladivo;
      }
    }


    //---------------------------------------------------------------------------------------------


    public void ZazdiTarget(ushort ingotAmount)
    {
      UOItem ingots = World.Player.Backpack.AllItems.FindType(IronIngotType.Graphic, IronIngotType.Color);

      if (!ingots.Exist || ingots.Amount < ingotAmount)
      {
        UO.PrintWarning("Nemas potrebny pocet ingotu! Pocet: {0}", ingots.Exist ? ingots.Amount : 0);
        return; 
      }

      UOItem kladivo = Kladivo;
      if (!kladivo.Exist)
        return;

      UO.PrintInformation("Zamer cil >");
      StaticTarget target = UIManager.Target();
      UOPositionBase currentPosition = new UOPositionBase(World.Player.X, World.Player.Y, 0);
      UOPositionBase targetPosition = new UOPositionBase(target.X, target.Y, 0);

      if (Robot.GetRelativeVectorLength(currentPosition, targetPosition) < 4)
      {
        if (ingots.Move(ingotAmount, target.X, target.Y, target.Z))
        {
          UO.Wait(100);
          UOItem targetIngots = World.Ground.FindType(IronIngotType.Graphic, IronIngotType.Color);
          if (targetIngots.Exist && targetIngots.Distance < 4)
          {
            UO.WaitTargetObject(targetIngots.Serial);
            kladivo.Use();
            UO.Wait(100);

            UO.PrintInformation("Ingoty premeneny!"); 
          }
          else
            UO.PrintWarning("Vzdalil jsi se od ingotu! Vzdalenost: {0}", targetIngots.Exist ? targetIngots.Distance : -1);
          
        }
        else
        {
          UO.PrintWarning("Nepodarilo se presunout ingoty na cil!");
        }
      }
      else
      {
        UO.PrintWarning("Cil neexistuje, nebo je dal nez 3 policka! Vzdalenost: {0}", Robot.GetRelativeVectorLength(currentPosition, targetPosition));
      }
    }

    //---------------------------------------------------------------------------------------------

    #region Exec 

    [Executable("Ada1x1")]
    public static void ZazdiTarget1x1()
    {
      new AdaKladivo().ZazdiTarget(10);
    }

    [Executable("Ada3x3")]
    public static void ZazdiTarget3x3()
    {
      new AdaKladivo().ZazdiTarget(30);
    }

    #endregion
  }
}