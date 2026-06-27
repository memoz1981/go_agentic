using UdemyAICourseNotes.Model.Appointment;
using static UdemyAICourseNotes.Helpers.Output; 

namespace UdemyAICourseNotes.Services.Appointment;

internal class AppointmentService
{
    private Dictionary<DateOnly, Dictionary<AppointmentSlot, Model.Appointment.Appointment>> _appointments = new();
    private HashSet<string> _blackList = ["dave"]; 
    
    public bool IsSlotFree(AppointmentSlot slot, out string failureReason)
    {
        var date = DateOnly.FromDateTime(slot.Date);
        var todaysDate = DateOnly.FromDateTime(DateTime.Now);
        var currentHour = DateTime.Now.Hour; 
        
        if (date < todaysDate || (date == todaysDate && slot.StartHour <= currentHour))
        {
            failureReason = "Cannot book a past date.";
            return false;
        }
        
        if (slot.StartHour < 9 || slot.StartHour > 17)
        {
            failureReason = "Out of working hours (9 am to 6pm)";
            return false;
        }

        if (date.DayOfWeek == DayOfWeek.Sunday)
        {
            failureReason = "We don't work on Sunday's";
            return false; 
        }

        if (_appointments.TryGetValue(date, out var slots) && slots.ContainsKey(slot))
        {
            failureReason = "Slot is already booked";
            return false; 
        }

        failureReason = null;
        return true; 
    }

    public List<AppointmentSlot> GetEmptySlots(DateTime date)
    {
        var slots = new List<AppointmentSlot>();

        for (int hour = 9; hour <= 17; hour++)
        {
            var slot = new AppointmentSlot(date, hour); 

            if(IsSlotFree(slot, out _))
                slots.Add(slot);
        }

        return slots;
    }

    public AppointmentBookingResultSlim AddAppointment(Model.Appointment.Appointment appointment)
    {
        var slot = new AppointmentSlot(appointment.Date, appointment.StartHour);

        if (!IsSlotFree(slot, out var failureReason))
            return new(false, appointment, failureReason);

        //of course not ideal to show this to user... but this is matrix...
        if (_blackList.Contains(appointment.Name.ToLower().Trim()))
            return new(false, appointment, $"User {appointment.Name} is in black list");

        var date = DateOnly.FromDateTime(appointment.Date);

        if (!_appointments.TryGetValue(date, out var slots))
        {
            _appointments[date] = [];
        }

        _appointments[date][slot] = appointment;

        return new(true, appointment, null); 
    }

    public bool TryGetAppointment(AppointmentSlot slot, out Model.Appointment.Appointment appointment)
    {
        var date = DateOnly.FromDateTime(slot.Date);

        if (!_appointments.TryGetValue(date, out var slots))
        {
            appointment = null;
            return false; 
        }

        return _appointments[date].TryGetValue(slot, out appointment);
    }

    public void PrintCalendar()
    {
        foreach (var date in _appointments.Keys.OrderBy(k => k))
        {
            var slots = _appointments[date];

            BlueLine(date.ToString());
            Console.WriteLine(); 

            foreach (var slot in slots.Keys)
                GreenLine($"{slot.ToString()} - {_appointments[date][slot].Name}");

            Separator(); 
        }
    }
}
