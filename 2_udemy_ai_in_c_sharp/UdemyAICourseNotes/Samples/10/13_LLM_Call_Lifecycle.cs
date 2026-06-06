using Microsoft.Extensions.AI;
using System.Text.Json;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Tools;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10;

internal class _13_LLM_Call_Lifecycle : BaseSample
{
    public override string Description => "Structured Output";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();
        var customHttpClientHandler = new CustomHttpClientHandler(); 
        using var httpClient = new HttpClient(customHttpClientHandler);

        var openAIClient = AgentClientFactory.GetClient(Enums.Clients.OpenAI, httpClient); 
        var agent = AgentClientFactory
             .GetAgent(
             openAIClient: openAIClient,
             model: Models.OpenAI.GPT_5_4_MINI,
             name: "agent",
             tools:
             [
                 AIFunctionFactory.Create(DateTimeTools.GetTodaysDate, "getDate", "use to get today's date"),
                 AIFunctionFactory.Create(FakeWeatherTool.GetWeather, "getWeather", "use to get weather forecast")
             ],
             instructions: "Use 'getDate' to get today's date, and 'useWeather' to get weather forecast",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var question = "Return the weather for today";

        Red($"> {question}");

        Console.WriteLine();

        var responseRaw = await agent.RunAsync(question);

        Green("Agent > ");
        GreenLine(responseRaw.ToString());

        Console.WriteLine();

        Separator();
    }

    private class CustomHttpClientHandler : HttpClientHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string requestString = await request.Content?.ReadAsStringAsync(cancellationToken)!;
            Green($"Raw Request ({request.RequestUri})");
            Gray(MakePretty(requestString));
            Separator();
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            string responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            Green("Raw Response");
            Gray(MakePretty(responseString));
            Separator();
            return response;
        }

        private string MakePretty(string input)
        {
            try
            {
                JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(input);
                return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception e)
            {
                return input;
            }
        }
    }
}
