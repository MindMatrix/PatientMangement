using System;
using System.Threading.Tasks;

namespace PatientManagement.Framework
{
    public interface IAggregateRepository
    {
        Task<T> Get<T>(Guid id) where T : IAggregateRoot;
        Task Save(IAggregateRoot aggregateRoot);
    }

}