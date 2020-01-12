using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using MediatR;
using StructureMap;

namespace ProjectionManager
{
    public class Program
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
                cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
                cfg.For<IMediator>().Use<Mediator>();
            });


            var projectionManager = container.GetInstance<ProjectionManager>();
            Console.WriteLine(container.WhatDoIHave());

            foreach (var it in container.GetAllInstances<IProjection>())
            {
                Console.WriteLine($"{it.GetType().Name}");
            }

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