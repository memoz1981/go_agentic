using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using System.Diagnostics;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._30_Open_AI_Topics;

internal class _30_Service_Tiers : BaseSample
{
    public override string Description => "Service Tiers";

    public override async Task RunAsync()
    {
        Console.Clear(); 
        Gray($"Running the sample for {Description} - ASK CINEMA RELATED QUESTIONS ONLY");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var priorityAgent = GetAgent(client, new ChatServiceTier("priority"), "priorityAgent");

        var defaultAgent = GetAgent(client, ChatServiceTier.Default, "defaultAgent");
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        var stopWatch = Stopwatch.StartNew();  
        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();
            stopWatch.Restart(); 

            var responseDefault = await defaultAgent.RunAsync(input);
            GreenLine($"Default agent response received in {stopWatch.ElapsedMilliseconds} milliseconds as below:");
            Yellow("> "); 
            YellowLine(responseDefault.ToString());

            Console.WriteLine();

            stopWatch.Restart();

            var responsePriority = await priorityAgent.RunAsync(input);
            GreenLine($"Priority agent response received in {stopWatch.ElapsedMilliseconds} milliseconds as below:");
            Yellow("> ");
            YellowLine(responsePriority.ToString());

            Separator();
        }
    }

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    private static AIAgent GetAgent(OpenAIClient client, ChatServiceTier serviceTier, string agentName)
    {
        var chatClientAgentOptions = new ChatClientAgentOptions()
        {
            Name = agentName,
            ChatOptions = new()
            {
                Instructions = "Answer like cinema guru - Answer questions only about cinemas only",
                RawRepresentationFactory = _ => new ChatCompletionOptions()
                {
                    ReasoningEffortLevel = ChatReasoningEffortLevel.High,
                    ServiceTier = serviceTier
                }
            },
        };

        return AgentClientFactory
             .GetAgent(
                openAIClient: client,
                model: Models.OpenAI.GPT_5_4,
                chatClientAgentOptions: chatClientAgentOptions
                );
    }
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}
