using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class AppointmentCheckerExecutor(InMemoryAppointmentSchedule schedule) : Executor<AppointmentDto, AppointmentDto>("AppointmentCheckerExecutor")
{
    public override ValueTask<AppointmentDto> HandleAsync(AppointmentDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("Checking the appointmentment schedule for empty slots...");
        var hours = schedule.ReturnAvailableSlots(message);

        GrayLine($"Checked the appointmentment schedule - returned {hours.Length} empty slots.");

        var filtered = message with { PossibleStartHours = hours };
        return ValueTask.FromResult(filtered); 
    }
}
