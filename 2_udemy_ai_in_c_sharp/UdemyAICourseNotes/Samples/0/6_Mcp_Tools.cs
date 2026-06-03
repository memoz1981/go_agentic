using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Enums;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Samples; 

internal class _6_Mcp_Tools : BaseSample
{
    public override string Description => "Using MCP (Model Context Protocol) tools - using Microsoft Learn";
    private const string MICROSOFT_MCP_URI = "https://learn.microsoft.com/api/mcp"; 

    public override async Task RunAsync()
    {
        Gray($"Running the sample for {Description}");
        Console.WriteLine();
        Console.WriteLine();

        await using var client = await McpClient.CreateAsync(new HttpClientTransport(
            new HttpClientTransportOptions() 
            { 
                Endpoint = new Uri(MICROSOFT_MCP_URI),
                TransportMode = HttpTransportMode.AutoDetect
            }));

        var tools = await client.ListToolsAsync(); //MCPClientTool

        var agentWithOutTools = AgentClientFactory
             .GetAgent(Enums.Clients.OpenAI, Models.OpenAI.GPT_5_4);

        var agentWithTools = AgentClientFactory
             .GetAgent(Enums.Clients.OpenAI, Models.OpenAI.GPT_5_4, tools: tools.Cast<AITool>().ToList());

        while (true)
        {
            Red("> ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.Equals(input, EXIT, StringComparison.InvariantCultureIgnoreCase))
                break;

            Console.WriteLine();

            var responseWithoutTools = await agentWithOutTools.RunAsync(input);
            Green("Agent without tools> ");
            GreenLine(responseWithoutTools.ToString());

            Console.WriteLine();

            var responseWithTools = await agentWithTools.RunAsync(input);
            Blue("Agent with tools> ");
            BlueLine(responseWithTools.ToString());

            Separator();
        }
    }
}
