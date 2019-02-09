using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOffensive.Assets
{
    public class DelayedAction
    {
        public DelayedAction(Action action, float delay)
        {
            TimeRemaining = delay;
            Action = action;
            Delay = delay;
        }

        public Action Action { get; private set; }
        public float Delay { get; private set; }
        public float TimeRemaining { get; private set; }
        public bool Active { get; private set; }

        public bool Update(float deltaTime)
        {
            if (Active)
            {
                TimeRemaining -= deltaTime;

                if (TimeRemaining <= 0)
                {
                    Action();
                    Active = false;
                    return false;
                }
            }

            return true;
        }

        public void Start()
        {
            Active = true;
        }

        public void Stop()
        {
            Active = false;
        }

        public void Toggle()
        {
            Active = !Active;
        }
    }

    public class TimedAction
    {
        public TimedAction(Action action, float delay)
        {
            TimeRemaining = delay;
            Action = action;
            Delay = delay;
        }

        public Action Action { get; private set; }
        public float Delay { get; private set; }
        public float TimeRemaining { get; private set; }
        public bool Active { get; private set; }

        public void Update(float deltaTime)
        {
            if (Active)
            {
                TimeRemaining -= deltaTime;

                if (TimeRemaining <= 0)
                {
                    Action();
                    TimeRemaining = Delay;
                }
            }
        }

        public void Start()
        {
            Active = true;
        }

        public void Stop()
        {
            Active = false;
        }

        public void Toggle()
        {
            Active = !Active;
        }
    }
}
