using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class AppParserExecutor(AIAgent appointmentParserAgent) : Executor<InitialAppointmentSlim, AppointmentLead>("AppointmentParser")
{
    public override async ValueTask<AppointmentLead> HandleAsync(InitialAppointmentSlim message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("Appointment parser parsing message...");
        var response = await appointmentParserAgent.RunAsync<AppointmentLead>(message.FinalizedRequest);

        GrayLine($"Appointment parser returned request with {response.Result.Slots.Length} possible slots");

        return response.Result;
    }
}
