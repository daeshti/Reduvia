using System.Threading;

namespace Reduvia;

/// <summary>
/// A separate thread pool to enqueue jobs into.
/// Start method must be called for it to start working.
/// </summary>
public interface IJobPool
{
    /// <summary>
    /// Enqueues a job into the thread pool.
    /// </summary>
    /// <param name="job">The job to enqueue</param>
    /// <param name="ct">Cancellation token for adding the job into the pool,
    /// it's not passed to the actual job running</param>
    void Enqueue(Job job, CancellationToken ct);
    
    /// <summary>
    /// Makes the threads of the thread pool start working.
    /// </summary>
    /// <param name="ct">Cancellation token for the disposal of the queue,
    /// will actually wait for the jobs to finish before finishing disposal. </param>
    void Start(CancellationToken ct);
}