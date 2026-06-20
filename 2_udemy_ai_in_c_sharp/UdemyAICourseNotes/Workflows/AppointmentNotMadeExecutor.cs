using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System.Text.Json;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class AppointmentNotMadeExecutor(AIAgent agent) : Executor<AppointmentResultDto>("AppointmentNotMadeExecutor")
{
    public override async ValueTask HandleAsync(AppointmentResultDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("AppointmentNotMadeExecutor agent executing...");

        var response = await agent.RunAsync($"Provide confirmation to user that no slot found for the provided info, " +
            $"repeating the date, time, name and description of the request {JsonSerializer.Serialize(message.Appointment)}");
        RedLine($"> {response.ToString()}");

        GrayLine("AppointmentNotMadeExecutor agent executed...");
    }
}

internal class AppointmentNotMadeExecutor2(AIAgent agent) : Executor<AppointmentDto>("AppointmentNotMadeExecutor2")
{
    public override async ValueTask HandleAsync(AppointmentDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("AppointmentNotMadeExecutor2 agent executing...");

        var response = await agent.RunAsync($"Provide confirmation to user that no slot found for the provided info, " +
            $"repeating the date, time, name, description, phone number: {JsonSerializer.Serialize(message)}");
        RedLine($"> {response.ToString()}");

        GrayLine("AppointmentNotMadeExecutor2 agent executed...");
    }
}
