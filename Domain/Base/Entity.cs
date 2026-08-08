using System;

namespace Domain.Base
{ // общий класс

    /// <typeparam name="TId">The type of the entity's ID.</typeparam>
    public abstract class Entity<TId> // нельз€ создать экземпл€р, только наследование
        where TId : struct, IEquatable<TId>
    {
        public TId Id { get; protected set; }

        protected Entity(TId id)
        { // IEquatable<TId>-TId должен поддерживать сравнение,struct-должен быть значимым типом (int, Guid, DateTime)
            Id = id;
        }

        protected Entity()
        {
            Id = default!; // !-Ёто точно не null(подавление предупреждений)
        }
    }
}
