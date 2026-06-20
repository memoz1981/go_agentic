using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows;

internal class InitialTalkExecutor(AIAgent agent, string name) : Executor<string, string>(name)
{
    private const string INSTRUCTIONS = $"Work with the user to gather following appoinment information: " +
        $"Name: " +
        $"Phone number" +
        $"Date of appointment (single date)" +
        $"Possible hours (like 10 to 18 or after noon, evening etc.) " +
        $"Description - what is the appointment for" +
        $"All data should be provided, keep your questions short, precise, pretty - " +
        $"first need to welcome the user - and don't ask again the information provided -" +
        $"for example say now we have your name, need following additional details";

    public override async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("InitialTalkExecutor agent executing...");

        var session = await agent.CreateSessionAsync();
        while (true)
        {
            var response = await agent.RunAsync($"{INSTRUCTIONS}", session);

            BlueLine($">{response.ToString()}");
            Blue(">");
            var input = Console.ReadLine();

            var oneSlotSelectedResponse = await agent.RunAsync<bool>($"Confirm based on the provided input if all information is provided" +
                $"and there's only one date selected: {input}", session);

            if (oneSlotSelectedResponse.Result)
            {
                var selectionResponse = await agent.RunAsync($"Summarize user appointment details in short pretty way", session);

                GrayLine("InitialTalkExecutor agent executed...");
                return selectionResponse.ToString();
            }
        }
    }

    private record Response(bool RequestComplete, string ResponseMessage); 
}
