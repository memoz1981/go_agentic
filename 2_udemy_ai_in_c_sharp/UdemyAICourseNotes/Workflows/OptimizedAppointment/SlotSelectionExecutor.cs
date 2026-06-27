using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using System.Text.Json;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class SlotSelectionExecutor(AIAgent slotSelectionAgent) : Executor<AppointmentLead, SlotSelectionResult>("slotSelectionExecutor")
{
    public override async ValueTask<SlotSelectionResult> HandleAsync(AppointmentLead message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("SlotSelectionExecutor started running");

        var result = await SelectSlot(message, slotSelectionAgent);

        YellowBgLine("SlotSelectionExecutor finished running");
        Console.WriteLine(); 

        return new SlotSelectionResult(result.SlotSelectionStatus, message, result.SelectedSlot, 
            result.AlternativeDateRequestDetails);
    }

    private static async Task<SlotSelectionResultSlim> SelectSlot(AppointmentLead appointmentLead, AIAgent agent)
    {
        var serialized = JsonSerializer.Serialize(appointmentLead);
        var input = $"Following details are available - available slots are found under slots: {serialized}";
        while (true)
        {
            var resString = (await agent.RunAsync(input)).ToString();
            var result = (await agent.RunAsync<SlotSelectionResultSlim>(input)).Result;

            if (result.SlotSelectionStatus != SlotSelectionStatus.ClarificationsRequired)
            {
                return result;
            }

            BlueLine($"> {result.FurtherQuestionsToUser}");
            Console.Write("> ");
            input = Console.ReadLine();
        }
    }
}
