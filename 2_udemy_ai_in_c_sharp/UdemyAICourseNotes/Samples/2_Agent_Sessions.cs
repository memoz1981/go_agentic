using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples; 

internal class _2_Agent_Sessions : BaseSample
{
    private const string EXIT = "exit";
    public override string Description => "Comparison of agents with or without sessions."; 

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var agentWithoutSession = AgentClientFactory
             .GetAgent(Enums.Clients.Github, Models.OpenAI.GPT_4o_MINI);

        var agentWithSession = AgentClientFactory
             .GetAgent(Enums.Clients.Github, Models.OpenAI.GPT_4o_MINI);
        var session = await agentWithSession.CreateSessionAsync(); 

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var responseWithoutSession = await agentWithoutSession.RunAsync(input);
            var responseWithSession = await agentWithSession.RunAsync(input, session);

            Green("Agent without session: > ");
            Green(responseWithoutSession.ToString());
            Console.WriteLine(); 

            Blue("Agent with session: > ");
            Blue(responseWithSession.ToString());

            Separator();
        }
    }
}
