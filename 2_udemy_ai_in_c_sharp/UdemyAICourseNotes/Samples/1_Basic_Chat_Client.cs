using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples;

internal class _1_Basic_Chat_Client : BaseSample
{
    private const string EXIT = "exit"; 
    public override async Task RunAsync()
    {
        var agent = AgentClientFactory
            .GetAgent(Enums.Clients.Github, Models.OpenAI.GPT_4o_MINI);

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;


            Console.WriteLine();
            Green("Agent > ");

            var response = await agent.RunAsync(input);

            Green(response.ToString());

            Separator();
        }
    }
}
