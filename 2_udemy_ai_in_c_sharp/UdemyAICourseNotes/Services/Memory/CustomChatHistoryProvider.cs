using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace UdemyAICourseNotes.Services.Memory;

//since AgentSession doesn't have an identifier - we are providing a Guid to simulate a session
internal class CustomChatHistoryProvider : ChatHistoryProvider
{
    public CustomSession Session { get; set; }
    private readonly SessionService _sessionService;
    private const string SESSION_NAME = "sessionName";

    public CustomChatHistoryProvider()
    {
        _sessionService = new(); 
    }

    protected override async ValueTask<IEnumerable<ChatMessage>> ProvideChatHistoryAsync(
        InvokingContext context, 
        CancellationToken cancellationToken = default)
    {
        //if we have a session
        if (context.Session.StateBag.TryGetValue(SESSION_NAME, out string name))
        {
            Session = await _sessionService.GetSession(name);
            return Session.Messages; 
        }

        Session = _sessionService.CreateEmptySession(); 

        return Session.Messages;
    }

    protected override async ValueTask StoreChatHistoryAsync(
        InvokedContext context, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(Session); 

        Session.Messages = Session.Messages
            .Concat(context.RequestMessages ?? [])
            .Concat(context.ResponseMessages ?? [])
            .ToList();

        await _sessionService.SaveSession(Session); 
    }
}
