using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.VectorRepo;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10; 

internal class _15_RAG_VectorStore : BaseSample
{
    public override string Description => "RAG (Retrieval-Augmented Generation) - Vector Store";

    List<SimpleTextVector> vectorList = 
        [
            new SimpleTextVector("The WIFI password at office is '12345678'"),
            new SimpleTextVector("There are 10 men and 20 women working in our office"),
            new SimpleTextVector("Our office opens at 8 am and closes at 10 pm"),
            new SimpleTextVector("Our office is located in the city center in Neftchilar avenue"),
            new SimpleTextVector("Our internet connection uses fiber optic internet"),
            new SimpleTextVector("Cat's eyes are green...")
        ];

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);
        var embeddingAgent = AgentClientFactory.GetEmbeddingGenerator(client,
            Models.OpenAIEmbedding.LARGE_3);

        var mainAgent = AgentClientFactory
             .GetAgent(
             openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "agent",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var repo = new SqlLiteVectorRepo<SimpleTextVector>(embeddingAgent);

        Console.WriteLine(); 
        RedLine("Should I reload the data? (Y/N)");
        var shouldReload = Console.ReadLine();
        var collection = await repo.GetCollection("office");

        if (shouldReload == "Y")
        {
            await repo.DeleteCollection("office");

            collection = await repo.GetCollection("office"); 
            await repo.Insert(collection, vectorList);
        }
        else if (shouldReload == "N")
        {
            //do nothing really
        }
        else
            throw new ArgumentException();

        //writes raw data - importantly which doesn't include the actual vector...
        await foreach (var data in repo.GetAll(collection))
        {
            GrayLine($"{data.Id}, {data.Text}, {data.Vector}");
        }

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var chatMessageList = new List<ChatMessage>();
            chatMessageList.Add(new ChatMessage(ChatRole.User, input));
            
            //write the vector search results and add to list to send to LLM
            BlueLine("Getting all elements from the vector store:");
            int index = 1; 
            await foreach (var vectorSearchResult in repo.Search(collection, input).OrderByDescending(r => r.Score))
            {
                BlueLine($"Text: {vectorSearchResult.Record.Text}, Score: {vectorSearchResult.Score}");

                var chatMessage = $"This is the {index}th relevant information: {vectorSearchResult.Record.Text}";
                chatMessageList.Add(new ChatMessage(ChatRole.User, chatMessage)); 
            }
            Console.WriteLine(); 

            var response = await mainAgent.RunAsync(chatMessageList);
            Console.WriteLine();
            Green("Agent > ");
            GreenLine(response.ToString());

            Separator();
        }
    }
}
