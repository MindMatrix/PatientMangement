using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectionManager
{
    public abstract class Projection : IProjection
    {
        private readonly List<EventHandler> _handlers = new List<EventHandler>();

        protected void When<T>(Action<T> when)
        {
            _handlers.Add(new EventHandler { EventType = typeof(T).Name, Handler = e => when((T)e) });
        }

        void IProjection.Handle(string eventType, object e)
        {
            foreach (var it in _handlers)
                if (it.EventType == eventType)
                    it.Handler(e);
        }

        bool IProjection.CanHandle(string eventType)
        {
            return _handlers.Any(h => h.EventType == eventType);
        }
    }
}