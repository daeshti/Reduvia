using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Reduvia;

public class JobPool : IDisposable, IJobPool
{
    private readonly Thread[] _workers;
    private readonly BlockingCollection<Job> _jobs;
    private CancellationToken _ct;
    public JobPool() : this(Environment.ProcessorCount * 256)
    {
    }

    public JobPool(int size)
    {
        _workers = new Thread[size];
        for (var i = 0; i < _workers.Length; i++)
        {
            _workers[i] = new Thread(Work);
        }

        _jobs = new BlockingCollection<Job>(boundedCapacity: size * 2);
    }

    public void Enqueue(Job task, CancellationToken ct)
    {
        _jobs.Add(task, ct);
    }

    public void Start(CancellationToken ct)
    {
        _ct = ct;
        foreach (var worker in _workers)
        {
            worker.Start();
        }
    }

    private void Work()
    {
        try
        {
            while (true)
            {
                var task = _jobs.Take(_ct);
                try
                {
                    var result = task.Computation.Invoke();
                    task.Completion.SetResult(result);
                }
                catch (OperationCanceledException)
                {
                    // OperationCanceledException must be propagated.
                    throw;
                }
                catch (Exception e)
                {
                    task.Completion.SetException(e);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // OperationCanceledException must be propagated.
            throw;
        }
        catch (Exception)
        {
            // ignored
            // The queue is disposing, or has already been disposed
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        // Don't accept any new tasks
        _jobs.CompleteAdding();
        
        // ReSharper disable MethodSupportsCancellation
        Task.Run(async () =>
        {
            // Wait until all the running tasks are done
            // ReSharper disable once AccessToDisposedClosure
            while (_jobs.Count != 0)
            {
                await Task.Delay(1000);
            }
        }).GetAwaiter().GetResult();
        // ReSharper restore MethodSupportsCancellation

        _jobs.Dispose();
    }
}