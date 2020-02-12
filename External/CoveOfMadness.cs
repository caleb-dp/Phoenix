using System;
using System.Collections.Generic;
using System.Text;
using Phoenix;
using Phoenix.WorldData;

namespace CaveofMadness 
{
    public class Madness
    {
        bool mamcil = false;
        UOItem cil;
       
        [Executable]
        public void SipkaKokonNew()
        {
            
            if(!mamcil)
            {
                int vzdalenost = 0;
                foreach (UOItem k in World.Ground)
                  {
                    if(k.Graphic == 0x10DC && k.Color == 0x0B77 && k.Distance < 4)
                    {
                       if (!mamcil)
                       {
                          cil = new UOItem(k);
                          mamcil = true;
                       }
                       if(vzdalenost > k.Distance)
                       {
                          vzdalenost = k.Distance;
                          cil = new UOItem(k);
                       }
                     }                
                  }
             }
          if(mamcil)
          {
            if(!cil.Exist)
            {
                UO.Print("Kokon Znicen");
                mamcil = false;
                return;
            }
            if(cil.Distance < 4)
            {    
              UO.PrintObject(cil.Serial, "-CIL-");   
              UO.WaitTargetObject(cil);
              UO.Cast("Magic Arrow");
            }
            else
            {
              UO.Print("Si moc daleko");
            }
          }
          else
          {
            UO.Print("Nemam CIL.");
          }
        }
   

  }     
}