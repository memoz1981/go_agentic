using Google.GenAI;
using UdemyAICourseNotes.Helpers;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._40_Gemini_Topics; 

internal class _41_List_Models : BaseSample
{
    public override string Description => "List GEMINI Base Models";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = new Client(apiKey: SecretsManager.GetApiKey(Enums.Clients.Gemini));

        var models = await client.Models.ListAsync();

        BlueLine("Below is the list of base models for Google Gemini"); 
        await foreach (var model in models)
        {
            Yellow($"{model.Name} ({model.DisplayName}) - ");
            Green(model.Description);
            Console.WriteLine(); 
        }
    }
}
