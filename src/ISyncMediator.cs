using System.Threading;
using System.Threading.Tasks;

namespace Reduvia;

/// <summary>
/// The mediator entry point
/// </summary>
public interface ISyncMediator
{
    /// <summary>
    /// Sends the message to a Single handler
    /// </summary>
    Task<TR> Send<TM, TR>(TM message, CancellationToken ct) where TM : IMessage<TR>;

    /// <summary>
    /// Broadcasts the notification to all handlers
    /// </summary>
    Task Send<TN>(TN notification, CancellationToken ct) where TN : INotification;

    /// <summary>
    /// Send message to the handler and then to all middlewares and aggregates the result.
    /// </summary>
    Task<TR> SendToAll<TM, TR>(TM message, CancellationToken ct) where TM : IMessage<TR>;
}