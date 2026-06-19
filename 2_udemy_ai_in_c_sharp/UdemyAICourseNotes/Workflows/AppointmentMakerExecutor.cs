using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class AppointmentMakerExecutor(InMemoryAppointmentSchedule schedule) : Executor<AppointmentDto, bool>("AppointmentMakerExecutor")
{
    public override ValueTask<bool> HandleAsync(AppointmentDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        GrayLine("Booking slot...");
        if (message is null
            || message.PossibleStartHours is null or []
            || message.PossibleStartHours.Length != 1
            || string.IsNullOrWhiteSpace(message.Name)
            || string.IsNullOrWhiteSpace(message.Phone)
            || string.IsNullOrWhiteSpace(message.Description))
        {
            GrayLine("Couldn't book the slot.");
            return ValueTask.FromResult(false);
        }

        var addAppointmentDto = new AddAppointmentDto(message.Name, message.Date, message.PossibleStartHours.First(),
            message.Description, message.Phone);

        schedule.AddAppointment(addAppointmentDto); 
        GrayLine("Booked the slot");

        return ValueTask.FromResult(true);
    }
}
