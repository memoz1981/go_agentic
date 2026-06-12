using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.VectorRepo;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._10;

internal class _16_RAG_As_A_Tool : BaseSample
{
    public override string Description => "RAG (Retrieval-Augmented Generation) as a tool";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);
        var embeddingAgent = AgentClientFactory.GetEmbeddingGenerator(client,
            Models.OpenAIEmbedding.LARGE_3);

        using var repo = new SqlLiteVectorRepo<SimpleTextVector>(embeddingAgent);
        using var collection = await repo.GetCollection("office");
        using var vectorSearchTool = new VectorSearchTool(repo, collection);

        var mainAgent = AgentClientFactory
             .GetAgent(
             openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "agent",
             tools: [AIFunctionFactory.Create(vectorSearchTool.Search, "search_knowledge")],
             instructions: "You are an expert in the companies Internal Knowledge Base (use the 'search_knowledge' tool)",
             withMiddleware: true,
             clientType: ClientType.Chat);

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;
          
            var response = await mainAgent.RunAsync(input);
            Console.WriteLine();
            Green("Agent > ");
            GreenLine(response.ToString());

            Separator();
        }
    }
}
