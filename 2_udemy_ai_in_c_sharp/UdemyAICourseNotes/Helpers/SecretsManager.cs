using Microsoft.Extensions.Configuration;
using UdemyAICourseNotes.Clients;

namespace UdemyAICourseNotes.Helpers; 

internal class SecretsManager
{
    private const string GITHUB_KEY = "github";
    private const string OPEN_AI_KEY = "openAI";

    public static GithubModel GetGithubModel()
    {
        var configuration = (new ConfigurationBuilder().AddUserSecrets<SecretsManager>()).Build();

        var githubModel = configuration.GetRequiredSection(GITHUB_KEY).Get<GithubModel>();

        ArgumentNullException.ThrowIfNullOrWhiteSpace(githubModel.ApiKey); 
        ArgumentException.ThrowIfNullOrWhiteSpace(githubModel.Token);

        return githubModel; 
    }

    public static OpenAIModel GetOpenAIModel()
    {
        var configuration = (new ConfigurationBuilder().AddUserSecrets<SecretsManager>()).Build();

        var githubModel = configuration.GetRequiredSection(OPEN_AI_KEY).Get<OpenAIModel>();

        ArgumentNullException.ThrowIfNullOrWhiteSpace(githubModel.ApiKey);

        return githubModel;
    }
}
