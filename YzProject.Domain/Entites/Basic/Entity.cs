using System.ComponentModel.DataAnnotations;

namespace YzProject.Domain.Entites
{
    public class Entity : IEntity
    {

    }

    public class Entity<TKey> : Entity, IEntity<TKey>
    {
        //[Key]
        public virtual TKey Id { get; set; }
    }
}
