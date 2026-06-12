using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._10;

internal class _19_Empty : BaseSample
{
    public override string Description => "Empty - Placeholder";

    public override async Task RunAsync()
    {
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory
             .GetClient(Enums.Clients.OpenAI);

        var agent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "agent",
             instructions: "You are a console window symbol artist",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var input = @"Draw the word 'EMPTY' using stars (*) only. 
Height of each letter: 30 stars
Width of each letter: Scaled as per the height 
space between letters: 10 characters
each letter should be clearly visible readable";

        var result = await agent.RunAsync(input);

        Green(result.ToString()); 
    }
}
