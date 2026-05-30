using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
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
        IList<AITool> tools = null)
    {
        return client switch
        {
            Enums.Clients.Github => GetGithubClient(model, name, instructions, tools),
            _ => throw new ArgumentException(nameof(client))
        };
    }

    private static AIAgent GetGithubClient(string model, string name, string instructions, IList <AITool> tools = null)
    {
        var githubModel = SecretsManager.GetGithubModel();
        
        return new OpenAIClient(
            new ApiKeyCredential(githubModel.Token),
            new OpenAIClientOptions 
            { 
                Endpoint = new Uri(GITHUB_ENDPPOINT) 
            })
            .GetChatClient(model)
            .AsAIAgent(
                name: name, 
                instructions: instructions, 
                tools: tools); 
    }
}
