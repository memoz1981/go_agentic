using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10;

internal class _14_RAG_Generation : BaseSample
{
    public override string Description => "RAG (Retrieval-Augmented Generation) - Generation";

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

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            //input generation using GenerateAsync method (returns Embedding<float>)
            var embeddingInput1 = await embeddingAgent.GenerateAsync(input);
            embeddingInput1.Print();

            //input generation using GenerateVectorAsync (returns ReadOnlyMemory<float>)
            var embeddingInput2 = await embeddingAgent.GenerateVectorAsync(input);
            embeddingInput2.Print();

            //check if the two formats return "same data"
            var isInputSame = embeddingInput1.IsSame(embeddingInput2);
            RedLine($"Generated input data is same? {isInputSame}"); 

            var response = await mainAgent.RunAsync(input);
            Console.WriteLine();
            Green("Agent > ");
            GreenLine(response.ToString());

            var embeddingOutput1 = await embeddingAgent.GenerateAsync(input);
            embeddingOutput1.Print();

            var embeddingOutput2 = await embeddingAgent.GenerateVectorAsync(input);
            embeddingOutput2.Print();

            var isOutputSame = embeddingOutput1.IsSame(embeddingOutput2);
            RedLine($"Generated output data is same? {isOutputSame}");

            //call vector for input 10 times and check if same
            GrayLine("Calling generate async for input 10 times and check if all same?");
            var inputVectors = new List<Embedding<float>>();
            for (int i = 0; i < 10; i++)
            {
                inputVectors.Add(await embeddingAgent.GenerateAsync(input));
            }
            inputVectors.IsSame();

            //call vector for output 10 times and check if same
            GrayLine("Calling generate async for output 10 times and check if all same?");
            var outputVectors = new List<Embedding<float>>();
            for (int i = 0; i < 10; i++)
            {
                outputVectors.Add(await embeddingAgent.GenerateAsync(response.ToString()));
            }
            outputVectors.IsSame();

            Separator();
        }
    }
}
