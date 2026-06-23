namespace Infrastructure.Api.Messaging;

/// <summary>
/// Lightweight command dispatcher that resolves handlers from DI.
/// </summary>
public interface ICommandDispatcher
{
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default);
}
