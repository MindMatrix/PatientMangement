namespace ProjectionManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using PatientManagement.AdmissionDischargeTransfer;

    public interface IProjection<T>
    {
        Task Handle(T e);
    }

    public class TestProjection : IProjection<PatientAdmitted>
    {
        public Task Handle(PatientAdmitted e)
        {
            Console.WriteLine($"Whoa! {e.PatientId}");
            return Task.CompletedTask;
        }
    }

    // public abstract class ProjectionWrapper : IProjection
    // {
    //     public abstract bool CanHandle(string eventType);

    //     public abstract void Handle(string eventType, object e);
    // }

    public class ProjectionWrapper<T> : IProjection
    {
        private readonly string _eventType;
        private readonly IProjection<T> _handler;
        public ProjectionWrapper(IProjection<T> handler)
        {
            _eventType = typeof(T).Name;
            _handler = handler;
        }

        public bool CanHandle(string eventType) => _eventType == eventType;

        public void Handle(string eventType, object e) => _handler.Handle((T)e).Wait();
    }




    // public class Projection<T> : IProjection
    // {
    //     private readonly IProjection<T> _handler;

    //     public Projection(ProjectionManager projectionManager, IProjection<T> handler)
    //     {
    //         _handler = handler;
    //     }

    //     protected void When<T>(Action<T> when)
    //     {
    //         _handlers.Add(new EventHandler { EventType = typeof(T).Name, Handler = e => when((T)e) });
    //     }

    //     void IProjection.Handle(string eventType, object e)
    //     {
    //         _handlers
    //             .Where(h => h.EventType == eventType)
    //             .ToList()
    //             .ForEach(h => h.Handler(e));
    //     }

    //     bool IProjection.CanHandle(string eventType)
    //     {
    //         return _handlers.Any(h => h.EventType == eventType);
    //     }
    // }



    public class PatientDemographicProjection : Projection
    {
        public PatientDemographicProjection(ConnectionFactory connectionFactory)
        {
            When<PatientAdmitted>(e =>
            {
                var rangeLookup = RangeLookup.Get(e.AgeInYears);

                using (var session = connectionFactory.Connect())
                {
                    var range = session.Load<Range>(rangeLookup.Name);

                    if (range == null)
                    {
                        session.Store(new Range { Id = rangeLookup.Name, Count = 1 });
                        Console.WriteLine($"{rangeLookup.Name}: 1");
                    }
                    else
                    {
                        range.Count++;
                        Console.WriteLine($"{rangeLookup.Name}: {range.Count}");

                    }

                    session.SaveChanges();
                }
            });
        }
    }

    public class RangeLookup
    {
        static readonly List<RangeLookup> Ranges = new List<RangeLookup>
        {
            new RangeLookup { Name = "0 - 10", Min = 0, Max = 10 },
            new RangeLookup { Name = "11 - 20", Min = 11, Max = 20 },
            new RangeLookup { Name = "21 - 30", Min = 21, Max = 30 },
            new RangeLookup { Name = "31 - 40", Min = 31, Max = 40 },
            new RangeLookup { Name = "41 - 50", Min = 41, Max = 50 },
            new RangeLookup { Name = "51 - 60", Min = 51, Max = 60 },
            new RangeLookup { Name = "61 - 70", Min = 61, Max = 70 },
            new RangeLookup { Name = "71+", Min = 71, Max = int.MaxValue }
        };

        public string Name { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public static RangeLookup Get(int ageInYears)
        {
            return Ranges.Single(r => r.Min <= ageInYears && ageInYears <= r.Max);
        }
    }

    public class Range
    {
        public string Id { get; set; }

        public int Count { get; set; }
    }
}