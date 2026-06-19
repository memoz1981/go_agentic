using Microsoft.Agents.AI.Workflows;
using static UdemyAICourseNotes.Helpers.Output;
using static UdemyAICourseNotes.Samples._20._25_Workflows_Intruduction;

namespace UdemyAICourseNotes.Workflows;

internal class AppointmentMakerExecutor(InMemoryAppointmentSchedule schedule) : Executor<AppointmentDto, AppointmentResultDto>("AppointmentMakerExecutor")
{
    private HashSet<string> _blackList = ["dave1", "dave2"];
    
    public override ValueTask<AppointmentResultDto> HandleAsync(AppointmentDto message, IWorkflowContext context, CancellationToken cancellationToken = default)
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
            return ValueTask.FromResult(new AppointmentResultDto(false, message));
        }

        if (_blackList.Contains(message.Name.ToLower().Trim()))
        {
            GrayLine("Couldn't book the slot - user is in black list");
            return ValueTask.FromResult(new AppointmentResultDto(false, message));
        }

        var addAppointmentDto = new AddAppointmentDto(message.Name, message.Date, message.PossibleStartHours.First(),
            message.Description, message.Phone);

        schedule.AddAppointment(addAppointmentDto); 
        GrayLine("Booked the slot");

        return ValueTask.FromResult(new AppointmentResultDto(true, message));
    }
}
