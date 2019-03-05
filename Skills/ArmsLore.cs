using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;
using CalExtension.Skills;

namespace CalExtension.Skills
{
  public class ArmsLore : Skill
  {
    [Executable]
    public static void TrainArmsLore()
    {
      Journal.Clear();
      Game.PrintMessage("Vez ke zkoumani >");
      UOItem item = new UOItem(UIManager.TargetObject());

      while (item.Exist && !UO.Dead)
      {
        UO.UseSkill(StandardSkill.ArmsLore);
        UO.WaitTargetObject(item.Serial);
        Game.Wait(100);

        JournalEventWaiter jew = new JournalEventWaiter(true, "This item is", "You are uncertain about this item");

        UO.Print("Wait Start");// + counter);
        jew.Wait(2000);
        UO.Print("Wait end");//UO.Print("Wait :" + counter);
      //  int counter = 0;
        //while(Journal.WaitForText(true, 1500,"This item is", "You are uncertain about this item"))
        //{
        //  UO.Print("Wait :" + counter);
        //  Game.Wait(100);
        //  counter = counter + 100;  
        //}
        Journal.Clear();
        //This item is
        //You are uncertain about this item
        Game.Wait(250);
      }
    }
  }
}
//Attack [13].This item is in full repair. It is repairable.
//15:41 System: You are uncertain about this item
//15:41 System: What would you like to evaluate?
//15:41 System: Attack [11].This item is in full repair. It is repairable.
//15:41 System: You are uncertain about this item
//15:41 System: What would you like to evaluate?
//15:41 System: Attack [4].This item is in full repair. It is repairable.
//15:41 You see: chest of drawers crafted by Pedisequus (29 items)
//15:41 System: What would you like to evaluate?
//15:41 System: Attack [7].This item is in full repair. It is repairable.