using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using StructureMap;

namespace ProjectionManager
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var eventStoreConnection = GetEventStoreConnection();

            var connectionFactory = new ConnectionFactory("PatientManagement");

            var registry = new Registry();
            registry.IncludeRegistry<ProjectionRegistry>();

            var container = new Container(registry);
            container.Configure(cfg =>
            {
                cfg.For<IEventStoreConnection>().Use(eventStoreConnection);
                cfg.For<ConnectionFactory>().Use(connectionFactory);
                cfg.For<ProjectionManager>().Use<ProjectionManager>();
            });

            foreach (var it in typeof(Program).Assembly.GetExportedTypes().Where(x => x.GetInterface(typeof(IProjection).FullName) != null))
                Console.WriteLine(it);

            foreach (var it in container.GetAllInstances<IProjection>())
                Console.WriteLine(it);

            var projectionManager = container.GetInstance<ProjectionManager>();

            // var projections = new List<IProjection>
            // {
            //     new WardViewProjection(connectionFactory),
            //     new PatientDemographicProjection(connectionFactory)
            // };

            // var projectionManager = new ProjectionManager(
            //     eventStoreConnection,
            //     connectionFactory,
            //     projections);

            projectionManager.Start();

            Console.WriteLine("Projection Manager Running");
            Console.ReadLine();
        }

        static IEventStoreConnection GetEventStoreConnection()
        {
            ConnectionSettings settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials("admin", "changeit"));

            var eventStoreConnection = EventStoreConnection.Create(
                settings,
                new IPEndPoint(IPAddress.Loopback, 1113));

            eventStoreConnection.ConnectAsync().Wait();
            return eventStoreConnection;
        }
    }
}