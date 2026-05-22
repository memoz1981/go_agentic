using HelloAgent.Extensions;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Net;

const string EXIT = "exit";
const string SECRETS = "secrets_ai";
const string API_KEY = "github_ai_token";
const string OPEN_AI_MODEL = "gpt-4o-mini";
const string OPEN_AI_ENDPPOINT = "https://models.github.ai/inference";


var configuration = new ConfigurationBuilder().AddUserSecrets(SECRETS).Build();

var token = configuration.GetSection(API_KEY)?.Value;

ArgumentException.ThrowIfNullOrEmpty(token);

var agent = new OpenAIClient(new ApiKeyCredential(token),
    new OpenAIClientOptions { Endpoint = new Uri(OPEN_AI_ENDPPOINT) })
    .GetChatClient(OPEN_AI_MODEL)
    .AsAIAgent(
        name: "Assistant",
        instructions: """
            You are a friendly and knowledgeable AI assistant.
            Be concise but thorough. If you don't know something, admit it.
            Always respond in the user's language.
            """);

ArgumentException.ThrowIfNullOrEmpty(agent?.Name);

agent.PrintAgent();

while (true)
{
    Console.Write("Write a question: > ");
    

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
        break;
    Console.WriteLine();
    Console.Write("Agent > ");

    await foreach (var update in agent.RunStreamingAsync(input))
    {
        Console.Write(update.ToString()); 
    }

    Console.WriteLine();

    Console.Write("---------------------------------------------------------------");

    Console.WriteLine("\n");
}




