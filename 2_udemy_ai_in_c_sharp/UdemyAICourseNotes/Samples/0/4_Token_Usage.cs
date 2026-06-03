using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples; 

internal class _4_Token_Usage : BaseSample
{
    public override string Description => "Demonstration of the token usage in sessionless and sessionful agents.";

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
            GreenLine(responseWithoutSession.ToString());
            GreenLine(responseWithoutSession.Usage.Counts()); 
            Console.WriteLine();

            Blue("Agent with session: > ");
            BlueLine(responseWithSession.ToString());
            BlueLine(responseWithSession.Usage.Counts()); 
            
            Separator();
        }
    }
}
