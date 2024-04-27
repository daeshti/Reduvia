using System;
using System.Threading;
using System.Threading.Tasks;

namespace Reduvia;

public record struct Job(Func<object> Computation, TaskCompletionSource<object> Completion);