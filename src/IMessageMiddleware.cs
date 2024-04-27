using System;
using System.Threading;

namespace Reduvia;

/// <summary>
/// A middleware that handles a message and then passes to the next middleware.
/// </summary>
/// <typeparam name="TM">Type of Message</typeparam>
/// <typeparam name="TR">Type of Response</typeparam>
public interface IMessageMiddleware<in TM, TR>
{
    /// <summary>
    /// The smaller the order the sooner the middleware is executed
    /// </summary>
    uint RelativeOrder { get; }


    /// <summary>
    /// Handles a message and then passes to the next middleware.
    /// </summary>
    /// <param name="message">The message handled</param>
    /// <param name="nextMiddleware">The next middleware to call</param>
    /// <param name="ct">Cancellation token for the operation</param>
    /// <typeparam name="TM">Type of Message</typeparam>
    /// <typeparam name="TR">Type of Response</typeparam>
    TR Handle(TM message, Func<TR> nextMiddleware, CancellationToken ct);
}