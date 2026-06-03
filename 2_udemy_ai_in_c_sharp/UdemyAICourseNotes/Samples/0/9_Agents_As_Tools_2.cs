using Microsoft.Agents.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples;

internal class _9_Agents_As_Tools_2 : BaseSample
{
    public override string Description => "Agents as tools - second go...";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        //astronomy agent
        var astronomyAgent = AgentClientFactory
             .GetAgent(
             client: Enums.Clients.OpenAI,
             model: Models.OpenAI.GPT_5_4_MINI,
             name: "astronomyAgent",
             instructions: "You are an astronomy expert.",
             withMiddleware: true);

        var mainAgent = AgentClientFactory
            .GetAgent(
            client: Enums.Clients.Github,
            model: Models.OpenAI.GPT_4o_MINI,
            name: "mainAgent",
            instructions: "Refer all astronomy questions to 'astronomyAgent'",
            tools: [ astronomyAgent.AsAIFunction() ],
            withMiddleware: true);

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var response = await mainAgent.RunAsync(input);

            Green("Agent > ");
            GreenLine(response.ToString());

            Separator();
        }
    }
}
