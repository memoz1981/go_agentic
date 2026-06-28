using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UdemyAICourseNotes.Clients;
using UdemyAICourseNotes.Tools;

namespace UdemyAICourseNotes.Samples._60;

internal class _60_Dev_UI : BaseSample
{
    public _60_Dev_UI()
    {
        
    }


    public override string Description => "Dev UI"; 

    public override async Task RunAsync()
    {
        var builder = WebApplication.CreateBuilder();

        var client = AgentClientFactory.GetClient(Enums.Clients.OpenAI);

        var chatClient = client.GetChatClient(Enums.Models.OpenAI.GPT_5_4_MINI);

        builder.Services.AddChatClient(chatClient.AsIChatClient()); //You need to register a chat client for the dummy agents to use
        builder.Services.AddOpenAIResponses();
        builder.Services.AddOpenAIConversations();

        builder.Services.AddDevUI();

        builder.AddAIAgent("Comic Book Guy", "You are comic-book guy from The Simpsons")
            .WithAITool(AIFunctionFactory.Create(DateTimeTools.GetTodaysDate));

        var app = builder.Build();

        //Needed for DevUI to function 
        app.MapOpenAIResponses();
        app.MapOpenAIConversations();
        app.MapDevUI();

        await Task.CompletedTask; 

        app.Run();
    }
}
