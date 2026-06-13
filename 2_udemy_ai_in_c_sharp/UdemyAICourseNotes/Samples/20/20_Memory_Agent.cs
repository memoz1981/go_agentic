using Microsoft.Agents.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.ContextProviders;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._20
{
    internal class _20_Memory_Agent : BaseSample
    {
        public override string Description => "Memory agent using AIContextProvider";

        public override async Task RunAsync()
        {
            Gray($"Running the sample for {Description}");
            Console.WriteLine();
            Console.WriteLine();

            var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI); 

            //this agent doesn't manage memory itself 
            var memoryAgent = AgentClientFactory
                 .GetAgent(
                openAIClient: client, 
                model: Models.OpenAI.GPT_5_4_NANO,
                name: "memoryAgent",
                instructions: "Look at the user's message and extract any memory that we do not already know (or non if there aren't any memories to store)"
                );

            var chatClientAgentOptions = new ChatClientAgentOptions()
            {
                Name = "mainAgent",
                ChatOptions = new()
                { 
                    Instructions = "Talk like italian in english"
                },
                AIContextProviders = [new CustomContextProvider(memoryAgent, "mehdi.zeynalov")]
            };

            var mainAgent = AgentClientFactory
                 .GetAgent(
                openAIClient: client,
                model: Models.OpenAI.GPT_5_4_NANO,
                chatClientAgentOptions: chatClientAgentOptions
                );

            var session = await mainAgent.CreateSessionAsync();

            while (true)
            {
                Red("> ");

                var input = Console.ReadLine() ?? string.Empty;

                if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                    break;

                Console.WriteLine();

                var response = await mainAgent.RunAsync(input, session);

                Green("Agent response: > ");
                GreenLine(response.ToString());

                Separator();
            }
        }
    }
}
