using Flunt.Notifications;
using System.Collections.Generic;

namespace RefactoringDemo.Application.Common
{
    public interface ICommandResult
    {
        bool Success { get; }

        IReadOnlyCollection<Notification> Notifications { get; set; }
    }
}