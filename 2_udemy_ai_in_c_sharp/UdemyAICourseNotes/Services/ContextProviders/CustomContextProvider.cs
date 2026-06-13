using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Services.ContextProviders;

/// <summary>
/// Saves user facts as raw strings
/// </summary>
internal class CustomContextProvider : AIContextProvider
{
    private readonly AIAgent _memoryAgent;
    private readonly List<string> _userFacts = [];
    private readonly string _userMemoryFilePath;

    public CustomContextProvider(AIAgent memoryAgent, string userId)
    {
        _memoryAgent = memoryAgent;
        _userMemoryFilePath = Path.Combine(Path.GetTempPath(), $"{userId}.txt");
        if (File.Exists(_userMemoryFilePath))
        {
            _userFacts.AddRange(File.ReadAllLines(_userMemoryFilePath));
            GrayLine($"Loading facts with {_userFacts.Count} count user facts");
        }
    }

    //called by Invoking - provides additional context for the LLM call
    protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext context,
        CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new AIContext()
            {
                Instructions = $"User facts - {string.Join(" | ", _userFacts)}",
            });

    //called by Invoked - saves the context
    protected override async ValueTask StoreAIContextAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var lastMessageFromUser = context.RequestMessages.Last();

        //build messages to memory agent
        List<ChatMessage> inputToMemoryAgent =
            [
                new(ChatRole.Assistant, $"We know the following about the user already and should not extract that again: {string.Join(" | ", _userFacts)}"),
                lastMessageFromUser
            ];

        //send request to memory agent - structured output is requested...Agent will decide which 
        //memory to add and which memory to remove...
        var memoryAgentResponse = await _memoryAgent.RunAsync<MemoryUpdate>(inputToMemoryAgent, 
            cancellationToken: cancellationToken);

        //remove all memory from the in memory user fact
        foreach (var memoryToRemove in memoryAgentResponse.Result.MemoryToRemove ?? [])
        {
            _userFacts.Remove(memoryToRemove);
            GrayLine($"Removing user fact: {memoryToRemove}");
        }

        //add any new facts returned
        foreach (var memoryToAdd in memoryAgentResponse.Result.MemoryToAdd ?? [])
        {
            _userFacts.Add(memoryToAdd);
            GrayLine($"Adding user fact: {memoryToAdd}");
        }

        var updateCount = (memoryAgentResponse.Result.MemoryToRemove?.Count ?? 0) +
            (memoryAgentResponse.Result.MemoryToAdd?.Count ?? 0);

        if (updateCount > 0)
        {
            //save updated memory
            await File.WriteAllLinesAsync(_userMemoryFilePath, _userFacts, cancellationToken: cancellationToken);
            GrayLine($"Saved file with {_userFacts.Count} count facts...");
        }
    }

    private record MemoryUpdate(List<string> MemoryToAdd, List<string> MemoryToRemove); 
}
