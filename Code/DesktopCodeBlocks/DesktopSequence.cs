using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynergySequence;

namespace DesktopCodeBlocks
{
    public class DesktopSequence : Sequence
    {
        Queue<Event> Events = new Queue<Event>();

        public void InvokeFirstEvent()
        {
            if (Events.Count > 0)
                Events.Dequeue().Invoke();
        }

        public int EventsPending()
        {
            return Events.Count;
        }

        public void AddEvent(Event _Event)
        {
            Events.Enqueue(_Event);
        }

        public class Event
        {
            public Event(DesktopCodeBlock _Target)
            {
                target = _Target;
            }

            DesktopCodeBlock target;
            public bool Invoke()
            {
#if !DEBUG
                try
#endif
                {
                    target.Trigger(target.TriggerOutputs[0]);
                    foreach (CodeBlock c in target.Sequence.CodeBlocks) ((DesktopCodeBlock)c).Finalize();
                    return true;
                }
#if !DEBUG
                catch { }
                return false;
#endif
            }
        }
    }
}
