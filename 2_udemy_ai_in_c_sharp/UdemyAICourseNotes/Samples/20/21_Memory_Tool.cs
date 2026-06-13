using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.Memory;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._20; 

internal class _21_Memory_Tool : BaseSample
{
    public override string Description => "Memory agent using Memory Tool (alternative to Sample20 context provider)";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);
        var filePath = Path.Combine(Path.GetTempPath(), "mz.txt");
        var memoryService = new MemoryService(filePath); 

        //this agent doesn't manage memory itself 
        var memoryAgent = AgentClientFactory
             .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_NANO,
            name: "memoryAgent",
            tools: 
            [
                AIFunctionFactory.Create(async () => await memoryService.GetMemory(), "getMemory", "use to get memory"),
                AIFunctionFactory.Create(async (List<string> data) => await memoryService.SetMemory(data), "setMemory", "use to set memory")
            ],
            instructions: "Look at the user's message and extract any memory that we do not already know " +
            "(or non if there aren't any memories to store) " +
            "use 'getMemory' to get all data in memory " +
            "use 'setMemory' to save all the memory changes to memory",
            withMiddleware: true);

        var mainAgent = AgentClientFactory
             .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_NANO,
            name: "mainAgent",
            tools: [memoryAgent.AsAIFunction()],
            instructions: "use memoryAgent to build context and also save the context",
            withMiddleware: true
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
