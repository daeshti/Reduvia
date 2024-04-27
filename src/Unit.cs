using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Reduvia;

record struct Unit;

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

/// <inheritdoc cref="ISyncMediator"/>
/// <param name="provider">The service provider that resolves the handlers and middlewares</param>
public class MessageMediator(IServiceProvider provider) : ISyncMediator
{
    private static readonly object Void = new();

    private readonly IJobPool _pool = provider.GetService<IJobPool>()!;

    /// <summary>Sends the message with response to the resolved Single handler</summary>
    public async Task<TR> Send<TM, TR>(TM message, CancellationToken ct) where TM : IMessage<TR>
    {
        var completion = new TaskCompletionSource<object>();
        var handler = provider.GetService<IMessageHandler<TM, TR>>();
        var job = new Job(() => handler!.Handle(message, ct)!, completion);

        _pool.Enqueue(job, ct);
        var res = await job.Completion.Task;
        return (TR)res;
    }

    /// <summary>Broadcasts the message with empty response to all resolved handlers</summary>
    public async Task Send<TN>(TN notification, CancellationToken ct) where TN : INotification
    {
        var jobs = 
            (provider.GetService<IEnumerable<INotificationHandler<TN>>>() ?? Array.Empty<INotificationHandler<TN>>())
            .Select(handler =>
            {
                var completion = new TaskCompletionSource<object>();
                return new Job(() =>
                    {
                        handler.Handle(notification, ct);
                        return Void;
                    },
                    completion);
            })
            .ToArray();

        foreach (var job in jobs)
        {
            _pool.Enqueue(job, ct);
        }

        await Task.WhenAll(
            jobs.Select(x => x.Completion.Task)
        );
    }
    
    /// <summary>
    /// Send message to the handler and then to all middlewares and aggregate the result.
    /// </summary>
    public async Task<TR> SendToAll<TM, TR>(TM message, CancellationToken ct) where TM : IMessage<TR>
    {
        var completion = new TaskCompletionSource<object>();

        var handler = provider.GetService<IMessageHandler<TM, TR>>();
        var middlewares = provider
            .GetServices<IMessageMiddleware<TM, TR>>()
            .OrderBy(x => x.RelativeOrder)
            .Reverse();

        var job = new Job(
            () => middlewares
                .Aggregate(
                    () => handler!.Handle(message, ct),
                    (f, middleware) => () => middleware.Handle(message, f, ct))
                .Invoke()!,
            completion);

        _pool.Enqueue(job, ct);
        var res = await job.Completion.Task;
        return (TR)res;
    }
}