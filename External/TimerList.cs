using System.Collections.Generic;
using System.Timers;

namespace Phoenix.Plugins
{
    public class TimerList<T> : List<T>
    {
        public Timer timer;

        public TimerList(int time)
        {
            timer = new Timer(time*1000) {AutoReset = false};
            timer.Elapsed += (s, a) => Clear();
        }

        public void AddTimer(T item)
        {
            Add(item);
            if (timer.Enabled)
                timer.Stop();
            timer.Start();
        }
    }
}