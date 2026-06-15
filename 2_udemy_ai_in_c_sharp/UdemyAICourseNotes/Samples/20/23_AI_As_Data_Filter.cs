using System.Text.Json;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Services.Filters;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Samples._20;

internal class _23_AI_As_Data_Filter : BaseSample
{
    public override string Description => "AI As Data Filter";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);

        var normalAgent = AgentClientFactory
            .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_NANO,
            name: "normalAgent",
            instructions: "Only answer requests related to the books",
            withMiddleware: true
            );

        var filterAgent = AgentClientFactory
            .GetAgent(
            openAIClient: client,
            model: Models.OpenAI.GPT_5_4_NANO,
            instructions: "Only answer requests related to the books - return filter output for the books",
            name: "filterAgent",
            withMiddleware: true
            );

        var books = JsonSerializer.Deserialize<List<Book>>(booksJson);

        var bookFilterService = new BookFilterService(books);
        GrayLine("All Books:");

        foreach (var book in bookFilterService.GetAll())
        {
            GrayLine(book.ToString()); 
        }

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            //Normal Agent
            var response = await normalAgent.RunAsync($"{input}, books {booksJson}");
            Green("Normal Agent response: > ");
            GreenLine(response.ToString());
            Console.WriteLine();

            //Filter Agent
            var filterRespononse = await filterAgent.RunAsync<BookFilter[]>(input);
            BlueLine("Filter Agent response (filters): > ");
            foreach (var filter in filterRespononse.Result)
            {
                BlueLine(filter.ToString());
            }
            BlueLine("********************"); 
            var filteredBooks = bookFilterService.Filter(filterRespononse.Result);
            BlueLine("Filtered Output as per the filters provided by the filter agent:");
            foreach (var book in filteredBooks)
                BlueLine(book.ToString());

            Separator();
        }
    }

    internal record struct Book(string Title, int YearOfRelease, string Author,
        string Genre, string Synopsis)
    {
        public override string ToString()
            => $"{Title} - {YearOfRelease} - {Author} - {Genre} - {Synopsis}";
    }

    internal record BookFilter(BookField Field, Operation Operation, string Value)
    {
        public override string ToString()
            => $"{Field} - {Operation} - {Value}";
    }

    internal enum BookField { Title, YearOfRelease, Author, Genre, Synopsis }

    internal enum Operation { Equals, NotEquals, StartsWith, EndsWith, Contains, GreaterThan, GreaterThanOrEqual,
        LessThan, LessThanOrEqual, Regex};

    private readonly string booksJson = @"
[{
        ""Title"": ""Don Quixote"",
        ""YearOfRelease"": 1605,
        ""Author"": ""Miguel de Cervantes"",
        ""Genre"": ""Classic"",
        ""Synopsis"": ""The adventures of a noble who loses his mind and decides to become a knight-errant.""
    },
    {
        ""Title"": ""Alice's Adventures in Wonderland"",
        ""YearOfRelease"": 1865,
        ""Author"": ""Lewis Carroll"",
        ""Genre"": ""Fantasy"",
        ""Synopsis"": ""A girl named Alice falls through a rabbit hole into a fantasy world.""
    },
    {
        ""Title"": ""The Adventures of Huckleberry Finn"",
        ""YearOfRelease"": 1884,
        ""Author"": ""Mark Twain"",
        ""Genre"": ""Adventure"",
        ""Synopsis"": ""A young boy and a runaway slave travel down the Mississippi River.""
    }]";
}
