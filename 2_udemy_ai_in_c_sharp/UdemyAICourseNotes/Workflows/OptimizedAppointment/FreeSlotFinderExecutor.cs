using Microsoft.Agents.AI.Workflows;
using UdemyAICourseNotes.Model.Appointment;
using UdemyAICourseNotes.Services.Appointment;

namespace UdemyAICourseNotes.Workflows.OptimizedAppointment;

internal class FreeSlotFinderExecutor(AppointmentService appointmentService) :
    Executor<AppointmentLead, AppointmentLead>("freeSlotFinderExecutor")
{
    public override async ValueTask<AppointmentLead> HandleAsync(AppointmentLead message, 
        IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var availableSlots = new List<AppointmentSlot>(message.Slots.Length);

        foreach (var slot in message.Slots)
        {
            if(appointmentService.IsSlotFree(slot))
                availableSlots.Add(slot);
        }

        await Task.CompletedTask; 

        return message with { Slots = availableSlots.ToArray() };
    }
}
