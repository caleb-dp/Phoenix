using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phoenix;
using System.Drawing;

namespace CalExtension.UI.Status
{
    public class Supplies
    {
        public static SuppliesForm GetCurrentWraper()
    {
      SuppliesForm form = WindowManager.GetDefaultManager().OwnedWindows.OfType<SuppliesForm>().FirstOrDefault() as SuppliesForm;
      

      return form;
              //      return WindowManager.GetDefaultManager().OwnedWindows.OfType<StatusForm>().Where(f => f.MobileId == id).Count() > 0;
    }

    


        [Executable("calreagents")]
        public void ShowReagentCounter()
        {
            // Vytvor okno
            WindowManager.GetDefaultManager().CreateWindow(delegate()
            {
                var f = new SuppliesForm();
                f.Size = new Size(200, 20);

                // Add(type, color, stockable, img offset);
                //f.Add(0x0F8D, 0x0000, true, new Point(-10, -1)); // SS
                //f.Add(0x0F7B, 0x0000, true, new Point(-5, -1)); // BM
                //f.Add(0x0F7A, 0x0000, true, new Point(-20, -1)); // BP
                //f.Add(0x0F84, 0x0000, true); // GA
                //f.Add(0x0F85, 0x0000, true); // GI
                //f.Add(0x0F86, 0x0000, true); // MR
                //f.Add(0x0F88, 0x0000, true); // NS
                //f.Add(0x0F8C, 0x0000, true, new Point(-11, -1)); // SA

                //f.Add(0x0EED, 0x0EEF, 0x0000, false, new Point(-7, -1)); // Gold
                //f.Add(0x0F3F, UOColor.Invariant, true); // Arrow
                //f.Add(0x1BFB, UOColor.Invariant, true); // Bolt

                return f;
            });
        }
    }
}
