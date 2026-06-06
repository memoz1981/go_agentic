using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Responses;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples._10; 

internal class _11_Code_Interpreter_Tool : BaseSample
{
    public override string Description => "Code interpreter tool";

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        //code interpreter client
        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);

        var agent = AgentClientFactory.GetAgent(
             openAIClient: client,
             model: Models.OpenAI.GPT_5_4,
             name: "agent",
             tools:
             [
                 new HostedCodeInterpreterTool()
             ],
             instructions: "You can make charts using you 'code_interpreter' tool",
             withMiddleware: true,
             clientType: ClientType.Response);

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var response = await agent.RunAsync(input);
            await DisplayChartFromAgentResponse(client, response); 

            Green("Agent > ");
            GreenLine(response.ToString());

            Separator();
        }
    }
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    private static async Task DisplayChartFromAgentResponse(OpenAIClient client, AgentResponse response)
    {
        var annotations = response.Messages
            .SelectMany(message => message.Contents)
            .SelectMany(content => content.Annotations ?? [])
            .ToList();

        foreach (var annotation in annotations)
        {
            if (annotation.RawRepresentation is not ContainerFileCitationMessageAnnotation containerFileCitation)
                continue;

            var containerClient = client.GetContainerClient();
            var fileContent = await containerClient.DownloadContainerFileAsync(containerFileCitation.ContainerId, containerFileCitation.FileId);

            string path = Path.Combine(Path.GetTempPath(), containerFileCitation.Filename);

            await File.WriteAllBytesAsync(path, fileContent.Value.ToArray());
            await Task.Factory.StartNew(() =>
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            });

        }
    }
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

}
