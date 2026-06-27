using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class EndOfAppointmentExecutor() : Executor<FinalAppointment>("endOfAppointmentExecutor")
{
    public override async ValueTask HandleAsync(FinalAppointment message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("EndOfAppointmentExecutor started running");
        await Task.CompletedTask; 

        if(message.Status == FinalAppointmentStatus.CancelledStopHere)
            RedLine($"> Dear {message.Appointment.Name} - Sorry not to being able to help you - would love you come back soon. ");

        var appointmentDateString = GetDateTimeString(message);
        if (message.Status == FinalAppointmentStatus.CompletedStopHere)
            GreenLine($"> Dear {message.Appointment.Name} - Your appointment is confirmed for {appointmentDateString}. " +
                $"See you soon.");

        YellowBgLine("EndOfAppointmentExecutor finished running");
        Console.WriteLine(); 
    }

    private static string GetDateTimeString(FinalAppointment message)
    {
        var date = new DateTime(message.Appointment.Date.Year, message.Appointment.Date.Month, message.Appointment.Date.Day,
            message.Appointment.StartHour, 0, 0);

        return date.ToString("d-MMM hh t");
    }

}
