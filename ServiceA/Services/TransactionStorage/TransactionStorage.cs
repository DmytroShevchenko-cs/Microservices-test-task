namespace ServiceA.Services.TransactionStorage;

using System.Collections.Concurrent;
using Models;

public class TransactionStorage
{
    private readonly ConcurrentDictionary<Guid, TransactionState> _states = new();
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<bool>> _waiters = new();

    public TransactionState Create(Guid id)
    {
        var state = new TransactionState(id);
        _states[id] = state;
        _waiters[id] = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        return state;
    }

    public TransactionState? Get(Guid id) => _states.GetValueOrDefault(id);

    public Task WaitAsync(Guid id, CancellationToken ct)
    {
        if (!_waiters.TryGetValue(id, out var tcs))
        {
            throw new InvalidOperationException("Transaction not found");
        }

        ct.Register(() => tcs.TrySetCanceled());
        return tcs.Task;
    }

    public void TryComplete(Guid id)
    {
        if (_states.TryGetValue(id, out var state) && state.IsComplete)
        {
            if (_waiters.TryGetValue(id, out var tcs))
            {
                tcs.TrySetResult(true);
            }
        }
    }
    
    public void Remove(Guid id)
    {
        _states.TryRemove(id, out _);
        _waiters.TryRemove(id, out _);
    }
}