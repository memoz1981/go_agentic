using HelloAgent.Extensions;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

const string EXIT = "exit";
const string SECRETS = "secrets_ai";
const string API_KEY = "api_key";
const string OPEN_AI_MODEL = "gpt-4o-mini"; 


var configuration = new ConfigurationBuilder().AddUserSecrets(SECRETS).Build();

var apiKey = configuration.GetSection(API_KEY).Value;

ArgumentException.ThrowIfNullOrEmpty(apiKey);

var agent = new OpenAIClient(apiKey)
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
    Console.WriteLine("Write a question: >");

    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
        break;

    Console.WriteLine("Agent > ");

    await foreach (var update in agent.RunStreamingAsync(input))
    {
        Console.WriteLine(update.ToString()); 
    }

    Console.WriteLine(); 
}




