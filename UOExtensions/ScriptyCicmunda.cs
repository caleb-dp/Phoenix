using System;
using Phoenix;
using Phoenix.Communication;
using Phoenix.WorldData;
using System.Collections.Generic;

namespace ScriptyCicmunda
{
  public class TheCica
  {
    //[Executable]//ONLY .NET 4
    //public static void TrideniKockoKlamakuSA()
    //{
    //  UO.Print("Zdrojovy kontejner >");
    //  UOItem containerFrom = new UOItem(UIManager.TargetObject());
    //  UO.Print("Cilovy kontejner (esc = Zdrojovy kontejner) >");
    //  UOItem containerTo = new UOItem(UIManager.TargetObject());

    //  if (containerTo == Serial.Invalid)
    //    containerTo = containerFrom;

    //  Tuple<Graphic, UOColor> cougar = new Tuple<Graphic, UOColor>(0x2119, 0x0000);
    //  Tuple<Graphic, UOColor> panther = new Tuple<Graphic, UOColor>(0x2119, 0x0000);
    //  Tuple<Graphic, UOColor> leopard = new Tuple<Graphic, UOColor>(0x2119, 0x0000);
    //  Tuple<Graphic, UOColor> cat1 = new Tuple<Graphic, UOColor>(0x211B, 0x08FF);
    //  Tuple<Graphic, UOColor> cat2 = new Tuple<Graphic, UOColor>(0x211B, 0x0909);
    //  Tuple<Graphic, UOColor> cat3 = new Tuple<Graphic, UOColor>(0x211B, 0x0904);
    //  Tuple<Graphic, UOColor> cat4 = new Tuple<Graphic, UOColor>(0x211B, 0x090C);

    //  containerTo.Use();
    //  UO.Wait(500);

    //  List<UOItem> searchItems = new List<UOItem>();
    //  searchItems.AddRange(containerFrom.Items);

    //  UO.Print("Tridim cicmundy ;]");
    //  foreach (UOItem item in searchItems)
    //  {
    //    //UO.Print(((UOItemType)item).ToString() + ", " + ((UOItemType)item == leopard));
    //    if (cougar == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))//.EqualUOItem(item))
    //    {
    //      item.Move(1, containerTo, 20, 35);
    //      UO.Wait(500);
    //    }
    //    else if (panther == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))
    //    {
    //      item.Move(1, containerTo, 40, 35);
    //      UO.Wait(500);
    //    }
    //    else if (leopard == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))
    //    {
    //      item.Move(1, containerTo, 60, 35);
    //      UO.Wait(500);
    //    }
    //    else if (cat1 == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))
    //    {
    //      item.Move(1, containerTo, 80, 35);
    //      UO.Wait(500);
    //    }
    //    else if (cat2 == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))
    //    {
    //      item.Move(1, containerTo, 100, 35);
    //      UO.Wait(500);
    //    }
    //    else if (cat3 == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))
    //    {
    //      item.Move(1, containerTo, 120, 35);
    //      UO.Wait(500);
    //    }
    //    else if (cat4 == new Tuple<Graphic, UOColor>(item.Graphic, item.Color))
    //    {
    //      item.Move(1, containerTo, 140, 35);
    //      UO.Wait(500);
    //    }

    //  }
    //  UO.Print("Tridim cicmundy ;] dokonceno");
    //}

    [Executable]
    public static void TrideniKockoKlamakuSA()
    {
      UO.Print("Zdrojovy kontejner >");
      UOItem containerFrom = new UOItem(UIManager.TargetObject());
      UO.Print("Cilovy kontejner (esc = Zdrojovy kontejner) >");
      UOItem containerTo = new UOItem(UIManager.TargetObject());

      if (containerTo == Serial.Invalid)
        containerTo = containerFrom;

      Tuplik cougar = new Tuplik(0x2119, 0x0000); 
      Tuplik panther = new Tuplik(0x2119, 0x0901) ;
      Tuplik leopard = new Tuplik(0x2119, 0x0972); 
      Tuplik cat1 = new Tuplik(0x211B, 0x08FF) ;
      Tuplik cat2 = new Tuplik(0x211B, 0x0909);
      Tuplik cat3 = new Tuplik(0x211B, 0x0904);
      Tuplik cat4 = new Tuplik(0x211B, 0x090C);

      containerTo.Use();
      UO.Wait(500);
      containerFrom.Use();

      List<UOItem> searchItems = new List<UOItem>();
      searchItems.AddRange(containerFrom.Items);

      UO.Print("Tridim cicmundy ;]");
      foreach (UOItem item in searchItems)
      {
        bool wait = false;
        //UO.Print(((UOItemType)item).ToString() + ", " + ((UOItemType)item == leopard));
        if (cougar.EqualUOItem(item))
        {
          item.Move(1, containerTo, 20, 35);
          wait = true;
        }
        else if (panther.EqualUOItem(item))
        {
          item.Move(1, containerTo, 40, 35);
          wait = true;
        }
        else if (leopard.EqualUOItem(item))
        {
          item.Move(1, containerTo, 60, 35);
          wait = true;
        }
        else if (cat1.EqualUOItem(item))
        {
          item.Move(1, containerTo, 80, 35);
          wait = true;
        }
        else if (cat2.EqualUOItem(item))
        {
          item.Move(1, containerTo, 100, 35);
          wait = true;
        }
        else if (cat3.EqualUOItem(item))
        {
          item.Move(1, containerTo, 120, 35);
          wait = true;
        }
        else if (cat4.EqualUOItem(item))
        {
          item.Move(1, containerTo, 140, 35);
          wait = true;
        }

        if (wait)
          UO.Wait(500);
      }
      UO.Print("Tridim cicmundy ;] dokonceno");
    }

    public class Tuplik
    {
      public Graphic Graphic;
      public UOColor Color;
      public Tuplik(Graphic graphic, UOColor color)
      {
        this.Graphic = graphic;
        this.Color = color;
      }

      public bool EqualUOItem(UOItem item)
      {
        return this.Graphic == item.Graphic && item.Color == this.Color;
      }

    }
  }
}
