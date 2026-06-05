using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10;

internal class _12_Structured_Output : BaseSample
{
    public override string Description => "Structured Output";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        //astronomy agent
        var agent = AgentClientFactory
             .GetAgent(
             client: Enums.Clients.Github,
             model: Models.OpenAI.GPT_4o_MINI,
             name: "agent",
             instructions: "You are an IMDB Films expert.",
             withMiddleware: true,
             clientType: ClientType.Chat);

        var question = "List the top 3 best movies according to IMDB";

        Red($"> {question}");

        Console.WriteLine();

        GrayLine("Raw result");

        var responseRaw = await agent.RunAsync(question);

        Green("Agent > ");
        GreenLine(responseRaw.ToString());

        Console.WriteLine();

        GrayLine("Structured output");

        var results = await agent.RunAsync<MovieResult>(question);

        int index = 0; 
        foreach (var movie in results?.Result?.Movies ?? [])
        {
            BlueLine($"{index++} - {movie.ToString()}"); 
        }

        Separator();
    }

    private class MovieResult
    {
        public required List<Movie> Movies { get; set; }
    }

    private class Movie
    {
        public required string Name { get; set; }
        public required string Director { get; set; }
        public required int Year { get; set; }
        public required double Imdb { get; set; }

        public override string ToString()
            => $"{Name} ({Year}, by {Director}, YearOfRelease, imdb: {Imdb})";
    }
}
