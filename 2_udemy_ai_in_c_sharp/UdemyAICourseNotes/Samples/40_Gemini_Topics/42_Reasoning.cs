using Google.GenAI.Types;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Diagnostics;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._40_Gemini_Topics;

internal class _42_Reasoning : BaseSample
{
    public override string Description => "Gemini - Controlling reasoning";

    public override async Task RunAsync()
    {
        Console.Clear(); 
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetGeminiClient(Models.Gemini.GEMINI_3_5_FLASH_LITE);

        var chatClientAgentOptionsDefault = new ChatClientAgentOptions()
        {
            Name = "agentDefaultReasoning",
            ChatOptions = new()
            {
                Instructions = "Be nice",
            },
        };

        var defaultAgent = AgentClientFactory.GetGeminiAgent(client, chatClientAgentOptionsDefault);

        var chatClientAgentOptionsHigh = new ChatClientAgentOptions()
        {
            Name = "agentHighReasoning",
            ChatOptions = new()
            {
                Instructions = "Be nice",
                RawRepresentationFactory = _ => new GenerateContentConfig()
                {
                    ThinkingConfig = new()
                    {
                        ThinkingLevel = ThinkingLevel.High,
                        IncludeThoughts = true
                    }
                }
            },
        };

        var overThinkerAgent = AgentClientFactory.GetGeminiAgent(client, chatClientAgentOptionsHigh);
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

            var responseHigh = await overThinkerAgent.RunAsync(input);

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
