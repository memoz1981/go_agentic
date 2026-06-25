namespace UdemyAICourseNotes.Model.Appointment;

internal record AppointmentLead(string Name, string Description, string Phone, AppointmentSlot[] Slots);
