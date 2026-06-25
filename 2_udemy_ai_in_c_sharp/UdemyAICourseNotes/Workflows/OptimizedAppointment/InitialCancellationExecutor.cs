using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class InitialCancellationExecutor() : Executor<InitialAppointmentSlim>("cancellationExecutor")
{
    public override async ValueTask HandleAsync(InitialAppointmentSlim message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var feedback = message.InitialAppointmentStatus == InitialAppointmentStatus.RequestCancelled ?
            "by your request" : message.InitialAppointmentStatus == InitialAppointmentStatus.CouldNotFinalize ?
            "wasn't possible to conclude " : throw new ArgumentException(); 
        RedLine($"> The request is cancelled: {feedback} - would love you come back soon. ");

        await Task.CompletedTask; 
    }
}
