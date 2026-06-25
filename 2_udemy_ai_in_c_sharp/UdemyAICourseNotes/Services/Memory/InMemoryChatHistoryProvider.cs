using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace UdemyAICourseNotes.Services.Memory;

internal class InMemoryChatHistoryProvider : ChatHistoryProvider
{
    private CustomSession _session; 

    public InMemoryChatHistoryProvider() : base()
    {
        _session = new CustomSession([], Guid.NewGuid().ToString(), "Session", DateTime.UtcNow);
    }

    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
        InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; 

        return _session.Messages;
    }

    protected override async ValueTask StoreChatHistoryAsync(
        InvokedContext context,
        CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; 
        
        _session.Messages = _session.Messages
            .Concat(context.RequestMessages ?? [])
            .Concat(context.ResponseMessages ?? [])
            .ToList();
    }

    public void Reset() => _session = new CustomSession([], Guid.NewGuid().ToString(), "Session", DateTime.UtcNow);
}
