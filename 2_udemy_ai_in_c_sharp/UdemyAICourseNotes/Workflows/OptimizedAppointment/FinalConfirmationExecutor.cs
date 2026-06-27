using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System.Text.Json;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class FinalConfirmationExecutor(AIAgent finalConfirmationAgent) 
    : Executor<AppointmentBookingResult, FinalAppointment>("finalConfirmationExecutor")
{
    public override async ValueTask<FinalAppointment> HandleAsync(AppointmentBookingResult message, IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        YellowBgLine("FinalConfirmationExecutor started running");
        var result = await ConfirmWithUser(message, finalConfirmationAgent);

        YellowBgLine("FinalConfirmationExecutor finished running");

        return new FinalAppointment(result.Status, message.FinalAppointment, result.FollowUpRequest);
    }

    private static async Task<FinalAppointmentSlim> ConfirmWithUser(AppointmentBookingResult bookingResult, AIAgent agent)
    {
        var serialized = JsonSerializer.Serialize(bookingResult);
        var input = $"We tried to book apppointment which is as follows: {serialized} - check with the customer";
        while (true)
        {
            var result = (await agent.RunAsync<FinalAppointmentSlim>(input)).Result;

            if (result.Status != FinalAppointmentStatus.ClarificationsRequired)
            {
                return result;
            }

            BlueLine($"> {result.FurtherQuestionsToUser}");
            Console.Write("> ");
            input = Console.ReadLine();
        }
    }
}
