using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Services.Memory;

/// <summary>
/// Saves user facts as raw strings
/// </summary>
internal class CustomContextProvider : AIContextProvider
{
    private readonly AIAgent _memoryAgent;
    private readonly Lazy<Task<List<string>>> _userFacts;
    private readonly MemoryService _memoryService;

    public CustomContextProvider(AIAgent memoryAgent, string userId)
    {
        _memoryAgent = memoryAgent;
        var userMemoryFilePath = Path.Combine(Path.GetTempPath(), $"{userId}.txt");
        _memoryService = new(userMemoryFilePath);

        _userFacts = new(GetUserFacts); 
    }

    private Task<List<string>> GetUserFacts() => _memoryService.GetMemory();

    //called by Invoking - provides additional context for the LLM call
    protected override async ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context,
        CancellationToken cancellationToken = default)
    {
        var userFacts = await _userFacts.Value;

        return new AIContext()
        {
            Instructions = $"User facts - {string.Join(" | ", userFacts)}",
        };
    }

    //called by Invoked - saves the context
    protected override async ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var lastMessageFromUser = context.RequestMessages.Last();
        var userFacts = await _userFacts.Value;

        //build messages to memory agent
        List<ChatMessage> inputToMemoryAgent =
            [
                new(ChatRole.Assistant, $"We know the following about the user already and should not extract that again: {string.Join(" | ", userFacts)}"),
                lastMessageFromUser
            ];

        //send request to memory agent - structured output is requested...Agent will decide which 
        //memory to add and which memory to remove...
        var memoryAgentResponse = await _memoryAgent.RunAsync<MemoryUpdate>(inputToMemoryAgent, 
            cancellationToken: cancellationToken);

        //remove all memory from the in memory user fact
        foreach (var memoryToRemove in memoryAgentResponse.Result.MemoryToRemove ?? [])
        {
            userFacts.Remove(memoryToRemove);
            GrayLine($"Removing user fact: {memoryToRemove}");
        }

        //add any new facts returned
        foreach (var memoryToAdd in memoryAgentResponse.Result.MemoryToAdd ?? [])
        {
            userFacts.Add(memoryToAdd);
            GrayLine($"Adding user fact: {memoryToAdd}");
        }

        var updateCount = (memoryAgentResponse.Result.MemoryToRemove?.Count ?? 0) +
            (memoryAgentResponse.Result.MemoryToAdd?.Count ?? 0);

        if (updateCount > 0)
        {
            //save updated memory
            await _memoryService.SetMemory(userFacts);
            GrayLine($"Saved file with {userFacts.Count} count facts...");
        }
    }
}
