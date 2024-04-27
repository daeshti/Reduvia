using System.Threading;

namespace Reduvia;

/// <summary>
/// Notification handler.
/// </summary>
/// <typeparam name="TN">Type of Message</typeparam>
public interface INotificationHandler<in TN> where TN : INotification
{
    /// <summary>
    /// Handles a notification.
    /// </summary>
    /// <param name="notification">The notification handled</param>
    /// <param name="ct">Cancellation token for the operation</param>
    /// <typeparam name="TN">Type of Message</typeparam>
    void Handle(TN notification, CancellationToken ct);
}