using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class AppParserExecutor(AIAgent appointmentParserAgent) 
    : Executor<InitialAppointmentSlim, AppointmentLead>("AppointmentParser")
{
    public override async ValueTask<AppointmentLead> HandleAsync(InitialAppointmentSlim message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("AppParserExecutor started running");
        var response = await appointmentParserAgent.RunAsync<AppointmentLead>(message.FinalizedRequest);

        YellowBgLine($"AppParserExecutor finished running with {response.Result.Slots.Length} possible slots");
        Console.WriteLine();
        return response.Result;
    }
}
