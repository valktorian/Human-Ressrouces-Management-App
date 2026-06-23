namespace Infrastructure.Api.Messaging;

/// <summary>
/// Contract for command handlers.
/// </summary>
public interface ICommandHandler<TCommand, TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
