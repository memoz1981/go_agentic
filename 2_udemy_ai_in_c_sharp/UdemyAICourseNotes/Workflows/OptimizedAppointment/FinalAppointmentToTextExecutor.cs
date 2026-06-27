using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class FinalAppointmentToTextExecutor() : Executor<FinalAppointment, string>("finalAppointmentToTextExecutor")
{
    public override async ValueTask<string> HandleAsync(FinalAppointment message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("FinalAppointmentToTextExecutor started running");
        await Task.CompletedTask;

        YellowBgLine("FinalAppointmentToTextExecutor finished running");
        Console.WriteLine(); 

        return message.FollowUpRequest; 
    }
}
