using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._40_Gemini_Topics; 

internal class _40_Basic_Gemini_Agent : BaseSample
{
    public override string Description => "Gemini - first demo";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetGeminiClient(Models.Gemini.GEMINI_3_5_FLASH_LITE);

        var agent = AgentClientFactory.GetGeminiAgent(client);

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
