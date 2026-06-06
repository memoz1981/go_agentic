using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Helpers;

namespace UdemyAICourseNotes.Clients;

internal class AgentClientFactory
{
    private const string DEFAULT_NAME = "assistant";
    private const string GITHUB_ENDPPOINT = "https://models.github.ai/inference";
    private const string DEFAULT_INSTRUCTIONS =
        """
            You are an AI assistant with access to tools.

            BEHAVIOR:
            - Be concise but thorough
            - Use tools when appropriate instead of making up answers
            - Always respond in the user's language

            IMPORTANT:
            - For calculations, ALWAYS use the calculator tool
            - Never make up data: use tools to get real information
            """;

    public static AIAgent GetAgent(
        Enums.Clients client,
        string model,
        string name = DEFAULT_NAME,
        string instructions = DEFAULT_INSTRUCTIONS,
        IList<AITool> tools = null,
        bool withMiddleware = false,
        ClientType clientType = ClientType.Chat)
    {
        var openAIClient = GetClient(client);

        return GetAgent(openAIClient, model, name, instructions, tools, withMiddleware, clientType);
    }

    public static OpenAIClient GetClient(
        Enums.Clients client,
        HttpClient httpClient = null)
    {
        var options = (client, httpClient) switch
        {
            (Enums.Clients.Github, null) => new OpenAIClientOptions() { Endpoint = new Uri(GITHUB_ENDPPOINT) },
            (Enums.Clients.Github, _) => new OpenAIClientOptions() { Endpoint = new Uri(GITHUB_ENDPPOINT), Transport = new HttpClientPipelineTransport(httpClient) },
            (Enums.Clients.OpenAI, null) => new OpenAIClientOptions(),
            (Enums.Clients.OpenAI, _) => new OpenAIClientOptions() { Transport = new HttpClientPipelineTransport(httpClient) },
            _ => throw new ArgumentOutOfRangeException()
        };
        
        if (client == Enums.Clients.Github)
        {
            var apiKey = SecretsManager.GetApiKey(Enums.Clients.Github);

            return new OpenAIClient(
               new ApiKeyCredential(apiKey), options);
        }

        if (client == Enums.Clients.OpenAI)
        {
            var apiKey = SecretsManager.GetApiKey(Enums.Clients.OpenAI);

            return new OpenAIClient(
               new ApiKeyCredential(apiKey), options); 
        }

        throw new ArgumentOutOfRangeException(); 
    }

    public static AIAgent GetAgent(
        OpenAIClient openAIClient,
        string model,
        string name = DEFAULT_NAME,
        string instructions = DEFAULT_INSTRUCTIONS,
        IList<AITool> tools = null,
        bool withMiddleware = false,
        ClientType clientType = ClientType.Chat)
    {
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var agent = clientType switch
        {
            ClientType.Chat => openAIClient
                   .GetChatClient(model)
                   .AsAIAgent(
                       name: name,
                       instructions: instructions,
                       tools: tools),
            ClientType.Response => openAIClient
                   .GetResponsesClient()
                   .AsAIAgent(
                       name: name,
                       model: model,
                       instructions: instructions,
                       tools: tools),
            _ => throw new ArgumentException(nameof(clientType))
        };
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        return agent
                .AsBuilder()
                .Use(Middleware)
                .Build();
    }

    private static async ValueTask<object> Middleware(AIAgent agent, FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object>> next, CancellationToken cancellationToken)
    {
        StringBuilder toolDetails = new();
        toolDetails.Append($"- Tool Call: '{context.Function.Name}'");
        if (context.Arguments.Count > 0)
        {
            toolDetails.Append($" (Args: {string.Join(",", context.Arguments.Select(x => $"[{x.Key} = {x.Value}]"))}");
        }

        Output.YellowLine(toolDetails.ToString());

        //if (context.Function.Name != "GetTodaysDate")
        //{
        //    return new DateTime(2030, 1, 1);
        //}

        return await next.Invoke(context, cancellationToken);
    }
}
