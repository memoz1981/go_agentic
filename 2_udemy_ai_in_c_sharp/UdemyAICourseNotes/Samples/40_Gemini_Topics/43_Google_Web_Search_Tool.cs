using Google.GenAI.Types;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._40_Gemini_Topics; 

internal class _43_Google_Web_Search_Tool : BaseSample
{
    public override string Description => "Google Web Search Tool";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var searchText = "Give me top 1 space news today (Show today's date, link and keep 20 words max for the description of the news)"; 

        var client = AgentClientFactory.GetGeminiClient(Models.Gemini.GEMINI_3_5_FLASH_LITE);

        var defaultAgent = AgentClientFactory.GetGeminiAgent(client, name: "defaultAgent");

        var agentWithTool1 = AgentClientFactory.GetGeminiAgent(client, name: "agentWithTool1", tools: [new HostedWebSearchTool()]);

        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "agentSetup1",
            ChatOptions = new()
            {
                Instructions = "Be nice",
                RawRepresentationFactory = _ => new GenerateContentConfig()
                {
                    Tools = 
                    [
                        new Tool()
                        {
                            GoogleSearch = new GoogleSearch()
                            {
                                SearchTypes = new SearchTypes()
                                {
                                    WebSearch = new WebSearch()
                                }
                            }
                        }
                    ]
                }
            },
        };

        var agentWithTool2 = AgentClientFactory.GetGeminiAgent(client, chatClientAgentOptions);

        Red($"> {searchText}");

        Console.WriteLine();

        var response1 = await defaultAgent.RunAsync(searchText); 
        YellowLine($"Default Agent> {response1}");

        Console.WriteLine();


        var response2 = await agentWithTool1.RunAsync(searchText);
        GreenLine($"Web Search Agent1> {response2}");

        Console.WriteLine();

        var response3 = await agentWithTool2.RunAsync(searchText);
        WhiteLine($"Web Search Agent2> {response3}");

        Console.WriteLine();
    }
}
