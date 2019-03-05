using System;
using System.Collections.Generic;
using System.Text;
using Phoenix.WorldData;
using Phoenix.Runtime;
using Phoenix;
using Caleb.Library;
using System.Threading;

namespace CalExtension.UOExtensions
{
  //---------------------------------------------------------------------------------------------
  
  public class RunscriptCounter
  {
    public static void Stop()
    {
      _Stop = true;
    }

    private static bool _Stop = false;

    public int Duration = 0;
    public int CurrentDuration = 0;
    public int Count = 0;
    public static RunscriptCounter Current;
    public DateTime StartTime;

    public void Run()
    {
      Current = this;
      Thread spearateTh = new Thread(new ThreadStart(Start));
      spearateTh.Start();
    }

    public event RunscriptCounterEventHandler RunscriptRun;


    protected void Start()
    {
      int sychr = 0;
      CurrentDuration = Duration;
      StartTime = DateTime.Now;
      _Stop = false;

      do
      {
        OnRunscriptRun(this, new RunscriptCounterEventArgs() { Count = Count, Duration = CurrentDuration, CurrentTime = DateTime.Now, StartTime = StartTime, IsStoped = _Stop });

        Thread.Sleep(5);

        if (UIManager.CurrentState == UIManager.State.Ready)
        {
          Count += 5;
          sychr += 5;
          CurrentDuration -= 5;
        }


      } while (/*sychr < Duration*/ CurrentDuration > 0 && sychr < 60000 && !_Stop);

      _Stop = false;
    }

    private void OnRunscriptRun(object sender, RunscriptCounterEventArgs e)
    {

      if (RunscriptRun != null)
        RunscriptRun(sender, e);
    }

    //---------------------------------------------------------------------------------------------
  }

  public delegate void RunscriptCounterEventHandler(object sender, RunscriptCounterEventArgs e);
    

  public class RunscriptCounterEventArgs : EventArgs
  {
    public int Count = 0;
    public decimal Duration = 0;
    public DateTime StartTime;
    public DateTime CurrentTime;
    public bool IsStoped = false;
  }

}
