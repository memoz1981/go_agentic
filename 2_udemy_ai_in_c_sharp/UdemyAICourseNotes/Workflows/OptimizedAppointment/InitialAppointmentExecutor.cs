using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using UdemyAICourseNotes.Samples._20;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class InitialAppointmentExecutor(AIAgent appointmentRecorderAgent) : Executor<string, InitialAppointmentSlim>("appointmentRecorderExecutor")
{
    public override async ValueTask<InitialAppointmentSlim> HandleAsync(string message,
        IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("InitialAppointmentExecutor started running"); 
        
        var result = await TakeAppointment(message, appointmentRecorderAgent);

        YellowBgLine("InitialAppointmentExecutor finished running");
        Console.WriteLine();
        return result; 
    }

    private static async Task<InitialAppointmentSlim> TakeAppointment(string message, AIAgent agent)
    {
        string input = null; 
        if (string.IsNullOrWhiteSpace(message) || message == _28_Workflows_Optimized.INITIAL_INPUT)
        {
            BlueLine("> Hello how can I help you?");
            Console.Write("> ");
            input = Console.ReadLine();
        }
        else
        {
            input = message;    
        }

        while (true)
        {
            var result = (await agent.RunAsync<InitialAppointment>(input)).Result;

            if (result.InitialAppointmentStatus != InitialAppointmentStatus.ClarificationsRequired)
            {
                return new InitialAppointmentSlim(result.InitialAppointmentStatus, result.FinalizedRequest);
            }

            BlueLine($"> {result.FurtherQuestionToUser}");
            Console.Write("> ");
            input = Console.ReadLine(); 
        }
    }
}
