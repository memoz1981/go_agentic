using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class SlotSelectionToTextConversionExecutor() 
    : Executor<SlotSelectionResult, string>("slotSelectionToTextConversionExecutor")
{
    public override async ValueTask<string> HandleAsync(SlotSelectionResult message, IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        YellowBgLine("SlotSelectionToTextConversionExecutor started running");

        await Task.CompletedTask;

        YellowBgLine("SlotSelectionToTextConversionExecutor started running");

        return message.AlternativeDateRequestDetails;
    }
}
