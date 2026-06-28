using Google.GenAI.Types;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._40_Gemini_Topics; 

internal class _44_Google_Maps : BaseSample
{
    public override string Description => "Google Maps Integration";

    public override async Task RunAsync()
    {
        Console.Clear(); 
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetGeminiClient(Models.Gemini.GEMINI_3_5_FLASH_LITE);

        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "agent",
            ChatOptions = new()
            {
                Instructions = "You are a Baku maps agent - don't answer any questions other than those related to Google Maps info related to Baku, Azerbaijan",
                RawRepresentationFactory = _ => new GenerateContentConfig()
                {
                    Tools =
                    [
                        new Tool()
                        {
                            GoogleMaps = new GoogleMaps()
                            {
                                EnableWidget = true
                            }
                        }
                    ]
                }
            },
        };

        var agent = AgentClientFactory.GetGeminiAgent(client, chatClientAgentOptions); 

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
            YellowLine(response.Usage.Counts());

            PrintMapsInfo(response); 

            Separator();
        }
    }

    private static void PrintMapsInfo(AgentResponse response)
    {
        if (response.RawRepresentation is not ChatResponse { RawRepresentation: GenerateContentResponse generateContentResponse })
            return;

        Console.WriteLine(); 
        //Google AI map specific properties
        foreach (var candiate in generateContentResponse.Candidates ?? [])
        {
            var groundingMetadata = candiate.GroundingMetadata;

            if (groundingMetadata is null)
                continue;

            //Widget can be displayed using Google Maps Javascript API
            //https://developers.google.com/maps/documentation/javascript/load-maps-js-api
            var widget = groundingMetadata.GoogleMapsWidgetContextToken;
            if (widget is not null)
                YellowLine($"Widget returned: {widget}"); 

            //Grounding data
            foreach (var chunk in groundingMetadata.GroundingChunks ?? [])
            {
                if (chunk?.Maps is null)
                    continue;

                YellowLine("- URL: " + chunk.Maps.Uri);
                YellowLine("- Title: " + chunk.Maps.Title);
                //YellowLine("- Text: \n" + chunk.Maps.Text);
            }
        }

        Console.WriteLine(); 
    }
}
