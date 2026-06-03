using Microsoft.Extensions.Configuration;
using UdemyAICourseNotes.Clients;

namespace UdemyAICourseNotes.Helpers; 

internal class SecretsManager
{
    private const string API_KEYS = "ApiKeys";

    public static string GetApiKey(Enums.Clients client)
    {
        var configuration = (new ConfigurationBuilder().AddUserSecrets<SecretsManager>()).Build();

        var apiKeys = configuration.GetRequiredSection(API_KEYS).Get<ApiKeys>();

        return client switch
        {
            Enums.Clients.OpenAI => apiKeys.OpenAI,
            Enums.Clients.Github => apiKeys.Github,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
