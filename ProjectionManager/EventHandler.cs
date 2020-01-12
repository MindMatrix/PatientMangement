using System;

namespace ProjectionManager
{
    public class EventHandler
    {
        public string EventType { get; set; }

        public Action<object> Handler { get; set; }
    }
}