using Anthropic.Models.Messages;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Diagnostics;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._50_Anthropic_Topics; 

internal class _50_Reasoning : BaseSample
{
    public override string Description => "Gemini - Controlling reasoning";

    public override async Task RunAsync()
    {
        Console.Clear();
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClaudeClient();

        var instructions = @"Your are a city finder agent. You only answer questions about cities. As the response you 
                            will return 3 words - The city name, Population, a word describing the city"; 


        var defaultAgent = AgentClientFactory.GetClaudeAgent(client, Models.Claude.HAIKU_4_5, "defaultAgent",
            instructions); 

        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = "agentWithReasoning",
            ChatOptions = new()
            {
                Instructions = instructions,
                RawRepresentationFactory = _ => new MessageCreateParams()
                {
                    MaxTokens = 10000,
                    Messages = [],
                    Model = Models.Claude.HAIKU_4_5,
                    Thinking = new ThinkingConfigParam(new ThinkingConfigEnabled() 
                    { 
                        BudgetTokens = 2000 //minimum 1024
                    })
                }
            },
        };

        var reasoningAgent = AgentClientFactory.GetClaudeAgent(client, Models.Claude.HAIKU_4_5, chatClientAgentOptions);
        var stopWatch = new Stopwatch();

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            stopWatch.Restart();

            var responseDefault = await defaultAgent.RunAsync(input);

            WriteResponse(responseDefault, stopWatch, "Default Agent");

            var responseHigh = await reasoningAgent.RunAsync(input);

            WriteResponse(responseHigh, stopWatch, "Overthinker Agent");

            Separator();
        }
    }

    private static void WriteResponse(AgentResponse response, Stopwatch stopWatch, string agentName)
    {
        GreenLine($"> {agentName}: {response.ToString()}");
        GrayLine($"Response received in {stopWatch.ElapsedMilliseconds} milliseconds. Used {response.Usage.Counts()} tokens");

        foreach (var message in response.Messages)
        {
            foreach (var content in message.Contents)
            {
                if (content is TextReasoningContent textReasoningContent)
                {
                    YellowLine("Reasoning Text:");
                    WhiteLine(textReasoningContent.Text);
                }
            }
        }
        Console.WriteLine();

        stopWatch.Restart();
    }
}
