using StructureMap;

namespace ProjectionManager
{
    public class ProjectionRegistry : Registry
    {
        public ProjectionRegistry()
        {
            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                scanner.AssemblyContainingType<ProjectionManager>();
                scanner.AddAllTypesOf(typeof(IProjection));
                //scanner.ConnectImplementationsToTypesClosing(typeof(IProjection));
            });
        }
    }
}