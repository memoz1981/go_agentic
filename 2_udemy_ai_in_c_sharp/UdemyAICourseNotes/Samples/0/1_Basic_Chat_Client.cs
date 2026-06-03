using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples;

internal class _1_Basic_Chat_Client : BaseSample
{
    public override string Description => "First demo model showing how chat client works."; 

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

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
