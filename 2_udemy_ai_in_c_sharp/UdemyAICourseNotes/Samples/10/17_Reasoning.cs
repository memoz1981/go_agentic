using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10; 

internal class _17_Reasoning : BaseSample
{
    public override string Description => "LLM Reasoning Effort Level";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory
             .GetClient(Enums.Clients.OpenAI);

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var agentNoneReasoning = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4_MINI,
             name: "agent",
             withMiddleware: true,
             clientType: ClientType.Chat,
             reasoningLevel: OpenAI.Chat.ChatReasoningEffortLevel.None);

        var agentLowReasoning = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4_MINI,
             name: "agent",
             withMiddleware: true,
             clientType: ClientType.Chat,
             reasoningLevel: OpenAI.Chat.ChatReasoningEffortLevel.Low);

        var agentMediumReasoning = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4_MINI,
             name: "agent",
             withMiddleware: true,
             clientType: ClientType.Chat,
             reasoningLevel: OpenAI.Chat.ChatReasoningEffortLevel.Medium);

#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var responseNoReasoning = await agentNoneReasoning.RunAsync(input);
            var responseLowReasoning = await agentLowReasoning.RunAsync(input);
            var responseMediumtReasoning = await agentMediumReasoning.RunAsync(input);

            Green("Agent without reasoning: > ");
            GreenLine(responseNoReasoning.ToString());
            GreenLine(responseNoReasoning.Usage.Counts());
            
            Console.WriteLine();

            Blue("Agent with low reasoning: > ");
            BlueLine(responseLowReasoning.ToString());
            BlueLine(responseLowReasoning.Usage.Counts());

            Console.WriteLine();

            Magenta("Agent with medium reasoning: > ");
            MagentaLine(responseMediumtReasoning.ToString());
            MagentaLine(responseMediumtReasoning.Usage.Counts());

            Separator();
        }
    }
}
