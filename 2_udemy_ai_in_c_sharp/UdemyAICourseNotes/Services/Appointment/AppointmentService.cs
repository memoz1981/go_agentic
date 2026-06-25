using UdemyAICourseNotes.Model.Appointment;

namespace UdemyAICourseNotes.Services.Appointment;

internal class AppointmentService
{
    private IDictionary<AppointmentSlot, Model.Appointment.Appointment> _appointments; 
    
    public bool IsSlotFree(AppointmentSlot slot)
    {
        if (slot.StartHour < 9 || slot.StartHour > 17 || slot.Date.DayOfWeek == DayOfWeek.Sunday)
            return false;

        return !_appointments.ContainsKey(slot);
    }

    public bool AddAppointment(Model.Appointment.Appointment appointment)
    {
        var slot = new AppointmentSlot(appointment.Date, appointment.StartHour);

        if (!IsSlotFree(slot))
            return false;

        _appointments[slot] = appointment;

        return true; 
    }

    public bool TryGetAppointment(AppointmentSlot slot, out Model.Appointment.Appointment appointment)
        => _appointments.TryGetValue(slot, out appointment);
}
