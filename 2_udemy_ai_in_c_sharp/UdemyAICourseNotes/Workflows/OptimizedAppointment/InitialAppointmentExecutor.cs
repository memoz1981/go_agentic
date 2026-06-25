using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output; 
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class InitialAppointmentExecutor(AIAgent appointmentRecorderAgent) : Executor<string, InitialAppointmentDto>("appointmentRecorderExecutor")
{
    public override async ValueTask<InitialAppointmentDto> HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
        => await TakeAppointment(appointmentRecorderAgent); 

    private static async Task<InitialAppointmentDto> TakeAppointment(AIAgent agent)
    {
        BlueLine("> Hello how can I help you?");
        Console.Write("> ");
        var input = Console.ReadLine(); 
        
        var result = (await agent.RunAsync<InitialAppointmentDto>(input)).Result;

        if (result.IsCancelled)
        {
            RedLine($"> {result.FurtherQuestionToUser}");
            return result;
        }

        GreenLine($"> {result.FurtherQuestionToUser}");
        var userInput = Console.ReadLine();

        while (true)
        {
            BlueLine($"> {result.FurtherQuestionToUser}");
            Console.Write("> ");
            input = Console.ReadLine(); 
            
            result = (await agent.RunAsync<InitialAppointmentDto>(userInput)).Result;

            if (result.IsFinal || result.IsCancelled)
            {
                return result;
            }
        }
    }
}
