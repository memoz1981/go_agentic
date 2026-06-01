using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples; 

internal class _8_Agents_As_Tools : BaseSample
{
    public override string Description => "Agents as tools of parent agents";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        //date time agent
        IList<AITool> toolsDateTime =
            [
                AIFunctionFactory.Create(DateTimeTools.GetTodaysDate),
                AIFunctionFactory.Create(DateTimeTools.GetNumberOfDaysFromNow)
            ];

        // date time agent for date related queries
        var dateTimeAgent = AgentClientFactory
             .GetAgent(
             client: Enums.Clients.Github, 
             model: Models.OpenAI.GPT_4o_MINI, 
             instructions: "Use this tool for any query for datetime",
             tools: toolsDateTime, 
             withMiddleware: true);

        //important note 
        //as per microsoft documentation -> we need to use dateTimeAgent.AsAiFunction() - which throws an exception, so instead called it directly... 
        // this seems to be working... 
        IList<AITool> tools =
            [
                AIFunctionFactory.Create(async (string input) => await dateTimeAgent.RunAsync(input), "dateTimeAgent", "use for any queries for date"),
                AIFunctionFactory.Create((DateOnly date) => FakeWeatherTool.GetWeather(date), "getWeather", "route all weather requests to this tool providing the date")
            ];

        var agent = AgentClientFactory
            .GetAgent(
            client: Enums.Clients.Github, 
            model: Models.OpenAI.GPT_4o_MINI, 
            tools: tools,
            withMiddleware: true);

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
