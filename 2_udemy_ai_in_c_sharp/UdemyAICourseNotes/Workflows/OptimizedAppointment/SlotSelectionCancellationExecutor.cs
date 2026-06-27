using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class SlotSelectionCancellationExecutor() : Executor<SlotSelectionResult>("slotSelectionCancellationExecutor")
{
    public override async ValueTask HandleAsync(SlotSelectionResult message, IWorkflowContext context, 
        CancellationToken cancellationToken = default)
    {
        YellowBgLine("SlotSelectionCancellationExecutor started running");
        
        RedLine($"> The request is cancelled: - would love you come back soon. ");

        await Task.CompletedTask;

        YellowBgLine("SlotSelectionCancellationExecutor finished running");
        Console.WriteLine(); 
    }
}
