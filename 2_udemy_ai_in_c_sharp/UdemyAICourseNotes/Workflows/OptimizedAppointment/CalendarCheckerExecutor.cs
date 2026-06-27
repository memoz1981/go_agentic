using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using UdemyAICourseNotes.Services.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class CalendarCheckerExecutor(AppointmentService appointmentService) :
    Executor<AppointmentLead, AppointmentLead>("calendarCheckerExecutor")
{
    public override async ValueTask<AppointmentLead> HandleAsync(AppointmentLead message, 
        IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        YellowBgLine("CalendarCheckerExecutor started running");
        var availableSlots = new List<AppointmentSlot>(message.Slots.Length);

        foreach (var slot in message.Slots)
        {
            if (!appointmentService.IsSlotFree(slot, out var failureReason))
                continue; 

            availableSlots.Add(slot);
        }

        await Task.CompletedTask;

        YellowBgLine($"CalendarCheckerExecutor finished running with {availableSlots.Count} number of available slots");
        Console.WriteLine();
        return message with { Slots = availableSlots.ToArray() };
    }
}
