using System.Threading;

namespace Reduvia;

/// <summary>
/// Message handler.
/// </summary>
/// <typeparam name="TM">Type of Message</typeparam>
/// <typeparam name="TR">Type of Response</typeparam>
public interface IMessageHandler<in TM, out TR> where TM : IMessage<TR>
{
    /// <summary>
    /// Handles a message.
    /// </summary>
    /// <param name="message">The message handled</param>
    /// <param name="ct">Cancellation token for the operation</param>
    /// <typeparam name="TM">Type of Message</typeparam>
    /// <typeparam name="TR">Type of Response</typeparam>
    TR Handle(TM message, CancellationToken ct);
}