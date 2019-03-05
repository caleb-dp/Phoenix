using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Phoenix.Configuration;

namespace CalExtension
{
  [Phoenix.SettingsCategory("Caleb global config")]
  public partial class CalebGlobalConfigSettings : UserControl
  {
    public CalebGlobalConfigSettings()
    {
      InitializeComponent();


      CalebConfig.PropertyChanged += CalebConfig_PropertyChanged;
      Disposed += new EventHandler(CalebConfig_Disposed);


      tbxGlobalCalebConfig.Text = CalebConfig.GlobalCalebConfig;
    }

    private void CalebConfig_PropertyChanged(object sender, EventArgs e)
    {
      if (InvokeRequired)
      {
        BeginInvoke(new EventHandler(CalebConfig_PropertyChanged), sender, e);
        return;
      }


      tbxGlobalCalebConfig.Text = CalebConfig.GlobalCalebConfig;
    }


    void CalebConfig_Disposed(object sender, EventArgs e)
    {
      CalebConfig.PropertyChanged -= new EventHandler(CalebConfig_PropertyChanged);
    }

    private void TbxGlobalCalebConfig_TextChanged(object sender, System.EventArgs e)
    {
      CalebConfig.GlobalCalebConfig = tbxGlobalCalebConfig.Text;
    }


  }
}
