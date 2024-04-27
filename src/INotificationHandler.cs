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
    /// <typeparam name="TN">Type of Message</typeparam>
    /// <typeparam name="TR">Type of Response</typeparam>
    void Handle(TN notification, CancellationToken ct);
}