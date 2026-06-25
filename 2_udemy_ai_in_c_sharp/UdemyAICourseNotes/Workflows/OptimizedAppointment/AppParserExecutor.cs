using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output; 
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class AppParserExecutor(AIAgent appointmentParserAgent) : Executor<InitialAppointmentDto, AppointmentDto>("AppointmentParser")
{
    public override async ValueTask<AppointmentDto> HandleAsync(InitialAppointmentDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("Appointment parser parsing message...");
        var response = await appointmentParserAgent.RunAsync<AppointmentDto>(message.FinalizedRequest);

        GrayLine($"Appointment parser returned request for date {response.Result?.Date} with possible hour " +
            $"count {response.Result?.PossibleStartHours?.Length ?? 0} of possible options");

        return response.Result;
    }
}
