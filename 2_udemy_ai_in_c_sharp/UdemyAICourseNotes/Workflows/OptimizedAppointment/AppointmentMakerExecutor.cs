using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using UdemyAICourseNotes.Services.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class AppointmentMakerExecutor(AppointmentService appointmentService)
    : Executor<SlotSelectionResult, AppointmentBookingResult>("appointmentMakerExecutor")
{
    public override async ValueTask<AppointmentBookingResult> HandleAsync(SlotSelectionResult message, 
        IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("AppointmentMakerExecutor started running");
        await Task.CompletedTask;

        var appointment = new Appointment(message.SelectedSlot.Date, message.SelectedSlot.StartHour,
            message.AppointmentLead.Name, message.AppointmentLead.Description, message.AppointmentLead.Phone);

        var result = appointmentService.AddAppointment(appointment);

        YellowBgLine($"AppointmentMakerExecutor finished running with booked {result.AppointmentBooked}");
        Console.WriteLine(); 
        return new AppointmentBookingResult(result.AppointmentBooked, result.FinalAppointment, result.FailureReason,
            message.AppointmentLead);
    }
}
