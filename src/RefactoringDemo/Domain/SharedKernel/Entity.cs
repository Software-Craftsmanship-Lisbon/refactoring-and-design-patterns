using Flunt.Notifications;
using System;

namespace RefactoringDemo.Domain.SharedKernel
{
    public abstract class Entity : Notifiable
    {
        protected Entity() => Id = Guid.NewGuid();

        public Guid Id { get; private set; }
    }
}