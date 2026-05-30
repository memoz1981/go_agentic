using Microsoft.Extensions.Configuration;
using UdemyAICourseNotes.Clients;

namespace UdemyAICourseNotes.Helpers; 

internal class SecretsManager
{
    private const string KEY = "githubModel"; 
    public static GithubModel GetGithubModel()
    {
        var configuration = (new ConfigurationBuilder().AddUserSecrets<SecretsManager>()).Build();

        var githubModel = configuration.GetRequiredSection(KEY).Get<GithubModel>();

        ArgumentNullException.ThrowIfNullOrWhiteSpace(githubModel.ApiKey); 
        ArgumentException.ThrowIfNullOrWhiteSpace(githubModel.Token);

        return githubModel; 
    }
}
