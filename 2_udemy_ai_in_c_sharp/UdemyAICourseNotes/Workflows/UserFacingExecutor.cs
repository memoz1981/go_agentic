using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows;

internal class UserFacingExecutor(AIAgent agent) : Executor<string, string>("User facing")
{
    private const string INSTRUCTIONS = @"You are a polite user facing agent - ask users all the information
that's required to collect if the instruction is to collect data - as soon as the data is ready - repeat the 
information requested to user - and then return that info as string structured in the way instructed. 
If you are providing output/feedback to user - provide it in brief polite way - don't ask for data input,
just tell what you have to say and return. You will return true if the request is completed otherwise false to 
re-iterate both for input and output (for output you will generally return true directly as we don't expect
any response from the user...";
    
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
                return response.Result.FullRequestText;
            }

            GreenLine("")
            var input = 

            //instead of this we should also return status signals like warning etc.
            if (iterationCount++ > 10)
                throw new InvalidOperationException("User is not ready yet...");
        }
    }

    private record Response(bool RequestComplete, string FullRequestText); 
}
