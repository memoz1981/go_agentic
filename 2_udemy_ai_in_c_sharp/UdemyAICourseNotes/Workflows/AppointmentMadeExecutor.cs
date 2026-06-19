using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System.Text.Json;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class AppointmentMadeExecutor(AIAgent agent, string name) : Executor<AppointmentResultDto>(name)
{
    public override async ValueTask HandleAsync(AppointmentResultDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("AppointmentMadeExecutor agent executing...");
        
        var response = await agent.RunAsync($"Provide confirmation to user that the appointment made, " +
            $"repeating the date, time, name, description, phone number: {JsonSerializer.Serialize(message.Appointment)}");
        GreenLine($"> {response.ToString()}");

        GrayLine("AppointmentMadeExecutor agent executed...");
    }
}
