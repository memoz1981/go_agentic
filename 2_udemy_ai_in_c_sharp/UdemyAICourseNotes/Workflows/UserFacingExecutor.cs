using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows;

internal class UserFacingExecutor(AIAgent agent, string name) : Executor<string, string>(name)
{
    private const string INSTRUCTIONS = @"You are a polite user facing agent:
1) If the instruction is to ask user some data: 
- ask users all the information that's required and not provided yet - in this case return false, text 
to show the additional information required. 
- If the provided info is full - return true, full information

If you are providing output/feedback to user - provide it in brief polite way - don't ask for data input,
just tell what you have to say and return true, empty string. ";
    
    public override async ValueTask<string> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("Lobby agent working...");
        var session = await agent.CreateSessionAsync();
        int iterationCount = 0; 
        while (true)
        {
            var response = await agent.RunAsync<Response>($"{INSTRUCTIONS}: {message}", session);

            if (response.Result.RequestComplete)
            {
                GrayLine("Lobby agent complete...");
                return response.Result.ResponseMessage;
            }

            GreenLine($"> {response.Result.ResponseMessage}");
            Green(">");
            message = Console.ReadLine(); 

            //instead of this we should also return status signals like warning etc.
            if (iterationCount++ > 10)
                throw new InvalidOperationException("User is not ready yet...");
        }
    }

    private record Response(bool RequestComplete, string ResponseMessage); 
}
