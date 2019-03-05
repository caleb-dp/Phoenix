using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using System.Threading;
using System.Text.RegularExpressions;

namespace CalExtension.UOExtensions
{
  public class StandAloneScripts
  {
    //Traing taming staf  Color: 0x04B9  Graphic: 0x13F4  
    //Elven bow  Color: 0x0237  Graphic: 0x13B1

    public static bool karibikWait = false;
    //---------------------------------------------------------------------------------------------
    
    [Executable]
    public static void Karibik(Serial poukazka)
    {
      Journal.Clear();
      UO.Print("Karibik Start");
      Journal.EntryAdded += Journal_EntryAdded;
      karibikWait = true;

      UOItem item = new UOItem(poukazka);
      item.Use();

      int counter = 0;
      while (karibikWait && counter <= 14)
      {
        if (counter % 3 == 0)
          UO.Print("Cekam na priklad...");

        UO.Wait(1000);
        counter++;
      }
      karibikWait = false;
      UO.Print("Karibik end");
      Journal.EntryAdded -= Journal_EntryAdded;
    }

    private static void Journal_EntryAdded(object sender, JournalEntryAddedEventArgs e)
    {
      if (karibikWait)
      {
        Regex rgx = new Regex("(?<a>\\d{1,4})[^0-9]*(?<b>\\d{1,4})");
        if (rgx.IsMatch(e.Entry.Text))
        {
          Match m = rgx.Match(e.Entry.Text);

          int a = Int32.Parse(m.Groups["a"].Value.Trim());
          int b = Int32.Parse(m.Groups["b"].Value.Trim());
          Random r = new Random();
          int wait = 1500 + r.Next(1000, 2000);
          UO.Print("{0} + {1} = {2} za {3}", a, b, a + b, wait);
//          UO.Wait(wait);

               //     UO, 0x0043 ,"{0}", a + b);
          

          karibikWait = false;
        }
      }
    }


    //---------------------------------------------------------------------------------------------
  }
}
//21:26 System: Your taming failed, try again.
//21:33 System: What do you want to use this on?
//21:33 Saj-hulud: Try to tame: Rat
//21:33 Saj-hulud: I wont hurt you.
//21:33 System: Your taming failed, try again.
//21:34 System: What do you want to use this on?
//21:34 System: Not tamable.


  //23:44:36: *You Cannot learn anything more from this animal.*

  //23:47:59: a dalsi hlaska je 
  //23:48:00: You are not able to tame this animal