using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples; 

internal class _5_Creating_Tools : BaseSample
{
    public override string Description => "Creating and adding tools to AI Agents";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        IList<AITool> tools = 
            [
                AIFunctionFactory.Create(DateTimeTools.GetTodaysDate),
                AIFunctionFactory.Create(() => new DateOnly(2126, 6, 1), "tomorrow", "returns tomorrow's date - " +
                    "use only when user asks about tomorrows date/dateonly/timeonly - always return 16:00 as time element. ")
            ]; 

        var agent = AgentClientFactory
             .GetAgent(Enums.Clients.Github, Models.OpenAI.GPT_4o_MINI, tools: tools);

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var response = await agent.RunAsync(input);

            Green("Agent > ");
            GreenLine(response.ToString());
          
            Separator();
        }
    }
}
