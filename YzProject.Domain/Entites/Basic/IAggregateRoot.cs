using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YzProject.Domain.Entites
{
    public interface IAggregateRoot : IEntity
    {
        //List<DomainEvent> DomainEvents { get; }
        List<EventBus.Events.IntegrationEvent> DomainEvents { get; }
    }

    public interface IAggregateRoot<TKey> : IEntity<TKey>, IAggregateRoot
    {

    }
}
