using System;
using System.Timers;
using CoolParking.BL.Interfaces;

namespace CoolParking.BL.Services
{
    public class TimerService : ITimerService
    {
        private Timer timer;
        public event ElapsedEventHandler Elapsed;

        public TimerService(double interval, ElapsedEventHandler eventHandler)
        {
            timer = new Timer();
            Interval = interval;
            timer.Elapsed += eventHandler;
        }

        public double Interval
        {
            get => Interval;
            set
            {
                if (value > 0)
                {
                    timer.Interval = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        public void Start()
        {
            timer.Start();
            timer.Enabled = true;
            timer.AutoReset = true;
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}

// TODO: implement class TimerService from the ITimerService interface.
//       Service have to be just wrapper on System Timers.
