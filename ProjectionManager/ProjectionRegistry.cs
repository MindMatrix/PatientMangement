using System;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace ProjectionManager
{

    // public class DeleteCommandRegistrationConvention : IRegistrationConvention
    // {
    //     private static readonly Type _openDeleteCommandType = typeof(DeleteEntityCommand<>);
    //     private static readonly Type _openHandlerInterfaceType = typeof(IHandler<>);
    //     private static readonly Type _openDeleteCommandHandlerType = typeof(DeleteEntityCommandHandler<>);

    //     public void ScanTypes(TypeSet types, Registry registry)
    //     {
    //         foreach (var type in types.FindTypes(TypeClassification.Concretes))
    //         {
    //             if (!type.IsAbstract && typeof(Entity).IsAssignableFrom(type))
    //             {
    //                 Type closedDeleteCommandType = _openDeleteCommandType.MakeGenericType(type);
    //                 Type closedHandlerInterfaceType = _openHandlerInterfaceType.MakeGenericType(closedDeleteCommandType);
    //                 Type closedDeleteCommandHandlerType = _openDeleteCommandHandlerType.MakeGenericType(type);

    //                 registry.For(closedHandlerInterfaceType).Use(closedDeleteCommandHandlerType);
    //             }
    //         }
    //     }
    // }

    public class ProjectionRegistry : Registry
    {
        public ProjectionRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                scanner.AssemblyContainingType<ProjectionManager>();
                scanner.AddAllTypesOf(typeof(IProjection));

                //scanner.AddAllTypesOf(typeof(IProjection<>)).NameBy(x => x.Name);
            });

            //For<IProjection>().Use<ProjectionWrapper>();
            //For(typeof(IProjection<>)).DecorateAllWith(typeof(ProjectionWrapper<>)).Singleton();
            //For(typeof(IProjection)).Use(typeof(IProjection<>));
        }
    }
}