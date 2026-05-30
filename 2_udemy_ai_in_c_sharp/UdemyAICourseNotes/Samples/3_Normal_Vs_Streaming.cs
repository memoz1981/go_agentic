using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples; 

internal class _3_Normal_Vs_Streaming : BaseSample
{
    public override string Description => "Normal vs. streaming output.";
    private static readonly ConsoleColor[] Colors = 
        [
            ConsoleColor.Red,
            ConsoleColor.Green, 
            ConsoleColor.Blue,
            ConsoleColor.Yellow, 
            ConsoleColor.White,
            ConsoleColor.Magenta,
        ];

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

            var responses = agent.RunStreamingAsync(input);
            int colorIndex = 0; 

            await foreach (var response in responses)
            {
                Write(response.ToString(), Colors[colorIndex%Colors.Length]);
                colorIndex++; 
            }

            Separator();
        }
    }
}
