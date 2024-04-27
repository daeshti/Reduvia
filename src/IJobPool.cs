using System.Threading;

namespace Reduvia;

public interface IJobPool
{
    void Enqueue(Job task, CancellationToken ct);
    void Start(CancellationToken ct);
}