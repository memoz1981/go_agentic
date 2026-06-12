using Microsoft.Extensions.AI;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using UdemyAICourseNotes.Extensions;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10;

internal class _18_Multi_Modal : BaseSample
{
    public override string Description => "Multi modal messages";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        var client = AgentClientFactory
             .GetClient(Enums.Clients.OpenAI);

        var agent = AgentClientFactory.GetAgent(
            openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "agent",
             instructions: "You are a chess grand master - you need to find solutions to provided chess puzzles. Also you are a C# developer at the same time",
             withMiddleware: true,
             clientType: ClientType.Chat);

        //passing URI content (link to an image)
        var chessPuzzleUri = "https://chessify.me/media/uploads/dubov-sarin.jpg"; 
        RedLine($"> Passed URI to LLM to solve chess puzzle: {chessPuzzleUri}");
        var chatMessageUri = new ChatMessage(ChatRole.User,
            [
                new TextContent("White moves and to mate in 5 moves (or less) - only give the forcing line, no description"),
                new UriContent(chessPuzzleUri)
            ]);

        var responseUri = await agent.RunAsync(chatMessageUri);

        Green("LLM (URI): > ");
        GreenLine(responseUri.ToString());
        GreenLine(responseUri.Usage.Counts());

        Console.WriteLine();

        //passing image as base64 string
        var imagePath = Path.Combine(Path.GetTempPath(), "dubov-sarin.jpg");
        var base64String = Convert.ToBase64String(File.ReadAllBytes(imagePath));
        var base64Uri = $"data:image/jpeg;base64,{base64String}"; 

        RedLine($"> Passed base64 string to LLM to solve chess puzzle: {chessPuzzleUri}");
        var chatMessageBase64String = new ChatMessage(ChatRole.User,
            [
                new TextContent("White moves and to mate in 5 moves (or less) - only give the forcing line, no description"),
                new DataContent(base64Uri, "image/jpeg")
            ]);

        var responseBase64String = await agent.RunAsync(chatMessageBase64String);

        Blue("LLM (Base 64 string): > ");
        BlueLine(responseBase64String.ToString());
        BlueLine(responseBase64String.Usage.Counts());

        Console.WriteLine();

        //passing image as byte array
        var byteArray = File.ReadAllBytes(imagePath).AsMemory();

        RedLine($"> Passed byteArray to LLM to solve chess puzzle: {chessPuzzleUri}");
        var chatMessageByteArray = new ChatMessage(ChatRole.User,
            [
                new TextContent("White moves and to mate in 5 moves (or less) - only give the forcing line, no description"),
                new DataContent(byteArray, "image/jpeg")
            ]);

        var responseByteArray = await agent.RunAsync(chatMessageByteArray);

        Yellow("LLM (Byte array): > ");
        YellowLine(responseByteArray.ToString());
        YellowLine(responseByteArray.Usage.Counts());

        Console.WriteLine();

        //passing image as byte array
        var csFilePath = Path.Combine(Path.GetTempPath(), "18_Multi_Modal.cs");
        var byteArrayCsFile = File.ReadAllBytes(csFilePath).AsMemory();

        RedLine($"> Passed current C# class file:");
        var chatMessageCSharpFile = new ChatMessage(ChatRole.User,
            [
                new TextContent("How many console outputs are written in the attached file"),
                new DataContent(byteArrayCsFile, "text/plain")
            ]);

        var responseCSharp = await agent.RunAsync(chatMessageCSharpFile);

        Magenta("LLM (Byte array): > ");
        MagentaLine(responseCSharp.ToString());
        MagentaLine(responseCSharp.Usage.Counts());

        Separator();
    }
}
