using System;
namespace YzProject.Domain.Entites
{
    public interface IEntity
    {
    }

    public interface IEntity<TKey> : IEntity
    { 
          TKey Id { get; set; }
    }
}
