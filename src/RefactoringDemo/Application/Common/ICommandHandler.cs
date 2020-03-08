namespace RefactoringDemo.Application.Common
{
    public interface ICommandHandler<T>
        where T : ICommand
    {
        ICommandResult Handle(T command);
    }
}