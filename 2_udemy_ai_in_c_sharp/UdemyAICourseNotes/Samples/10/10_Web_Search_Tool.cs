using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10;

internal class _10_Web_Search_Tool : BaseSample
{
    public override string Description => "Web search tool";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var agent = AgentClientFactory
             .GetAgent(
             client: Enums.Clients.OpenAI,
             model: Models.OpenAI.GPT_5_4,
             name: "agent",
             tools: 
             [
                 AIFunctionFactory.Create(DateTimeTools.GetTodaysDate),
                 new HostedWebSearchTool()
             ],
             instructions: "You are a finance news Agent (Always in include today's date at the top of your answers)",
             withMiddleware: true,
             clientType: ClientType.Response);

        var session = await agent.CreateSessionAsync(); 

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var response = await agent.RunAsync(input, session);

            Green("Agent > ");
            GreenLine(response.ToString());

            Separator();
        }
    }
}
