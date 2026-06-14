using Microsoft.Agents.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.Memory;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._20; 

internal class _22_Memory_Conversation_Persistence : BaseSample
{
    public override string Description => "Conversation Persistence (per session)";
    private readonly SessionService _sessionService;
    private const string SESSION_NAME = "sessionName";

    public _22_Memory_Conversation_Persistence()
    {
        _sessionService = new();
    }

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);

        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "mainAgent",
            ChatOptions = new()
            {
                Instructions = "Talk like chinese with intermediate English"
            },
            ChatHistoryProvider = new CustomChatHistoryProvider()
        };

        var mainAgent = AgentClientFactory
             .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_NANO,
            chatClientAgentOptions: chatClientAgentOptions
            );

        var session = await mainAgent.CreateSessionAsync();

        GreenLine("Select one of the following sessions: ");

        var sessions = await _sessionService.GetAllSessions();

        for (int i = 0; i < sessions.Count; i++)
        {
            GreenLine($"{i} - {sessions[i].CreatedAt} - {sessions[i].Description}");
        }

        GreenLine($"-1 to start a new session:");

        if (int.TryParse(Console.ReadLine(), out var index) && index >= 0 && index < sessions.Count)
        {
            session.StateBag.SetValue(SESSION_NAME, sessions[index].Name);

            //demonstration only - not required for the code to work...
            var customSession = await _sessionService.GetSession(sessions[index].Name);
            foreach (var message in customSession.Messages)
            {
                GrayLine($"{message.Role.Value} > {message.Text}");
            }
        }
        else
        {
            var newSession = _sessionService.CreateEmptySession();
            session.StateBag.SetValue(SESSION_NAME, newSession.Name);
        }

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var response = await mainAgent.RunAsync(input, session);

            Green("Agent response: > ");
            GreenLine(response.ToString());

            Separator();
        }
    }
}
